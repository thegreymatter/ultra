using System.Configuration;
using MongoDB.Driver;

namespace Ultra.Dal.Plumbing
{
	public interface IMongoProvider
	{
		MongoServer Server { get; }
		MongoDatabase Database { get; }
	}

	public class MongoProvider : IMongoProvider
	{
		private readonly MongoClient _client;
		private readonly MongoConnectionStringBuilder _connection;

		public static string ConnectionString 
		{
			get { return ConfigurationManager.ConnectionStrings["MongoDbConnectionString"].ConnectionString; }
		}

		public string DbName
		{
			get { return _connection.DatabaseName; }
		}

		public MongoServer Server
		{
			get { return _client.GetServer(); }
		}

		public MongoDatabase Database
		{
			get { return Server.GetDatabase(DbName); }
		}

		public MongoProvider()
		{
			_connection = new MongoConnectionStringBuilder(ConnectionString);
			_client = new MongoClient();
		}
	}
}