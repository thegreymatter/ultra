using System;
using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Entities
{
	[Persistence(CollectionName = "loadruns")]
	public class LoadRun : EntityBase
	{
		public LoadRunStatus Status { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		// TODO: maybe we should put this in a 'settings' sub-document
		public string JmxFilename { get; set; }
		public string[] Servers { get; set; }
		public string Domain { get; set; }
		public int Duration { get; set; }
		public int RampUp { get; set; }
		public string RunOutputFilename { get; set; }
		public string Label { get; set; }
		public double TotalPvs { get; set; }
		public PageMetric[] PageMetrics { get; set; }
		public string Remarks { get; set; }
	}

	public enum LoadRunStatus
	{
		Pending = 1,
		Running = 2,
		Finished = 3
	}

	public class PageMetric
	{
		public string PageName { get; set; }
		public int AverageResponseTime { get; set; }
		public int Percentile90 { get; set; }
		public double ErrorRate { get; set; }
		public double PercentOverThreshold { get; set; }
		public int RequestCount { get; set; }
		public bool IsAjax { get; set; }
	}
}