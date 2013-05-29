using System;
using System.Runtime.Serialization;

namespace Ultra.Services.JMeterOutput
{
	[Serializable]
	public class AnalysisException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public AnalysisException()
		{
		}

		public AnalysisException(string message) : base(message)
		{
		}

		public AnalysisException(string message, Exception inner) : base(message, inner)
		{
		}

		protected AnalysisException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}

	public class RunWasTooShortForAnalysisException : AnalysisException
	{
		public RunWasTooShortForAnalysisException()
		{
		}

		public RunWasTooShortForAnalysisException(string message) : base(message)
		{
		}

		public RunWasTooShortForAnalysisException(string message, Exception inner) : base(message, inner)
		{
		}

		protected RunWasTooShortForAnalysisException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}		
	}
}