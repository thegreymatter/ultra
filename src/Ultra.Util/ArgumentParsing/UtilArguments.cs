using System.Collections.Generic;

namespace Ultra.Util.ArgumentParsing
{
	public class UtilArguments
	{
		public Dictionary<string, string> KeyValues { get; set; }
		public List<string> Flags { get; set; }

		public UtilArguments()
		{
			KeyValues = new Dictionary<string, string>();
			Flags = new List<string>();
		}
	}
}