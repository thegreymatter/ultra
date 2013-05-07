using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Ultra.Services.Jmx;

namespace Ultra.Services.JMeterOutput
{
	public class JMeterOutputAnalyzer : IJMeterOutputAnalyzer
	{
		private readonly Dictionary<string, ThreadPoolStats> _threadPoolStats = new Dictionary<string, ThreadPoolStats>();
		private readonly int _elapsedThreshold = int.Parse(ConfigurationManager.AppSettings["ResponseTimeThreshold"]);

		public RunResults Analyze(string filename, JmxSettings runSettings)
		{
			var totalViews = 0;
			var minTimeStamp = DateTime.MaxValue;
			var maxTimeStamp = DateTime.MinValue;
			DateTime? firstRequestTimestamp = null;

			using (var fileStream = new StreamReader(filename))
			{
				fileStream.ReadLine(); //throw away first line (field names)

				string line;
				while ((line = fileStream.ReadLine()) != null)
				{
					var parsedSet = ParseLine(line, runSettings);

					if (!firstRequestTimestamp.HasValue)
						firstRequestTimestamp = parsedSet.TimeStamp;

					if ((parsedSet.TimeStamp - firstRequestTimestamp.Value).TotalMinutes < runSettings.RampUp)
						continue;

					if (!parsedSet.IsAjax()) ++totalViews;

					AddParsedDataToStats(parsedSet, parsedSet.IsAjax());
					var timestamp = parsedSet.TimeStamp;
					if (timestamp < minTimeStamp) minTimeStamp = timestamp;
					if (timestamp > maxTimeStamp) maxTimeStamp = timestamp;
				}
				fileStream.Close();
			}

			var overallExecutionTime = (maxTimeStamp.Subtract(minTimeStamp)).TotalSeconds;
			
			return new RunResults { Threads = _threadPoolStats.Values.ToList(), PVS = (int)(totalViews / overallExecutionTime) };
		}

		private void AddParsedDataToStats(ParsedSet data, bool isAjax)
		{
			var threadPoolName = data.ThreadPoolName;
			ThreadPoolStats stats;
			if (!_threadPoolStats.ContainsKey(threadPoolName))
			{
				stats = new ThreadPoolStats(threadPoolName, isAjax);
				_threadPoolStats.Add(threadPoolName, stats);
			}
			else
			{
				stats = _threadPoolStats[threadPoolName];
			}
			stats.AddRequest(data, (data.Elapsed > _elapsedThreshold));
		}


		private static ParsedSet ParseLine(string line, JmxSettings runSettings)
		{
			// TODO: add dynamic mapper here, so it won't matter what format the columns are in

			var fields = line.Split(',');

			return new ParsedSet
			{
				TimeStamp = DateTime.Parse(fields[0]),
				Elapsed = Convert.ToInt32(fields[1]),
				ThreadPoolName = fields[2],
				ResponseCode = fields[3],
				Url = fields[10]
			};
		}
	}

	public struct RunResults
	{
		public IList<ThreadPoolStats> Threads { get; set; }
		public double PVS { get; set; }
	}

	public struct ParsedSet
	{
		public DateTime TimeStamp { get; set; }
		public string ResponseCode { get; set; }
		public int Elapsed { get; set; }
		public string ThreadPoolName { get; set; }
		public string Url { get; set; }

		public bool IsAjax()
		{
			return Url.Contains("/-/");
		}
	}
}