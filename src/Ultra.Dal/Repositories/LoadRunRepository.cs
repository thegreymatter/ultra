using System.Linq;
using MongoDB.Driver.Builders;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Repositories
{
	public interface ILoadRunRepository
	{
		LoadRun[] GetMostRecentLoadRuns(int amount);
	}

	public class LoadRunRepository : ILoadRunRepository
	{
		private readonly IStorage<LoadRun> _loadRunStorage;

		public LoadRunRepository(IStorage<LoadRun> loadRunStorage)
		{
			_loadRunStorage = loadRunStorage;
		}

		public LoadRun[] GetMostRecentLoadRuns(int amount)
		{
			var cursor = _loadRunStorage.RunQueryOnAll().SetSortOrder("StartTime").SetLimit(amount);
			return cursor.ToArray();
		}
	}
}