using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Entities
{
	[Persistence(CollectionName = "configuration")]
	public class UltraConfiguration : EntityBase
	{
		public string[] Servers{ get; set; }
		public string JMeterBatFile { get; set; }
		public string JmxScripts { get; set; }
		public string JmxFileArchive { get; set; }
		public string OutputArchive { get; set; }

		public JmxSettings DefaultSettings { get; set; }

		public UltraConfiguration()
		{
			DefaultSettings = new JmxSettings();
		}
	}
}