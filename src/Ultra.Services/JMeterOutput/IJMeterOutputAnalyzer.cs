using Ultra.Services.Jmx;

namespace Ultra.Services.JMeterOutput
{
	public interface IJMeterOutputAnalyzer
	{
		RunResults Analyze(string filename, JmxSettings runSettings);
	}
}