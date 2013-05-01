using System;

namespace Ultra.Util.ArgumentParsing
{
	public class ArgumentParsingException : Exception
	{
		public ArgumentParsingException() : base("There was an error while parsing the input arguments")
		{
		}
	}
}