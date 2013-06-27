using System.IO;
using Ultra.Dal.Entities;

namespace Ultra.Services.JMeterOutput
{
	public interface IJMeterOutputAnalyzer
	{
		RunResults Analyze( string filename, JmxSettings runSettings);
	}
}