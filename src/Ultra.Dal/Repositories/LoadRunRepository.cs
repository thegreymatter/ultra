using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Repositories
{
	public interface ILoadRunRepository
	{
		LoadRun[] GetMostRecentLoadRuns(int amount);
		LoadRun GetLoadRun(ObjectId loadRunId);
		LoadRun[] GetPendingLoadRuns();
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
			var cursor = _loadRunStorage.GetAll().SetSortOrder("StartTime").SetLimit(amount);
			return cursor.ToArray();
		}

		public LoadRun GetLoadRun(ObjectId loadRunId)
		{
			return _loadRunStorage.GetById(loadRunId);
		}

		public LoadRun[] GetPendingLoadRuns()
		{
			return _loadRunStorage.RunQuery(Query.EQ("Status", "1")).ToArray();
		}
	}
}