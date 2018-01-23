using System;

using EtsyRobot.Engine.PageModel;
using EtsyRobot.Engine.WebSession.EtsyUtils;

namespace EtsyRobot.Engine.WebSession
{
	internal interface IBrowserSession : IDisposable
	{
        PageContent Scrape(Uri uri, bool isReferenceScraping = true);
        void ProcessPage(Uri uri, EtsyStrategy strategy);
	}
}