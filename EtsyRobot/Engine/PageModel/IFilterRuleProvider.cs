using System;
using System.Collections.ObjectModel;

namespace EtsyRobot.Engine.PageModel
{
	internal interface IFilterRuleProvider
	{
		ReadOnlyCollection<IFilterRule> LoadRules();
	}
}