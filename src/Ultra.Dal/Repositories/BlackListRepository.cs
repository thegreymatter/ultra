using System.Collections.Generic;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;
using System.Linq;

namespace Ultra.Dal.Repositories
{


	public interface IBlackListRepository
	{
		ISet<string> GetBlackList();
	}

	public class BlackListRepository : IBlackListRepository
	{
		private readonly IStorage<BlackList> _storage;

		public BlackListRepository(IStorage<BlackList> storage)
		{
			_storage = storage;
		}


		public ISet<string> GetBlackList()
		{
			var list = _storage.GetAll().FirstOrDefault();
			if (list != null)
			{
				return new HashSet<string>(list.Labels);
			}
			else
			{
				return new HashSet<string>();
			}
		}
	}

}

