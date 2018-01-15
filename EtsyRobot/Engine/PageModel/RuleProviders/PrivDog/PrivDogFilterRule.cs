using System;

namespace EtsyRobot.Engine.PageModel.RuleProviders.PrivDog
{
	public sealed class PrivDogFilterRule : IFilterRule
	{
		public PrivDogFilterRule(string host)
		{
			this._domain = GetDomainName(host);
		}

		#region IFilterRule Members
		public bool IsAdContent(string url)
		{
			string fullUrl = url.StartsWith("//") ? Uri.UriSchemeHttp + ":" + url : url;
			Uri uri;

			if (Uri.TryCreate(fullUrl, UriKind.Absolute, out uri))
			{
				if (uri.HostNameType == UriHostNameType.Dns)
				{
					return GetDomainName(uri.Host) == this._domain;
				}
			}

			return false;
		}
		#endregion

		static private string GetDomainName(string host)
		{
			string domain = host;
			string[] tokens = domain.Split('.');
			if (tokens.Length > 2)
			{
				int validTokens = 2;

				//Add only second level exceptions to the < 3 rule here
				string[] exceptions = {"info", "firm", "name", "com", "biz", "gen", "ltd", "web", "net", "pro", "org"};
				if (tokens[tokens.Length - 2].Length < 3 || Array.Exists(exceptions, t => t == tokens[tokens.Length - 2]))
				{
					validTokens += 1;
				}

				domain = string.Join(".", tokens, tokens.Length - validTokens, validTokens);
			}
			return domain;
		}
       
		private readonly string _domain;
	}
}