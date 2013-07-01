using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;

using Ultra.Dal.Entities;
using Ultra.Dal.Repositories;

namespace Ultra.Services.JMeterOutput
{
	public class JMeterOutputAnalyzer : IJMeterOutputAnalyzer
	{
		private readonly Dictionary<string, ThreadPoolStats> _threadPoolStats = new Dictionary<string, ThreadPoolStats>();
		private readonly int _elapsedThreshold = int.Parse(ConfigurationManager.AppSettings["ResponseTimeThreshold"]);

		public readonly ISet<string> _blacklist;
		public static string JMeterOutputArchive = ConfigurationManager.AppSettings["OutputArchive"];

		private  Dictionary<string, int> OutputFileMapper;

		public JMeterOutputAnalyzer(IBlackListRepository blacklist)
		{
			_blacklist = blacklist.GetBlackList();
		}

		public JMeterOutputAnalyzer()
		{
			_blacklist  = new HashSet<string>();
		}
		public RunResults Analyze(string filename, JmxSettings runSettings)
		{
			
			var totalViews = 0;
			var minTimeStamp = DateTime.MaxValue;
			var maxTimeStamp = DateTime.MinValue;
			DateTime? firstRequestTimestamp = null;
			DateTime? timestamp = null;

			_threadPoolStats.Clear();
			OutputFileMapper = new Dictionary<string, int>();
			using (var filestream = new StreamReader(filename))
			{


				MapOutputFileFields(filestream.ReadLine());

				string line;
				while ((line = filestream.ReadLine()) != null)
				{
					var parsedSet = ParseLine(line);

					if (!firstRequestTimestamp.HasValue)
						firstRequestTimestamp = parsedSet.TimeStamp;

					if ((parsedSet.TimeStamp - firstRequestTimestamp.Value).TotalSeconds < runSettings.RampUp)
						continue;

					if (!parsedSet.IsAjax()) ++totalViews;
					if (_blacklist.Contains(parsedSet.ThreadPoolName))
						continue;
					AddParsedDataToStats(parsedSet, parsedSet.IsAjax());
					timestamp = parsedSet.TimeStamp;
					if (timestamp < minTimeStamp) minTimeStamp = timestamp.Value;
					if (timestamp > maxTimeStamp) maxTimeStamp = timestamp.Value;
				}
			}
			var overallExecutionTime = (maxTimeStamp.Subtract(firstRequestTimestamp.Value)).TotalSeconds;

			if (timestamp == null)
				throw new RunWasTooShortForAnalysisException(
					"It seems that you are trying to analyze a file that was running for a short period. RampUp = " + runSettings.RampUp);

			return new RunResults {
				Threads = _threadPoolStats.Values.ToList(),
				RunningTime = (int)(overallExecutionTime / 60),
				StartTime = firstRequestTimestamp.Value,
				EndTime = timestamp.Value,
				PVS = (int)(totalViews / overallExecutionTime),
				OutputFilename = Path.GetFileName(filename)
			};
		}

		private void MapOutputFileFields(string fieldsLine)
		{
			var fields = fieldsLine.Split(',').ToArray();

			for (int i = 0; i < fields.Length; i++)
			{
				OutputFileMapper.Add(fields[i].ToLower(), i);
			}
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


		private  ParsedSet ParseLine(string line)
		{
			var fields = line.Split(',');

			return new ParsedSet
			{
				TimeStamp = DateTime.Parse(fields[OutputFileMapper["timestamp"]]),
				Elapsed = Convert.ToInt32(fields[OutputFileMapper["elapsed"]]),
				ThreadPoolName = fields[OutputFileMapper["label"]],
				ResponseCode = fields[OutputFileMapper["responsecode"]],
				Url = fields[OutputFileMapper["url"]]
			};
		}
	}

	


	public struct RunResults
	{
		public IList<ThreadPoolStats> Threads { get; set; }

		public string OutputFilename { get; set; }
		public int RunningTime { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

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