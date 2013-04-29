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

			using (var fileStream = new StreamReader(filename))
			{
				fileStream.ReadLine(); //throw away first line (field names)

				string line;
				while ((line = fileStream.ReadLine()) != null)
				{
					var parsedSet = ParseLine(line);
					if (!parsedSet.IsAjax()) ++totalViews;

					AddParsedDataToStats(parsedSet, parsedSet.IsAjax());
					var timestamp = parsedSet.TimeStamp;
					if (timestamp < minTimeStamp) minTimeStamp = timestamp;
					if (timestamp > maxTimeStamp) maxTimeStamp = timestamp;
				}
				fileStream.Close();
			}

			var overallExecutionTime = (maxTimeStamp.Subtract(minTimeStamp)).TotalSeconds;
			
			return new RunResults { Threads = _threadPoolStats.Values.ToList(), PVS = totalViews / overallExecutionTime };
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
			stats.AddRequest(data.Elapsed, (data.Elapsed > _elapsedThreshold), data.ResponseCode != "200");
		}


		private static ParsedSet ParseLine(string line)
		{
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

	internal struct ParsedSet
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

	public class ThreadPoolStats
	{
		private readonly string _threadPoolName;
		private int _requestCount = 0;
		private int _requestsAboveThresholdCount = 0;
		private int _responseTimeSum; //used to calculate avg response times
		private int _overThresholdResponseTimeSum; //used to calculate avg response times over threshold
		private int _errorCount;
		public bool IsAjax { get; set; }

		private readonly Dictionary<int, int> _responseTimesDistribution = new Dictionary<int, int>();

		public ThreadPoolStats(string threadPoolName, bool isAjax)
		{
			_threadPoolName = threadPoolName;
			IsAjax = isAjax;
		}

		public void AddRequest(int responseTime, bool isAboveThreshold, bool isError)
		{
			_responseTimeSum += responseTime;

			++_requestCount;

			if (isAboveThreshold)
			{
				++_requestsAboveThresholdCount;
				_overThresholdResponseTimeSum += responseTime;
			}

			if (isError) ++_errorCount;

			var responseTimeBucket = (responseTime/5)*5;
			if (_responseTimesDistribution.ContainsKey(responseTimeBucket))
			{
				++_responseTimesDistribution[responseTimeBucket];
			}
			else
			{
				_responseTimesDistribution[responseTimeBucket] = 1;
			}
		}


		public double GetErrorPercent()
		{
			if (_requestCount == 0) return 0;
			var percent = ((double) _errorCount/_requestCount)*100;
			return percent;
		}

		public int GetAvgResponseTime()
		{
			if (_requestCount == 0) return 0;
			return _responseTimeSum/_requestCount;
		}

		public int GetAvgResponseTimeAboveThreshold()
		{
			if (_requestCount == 0 || _requestsAboveThresholdCount == 0) return 0;
			return _overThresholdResponseTimeSum/_requestsAboveThresholdCount;
		}

		public double GetPercentAboveThreshold()
		{
			if (_requestCount == 0) return 0;
			var percent = ((double) _requestsAboveThresholdCount/_requestCount)*100;
			return percent;
		}

		public int GetRequestCount()
		{
			return _requestCount;
		}

		public string GetThreadPoolName()
		{
			return _threadPoolName;
		}

		public int GetPercentileX(int X)
		{
			var destinationCount = ((double) _requestCount * X) / 100;
			var orderedDistribution = _responseTimesDistribution.ToList().OrderBy(x => x.Key);
			var requestCount = 0;
			foreach (var bucket in orderedDistribution)
			{
				requestCount += bucket.Value;
				if (requestCount >= destinationCount) return bucket.Key;
			}
			return 0;
		}
	}
}