using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System.Linq;
using Ultra.Dal.Entities;

namespace Ultra.Dal.Plumbing
{
	public interface IStorage<T> where T : EntityBase
	{
		T GetById(ObjectId id);
		void SaveOrUpdate(T entity);
		IQueryable<T> RunQuery(IMongoQuery query);
	}

	public class Storage<T> : IStorage<T> where T : EntityBase
	{
		private readonly IMongoProvider _mongoProvider;

		public Storage(IMongoProvider mongoProvider)
		{
			_mongoProvider = mongoProvider;
		}

		public T GetById(ObjectId id)
		{
			return GetCollection().FindOneById(id);
		}

		public void SaveOrUpdate(T entity)
		{
			GetCollection().Save(entity);
		}

		public IQueryable<T> RunQuery(IMongoQuery query)
		{
			return GetCollection().Find(query).AsQueryable();
		}

		private MongoCollection<T> GetCollection()
		{
			string collectionName;

			var persistenceAttributes = typeof (T).GetCustomAttributes(typeof (PersistenceAttribute), true) as PersistenceAttribute[];

			if (persistenceAttributes != null)
				collectionName = persistenceAttributes.First().CollectionName;
			else
				collectionName = typeof (T).Name.ToLower();

			return _mongoProvider.Database.GetCollection<T>(collectionName);
		}
	}
}