using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Repositories
{
	public interface ILoadRunRepository
	{
		LoadRun[] GetMostRecentLoadRuns(int amount,int skip);
		LoadRun GetLoadRun(ObjectId loadRunId);
		LoadRun[] GetPendingLoadRuns();
		void CreateLoadRun(LoadRun loadRun);
		void DeleteLoadRun(ObjectId loadRunId);
		void SaveOrUpdate(LoadRun loadRun);
		LoadRun GetLoadRunByOutPutFilename(string filename);
	}

	public class LoadRunRepository : ILoadRunRepository
	{
		private readonly IStorage<LoadRun> _loadRunStorage;

		public LoadRunRepository(IStorage<LoadRun> loadRunStorage)
		{
			_loadRunStorage = loadRunStorage;
		}

		public LoadRun[] GetMostRecentLoadRuns(int amount,int skip)
		{
			var cursor = _loadRunStorage.GetAll().SetSortOrder(SortBy.Descending("StartTime")).SetLimit(amount).SetSkip(skip);
			return cursor.ToArray();
		}

		public LoadRun GetLoadRun(ObjectId loadRunId)
		{
			return _loadRunStorage.GetById(loadRunId);
		}

		public LoadRun GetLoadRunByOutPutFilename(string filename)
		{
			var query = Query<LoadRun>.EQ(x => x.RunOutputFilename, filename);
			return _loadRunStorage.RunQuery(query).FirstOrDefault();
		}

		public LoadRun[] GetPendingLoadRuns()
		{
			var query = Query<LoadRun>.EQ(x => x.Status, LoadRunStatus.Pending);
			var mongoCursor = _loadRunStorage.RunQuery(query);
			return mongoCursor.ToArray();
		}

		public void CreateLoadRun(LoadRun loadRun)
		{
			_loadRunStorage.SaveOrUpdate(loadRun);
		}

		public void DeleteLoadRun(ObjectId loadRunId)
		{
			_loadRunStorage.Delete(loadRunId);
		}

		public void SaveOrUpdate(LoadRun loadRun)
		{
			_loadRunStorage.SaveOrUpdate(loadRun);
		}
	}
}