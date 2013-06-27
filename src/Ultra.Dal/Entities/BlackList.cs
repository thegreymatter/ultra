using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Entities
{
	[Persistence(CollectionName = "blacklist")]
	public class BlackList : EntityBase
	{
		public  string[] Labels;
	}

}
