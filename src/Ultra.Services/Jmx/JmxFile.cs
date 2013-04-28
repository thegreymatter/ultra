using System.Collections.Generic;

namespace Ultra.Services.JmxFile
{
	public class JmxFile
	{
		public string FileName { get; set; }
		public List<ThreadGroup> ThreadGroups { get; set; }

		public JmxFile()
		{
			ThreadGroups = new List<ThreadGroup>();
		}
	}

	public class ThreadGroup
	{
		public string Name { get; set; }
		public bool Enabled { get; set; }
	}
}