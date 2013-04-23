using System.Configuration;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace Ultra.Dal.Plumbing
{
	public class DbPersistanceFactory
	{
		public static string ConnString
		{
			get { return ConfigurationManager.ConnectionStrings["WebSiteApp"].ConnectionString; }
		}

		public static NHibernate.Cfg.Configuration BuildDatabaseConfiguration()
		{
			return Fluently.Configure()
					.Database(MySQLConfiguration.Standard.ConnectionString(ConnString))
					.Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
					.BuildConfiguration();
		}
	}
}