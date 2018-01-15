using System;
using System.Data.Entity;

namespace Softengi.PageComparer.Core.Framework.Data
{
	public interface IContextFactory
	{
		TCtx CreateContext<TCtx>() where TCtx : DbContext;
	}
}