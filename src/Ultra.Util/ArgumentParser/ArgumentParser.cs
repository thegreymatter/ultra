using System;
using System.Linq;

namespace Ultra.Util.ArgumentParser
{
	public class ArgumentParser
	{
		private readonly string[] _validKeyValues = new[] {"--filename", "--duration", "--rampup", "--domain"};
		private readonly string[] _validFlags = new[] {"--analyze"};

		public UtilArguments ParseArguments(string[] args)
		{
			var arguments = new UtilArguments();

			for (var i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith("--"))
				{
					if (_validKeyValues.Any(x => x.Equals(args[i], StringComparison.OrdinalIgnoreCase)))
					{
						if (args.Length == i + 1 || !args[i+1].StartsWith("--"))
							throw new ArgumentParsingException();

						arguments.KeyValues.Add(args[i].Substring(2).ToLower(), args[i + 1]);
						continue;
					}

					if (_validFlags.Any(x => x.Equals(args[i], StringComparison.OrdinalIgnoreCase)))
					{
						arguments.Flags.Add(args[i].Substring(2));
						continue;
					}

					throw new ArgumentParsingException();
				}
			}

			return arguments;
		}
	}
}