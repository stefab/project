using System;

namespace EtsyRobot.Engine.PageModel
{
	internal interface IFilterRule
	{
		bool IsAdContent(string url);
	}
}