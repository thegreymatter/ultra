using System;

namespace Ultra.Dal.Plumbing
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class PersistenceAttribute : Attribute
	{
		public string CollectionName { get; set; }
	}
}