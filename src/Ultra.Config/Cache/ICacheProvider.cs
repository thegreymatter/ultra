using System;
using System.Collections;
using System.Web;

namespace Ultra.Config.Cache
{
	public interface ICacheProvider
	{
		IDictionary GetCache();
		T Get<T>(string key, Func<T> creator) where T : class;
		void Invalidate(string key);
	}

	public class CacheProvider : ICacheProvider
	{
		public IDictionary GetCache()
		{
			var httpContext = HttpContext.Current;
			return httpContext == null ? null : HttpContext.Current.Items;
		}

		private readonly object _nullPlaceholder = new object();
		public T Get<T>(string key, Func<T> creator)
			where T : class
		{
			if (GetCache() == null)
				return creator();

			var cache = GetCache();

			var item = cache[key];
			if (_nullPlaceholder == item)
				return null;

			var t = item as T;
			if (t != null)
				return t;

			t = creator();
			cache[key] = t ?? _nullPlaceholder;

			return t;
		}

		public void Invalidate(string key)
		{
			if (GetCache() == null)
				return;

			var cache = GetCache();
			cache[key] = null;
		}
	}
}