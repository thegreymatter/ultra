using System.Collections.Generic;
using Ultra.Dal.Entities;

namespace Ultra.Models
{
	public class RunPageModel
	{
		public string[] JmxFiles { get; set; }
		public LoadRun[] LoadRuns { get; set; }
	}
}