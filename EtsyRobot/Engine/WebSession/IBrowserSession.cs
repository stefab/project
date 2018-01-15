using System;

using EtsyRobot.Engine.PageModel;

namespace EtsyRobot.Engine.WebSession
{
	public interface IBrowserSession : IDisposable
	{
        PageContent Scrape(Uri uri, bool isReferenceScraping = true);
	}
}