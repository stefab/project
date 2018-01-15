using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace EtsyRobot.Engine.PageModel
{
	 class AdContentChecker
	{
		public AdContentChecker(IFilterRuleProvider ruleProvider)
		{
			this._rules = new Lazy<IEnumerable<IFilterRule>>(ruleProvider.LoadRules);
		}

		public bool IsAdContent(Node node)
		{
			if (IsTooSmall(node))
			{
				return false;
			}

			if (IsIconImage(node))
			{
				return false;
			}

			return this._rules.Value.Any(r => r.IsAdContent(node.ContentUrl));
		}

		static private bool IsTooSmall(Node node)
		{
			var minSize = new Size(20, 20);
			return node.Size.Height < minSize.Height && node.Size.Width < minSize.Width;
		}

		static private bool IsIconImage(Node node)
		{
			//TODO: Check the node selector to ensure that node tag is IMG.
			return node.ContentUrl.EndsWith(".ico");
		}

		private readonly Lazy<IEnumerable<IFilterRule>> _rules;
	}
}