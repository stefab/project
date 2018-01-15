using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EtsyRobot.Engine.PageModel.RuleProviders.EasyList
{
	public class EasyListFilterRule : IFilterRule
	{
		public EasyListFilterRule(string filter)
		{
			this._filter = filter;
		}

		#region IFilterRule Members
		public bool IsAdContent(string url)
		{
			string pattern = this._filter.Trim();

			//Ignore comments: !
			if (pattern.StartsWith(COMMENT_KEY) || (pattern.StartsWith("[") || pattern.EndsWith("]")))
			{
				return false;
			}

			//Ignore filter with hidding block: ##, #@#
			if (pattern.Contains(ELEMENT_HIDDING_BLOCK_KEY) || pattern.Contains(ELEMENT_HIDDING_BLOCK_INVERSE_KEY))
			{
				return false;
			}

			//Include inverse rule: @@
			bool isInverseRule = pattern.StartsWith(INVERSE_KEY);
			if (isInverseRule)
			{
				pattern = pattern.Substring(2);
			}

			//Get clear filter pattern, exclude options: $...
			//FYI: if hidding block rule will be  activated then need also exclude hidding block component from filter
			int filterOptionsIndex = pattern.LastIndexOf(FILTER_OPTIONS_KEY);
			if (filterOptionsIndex > -1)
			{
				pattern = pattern.Substring(0, filterOptionsIndex);

				//FYI: options not hanlde yet
			}

			//Regular expression treatment: /.../
			if (pattern.StartsWith(REG_EXP_KEY.ToString(CultureInfo.InvariantCulture)) &&
			    pattern.EndsWith(REG_EXP_KEY.ToString(CultureInfo.InvariantCulture)))
			{
				try
				{
					var regex = new Regex(pattern.Substring(1, pattern.Length - 2), RegexOptions.Compiled);
					Match match = regex.Match(url);
					return match.Success;
				}
				catch (ArgumentException)
				{
					//Continue as simple pattern
				}
			}

			//Handle domain rule
			if (pattern.StartsWith(DOMAIN_KEY))
			{
				//FYI: not finished yet
			}

			//Beginning of an address treatment: |
			if (pattern.StartsWith(BEGIN_END_KEY.ToString(CultureInfo.InvariantCulture)))
			{
				pattern = pattern.Substring(1);
			}
			else if (!pattern.StartsWith(WILDCARD_KEY.ToString(CultureInfo.InvariantCulture)))
			{
				pattern = WILDCARD_KEY + pattern;
			}

			//End of an address treatment: |
			if (pattern.Trim().EndsWith(BEGIN_END_KEY.ToString(CultureInfo.InvariantCulture)))
			{
				pattern = pattern.Substring(0, pattern.Length - 1);
			}
			else if (!pattern.EndsWith(WILDCARD_KEY.ToString(CultureInfo.InvariantCulture)))
			{
				pattern = pattern + WILDCARD_KEY;
			}


			bool result = MatchWildcardString(pattern, url);
			return isInverseRule ? !result : result;
		}
		#endregion

		static private bool MatchWildcardString(string pattern, string input)
		{
			if (String.CompareOrdinal(pattern, input) == 0)
			{
				return true;
			}

			if (String.IsNullOrEmpty(input))
			{
				if (String.IsNullOrEmpty(pattern.Trim(new[] {WILDCARD_KEY})))
				{
					return true;
				}
				return false;
			}

			if (pattern.Length == 0)
			{
				return false;
			}

			if (pattern[0] == SEPARATOR_KEY && IsSeparatorCharacter(input[0]))
			{
				return MatchWildcardString(pattern.Substring(1), input.Substring(1));
			}

			if (pattern[pattern.Length - 1] == SEPARATOR_KEY && IsSeparatorCharacter(input[input.Length - 1]))
			{
				return MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input.Substring(0, input.Length - 1));
			}

			if (pattern[0] == WILDCARD_KEY)
			{
				if (MatchWildcardString(pattern.Substring(1), input))
				{
					return true;
				}
				return MatchWildcardString(pattern, input.Substring(1));
			}

			if (pattern[pattern.Length - 1] == WILDCARD_KEY)
			{
				if (MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input))
				{
					return true;
				}
				return MatchWildcardString(pattern, input.Substring(0, input.Length - 1));
			}

			if (pattern[0] == input[0])
			{
				return MatchWildcardString(pattern.Substring(1), input.Substring(1));
			}

			return false;
		}

		static private bool IsSeparatorCharacter(Char chr)
		{
			return !Char.IsLetterOrDigit(chr) && chr != '_' && chr != '-' && chr != '.' && chr != '%';
		}

		private const Char WILDCARD_KEY = '*';
		private const Char SEPARATOR_KEY = '^';
		private const Char BEGIN_END_KEY = '|';
		private const Char REG_EXP_KEY = '/';
		private const Char FILTER_OPTIONS_KEY = '$';
		private const string ELEMENT_HIDDING_BLOCK_KEY = "##";
		private const string ELEMENT_HIDDING_BLOCK_INVERSE_KEY = "#@#";
		private const string DOMAIN_KEY = "||";
		private const string COMMENT_KEY = "!";
		private const string INVERSE_KEY = "@@";
		private readonly string _filter;
	}
}