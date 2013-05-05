using System;
using System.Collections.Generic;
using System.Linq;

namespace Ultra.Services.JMeterOutput
{
	public class ThreadPoolStats
	{
		private readonly string _threadPoolName;
		private int _requestCount = 0;
		private int _requestsAboveThresholdCount = 0;
		private int _responseTimeSum; //used to calculate avg response times
		private int _overThresholdResponseTimeSum; //used to calculate avg response times over threshold
		private int _errorCount;
		public bool IsAjax { get; set; }

		public List<ParsedSet> Requests { get; set; }

		private readonly Dictionary<int, int> _responseTimesDistribution = new Dictionary<int, int>();

		public ThreadPoolStats(string threadPoolName, bool isAjax)
		{
			_threadPoolName = threadPoolName;
			IsAjax = isAjax;
			Requests = new List<ParsedSet>();
		}

		public void AddRequest(ParsedSet set, bool isAboveThreshold)
		{
			Requests.Add(set);
			var isError = set.ResponseCode != "200";

			_responseTimeSum += set.Elapsed;

			++_requestCount;

			if (isAboveThreshold)
			{
				++_requestsAboveThresholdCount;
				_overThresholdResponseTimeSum += set.Elapsed;
			}

			if (isError) ++_errorCount;

			var responseTimeBucket = (set.Elapsed / 5) * 5;
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