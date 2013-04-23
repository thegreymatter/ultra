using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Entities
{
	[Persistence(TableName = "test")]
	public class TestEntity : EntityBase
	{
		public string Name { get; set; }
	}
}