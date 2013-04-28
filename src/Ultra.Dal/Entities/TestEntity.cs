using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Entities
{
	[Persistence(CollectionName = "loadruns")]
	public class LoadRun : EntityBase
	{
		public string JmxFilename { get; set; }
		public string Domain { get; set; }
		public int Duration { get; set; }
		public int RampUp { get; set; }
		public string RunOutputFilename { get; set; }
	}
}