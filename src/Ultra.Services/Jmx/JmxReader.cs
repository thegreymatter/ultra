using System.Xml.Linq;

namespace Ultra.Services.JmxFile
{
	public class JmxReader : IJmxReader
	{
		public JmxFile ReadFile(string jmxFilename)
		{
			var jmxDoc = XDocument.Load(jmxFilename);

			// TODO: extract this to a separate jmx parser
			var jmxFile = new JmxFile();
			jmxFile.FileName = jmxFilename;
			foreach (var threadGroup in jmxDoc.Descendants("ThreadGroup"))
			{
				jmxFile.ThreadGroups.Add(new ThreadGroup {
					Name = threadGroup.Attribute("testname").Value,
					Enabled = bool.Parse(threadGroup.Attribute("enabled").Value)
				});
			}

			return jmxFile;
		}
	}
}