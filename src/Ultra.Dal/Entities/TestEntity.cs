using System;
using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Entities
{
	[Persistence(CollectionName = "loadruns")]
	public class LoadRun : EntityBase
	{
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		public string JmxFilename { get; set; }
		public string Domain { get; set; }
		public int Duration { get; set; }
		public int RampUp { get; set; }
		public string RunOutputFilename { get; set; }
	}
}