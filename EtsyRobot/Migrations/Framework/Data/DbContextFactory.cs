using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;

//using Enviance.Eve.Framework.Utils;

//namespace Softengi.PageComparer.Core.Framework.Data
//{
//	public class DbContextFactory : IContextFactory
//	{
//		public DbContextFactory()
//		{}

//		public DbContextFactory(string containerName, string providerConnectionString)
//		{
//			Guard.AgainstNull(containerName, "containerName");

//			this._containerName = containerName;
//			this._providerConnectionString = providerConnectionString;
//		}

//		#region IContextFactory Members
//		public TCtx CreateContext<TCtx>() where TCtx : DbContext
//		{
//			if (!string.IsNullOrEmpty(this._containerName))
//			{
//				string connectionString = this.BuildConnectionString();
//				return (TCtx) Activator.CreateInstance(typeof(TCtx), connectionString);
//			}
//			return (TCtx) Activator.CreateInstance(typeof(TCtx));
//		}
//		#endregion

//		/// <summary>
//		/// DB provider connection string used for context creation.
//		/// </summary>
//		protected virtual string ProviderConnectionString
//		{
//			get { return this._providerConnectionString; }
//		}

//		private string BuildConnectionString()
//		{
//			ConnectionStringSettings originalString = ConfigurationManager.ConnectionStrings[this._containerName];
//			var entityBuilder = new EntityConnectionStringBuilder(originalString.ConnectionString)
//				{
//					ProviderConnectionString = this.ProviderConnectionString
//				};
//			return entityBuilder.ConnectionString;
//		}

//		private readonly string _containerName;
//		private readonly string _providerConnectionString;
//	}
//}