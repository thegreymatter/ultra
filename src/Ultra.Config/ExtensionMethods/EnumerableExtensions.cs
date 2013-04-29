using System;
using System.Collections.Generic;

namespace Ultra.Config.ExtensionMethods
{
	public static class EnumerableExtensions
	{
		 public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		 {
			 foreach (var t in collection)
				 action(t);
		 }
	}
}