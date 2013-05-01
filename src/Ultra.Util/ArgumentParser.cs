using System;
using System.Collections.Generic;
using System.Linq;

namespace Ultra.Util
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
						// TODO: make sure the next value is not a flag and exists in the array - throw proper exception
						arguments.KeyValues.Add(args[i].Substring(2).ToLower(), args[i + 1]);
						continue;
					}

					if (_validFlags.Any(x => x.Equals(args[i], StringComparison.OrdinalIgnoreCase)))
					{
						arguments.Flags.Add(args[i].Substring(2));
						continue;
					}

					// TODO: we don't recognize this parameter, throw proper exception
				}
			}

			return arguments;
		}
	}

	public class UtilArguments
	{
		public Dictionary<string, string> KeyValues { get; set; };
		public List<string> Flags { get; set; }

		public UtilArguments()
		{
			KeyValues = new Dictionary<string, string>();
			Flags = new List<string>();
		}
	}
}