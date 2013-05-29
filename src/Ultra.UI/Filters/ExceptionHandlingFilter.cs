using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Ultra.Filters
{
	public class ExceptionHandlingFilterAttribute : FilterAttribute, IExceptionFilter
	{
		private static readonly JsonSerializerSettings JsonSerializationSettings = new JsonSerializerSettings()
		{
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore
		};

		public void OnException(ExceptionContext filterContext)
		{
			var httpContext = filterContext.HttpContext;

			// Removes all of the buffered response  
			httpContext.Response.Clear();

			var exception = filterContext.Exception;

			filterContext.ExceptionHandled = true;
			
			HandleError(exception, filterContext);
		}

		private void HandleError(Exception exception, ControllerContext filterContext)
		{
			var statusCode = 500;
			var context = filterContext.HttpContext;
			context.Response.StatusCode = statusCode;
			context.Response.StatusDescription = "ERROR";

			ConvertExceptionToJson(context, statusCode, exception.Message);
		}

		private static void ConvertExceptionToJson(HttpContextBase context, int statusCode, string message)
		{
			context.Response.ContentType = "application/json";

			var responseObj = new
			{
				Error = new
				{
					StatusCode = statusCode,
					Message = message,
				}
			};
			context.Response.Write(JsonConvert.SerializeObject(responseObj, Formatting.None, JsonSerializationSettings));
		}
	}

	public static class RequestExtensions
	{
		public static bool IsAjax(this HttpRequestBase request)
		{
			return string.IsNullOrEmpty(request.Headers["X-Requested-With"]) == false;
		}
	}
}