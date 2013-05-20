using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using Ultra.Dal.Entities;

namespace Ultra.Dal.Plumbing
{
	public interface IStorage<T> where T : EntityBase
	{
		T GetById(ObjectId id);
		void SaveOrUpdate(T entity);
		MongoCursor<T> RunQuery(IMongoQuery query);
		MongoCursor<T> GetAll();
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

		public MongoCursor<T> RunQuery(IMongoQuery query)
		{
			return GetCollection().Find(query);
		}

		public MongoCursor<T> GetAll()
		{
			return GetCollection().FindAll();
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