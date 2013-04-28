namespace Ultra.Services.JmxFile
{
	public interface IJmxReader
	{
		JmxFile ReadFile(string jmxFilename);
	}
}