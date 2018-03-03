using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

//namespace Softengi.PageComparer.Core.Framework.Data
//{
//	static public class ConnectionStringUtils
//	{
//		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
//		static public string AddApplicationName(string connString)
//		{
//			var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connString)
//			                                 	{
//			                                 		ApplicationName = ConfigurationManager.AppSettings["ApplicationName"]
//			                                 	};
//			return sqlConnectionStringBuilder.ConnectionString;
//		}
//	}
//}