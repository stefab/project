using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;

using Microsoft.CSharp.RuntimeBinder;

using Newtonsoft.Json.Linq;

using EtsyRobot.Properties;

namespace EtsyRobot.Engine.PageModel.RuleProviders.PrivDog
{
	internal class PrivDogProvider : IFilterRuleProvider
	{
		private PrivDogProvider(Settings settings)
		{
			this._settings = settings;
		}

		static public PrivDogProvider Instance
		{
			get { return _instance.Value; }
		}

		#region IFilterRuleProvider Members
		public ReadOnlyCollection<IFilterRule> LoadRules()
		{
			_tracer.TraceEvent(TraceEventType.Verbose, 0, "PrivDog rules loading started...");

			string json = "{}";
			try
			{
				//json = new WebClient().DownloadString(this._settings.PrivDogRulesAddress);
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
				{
					var resp = (HttpWebResponse) ex.Response;
					if (resp.StatusCode == HttpStatusCode.NotFound)
					{
						throw new ApplicationException("The json file does not exist.", ex);
					}
				}
				throw;
			}

			try
			{
				dynamic jObject = JObject.Parse(json);

				var filters = new List<IFilterRule>();
                // default filters
                // trustedads.adtrustmedia.com, s.atmsrv.com
                filters.Add(new PrivDogFilterRule("s.atmsrv.com"));
                // add from json
				foreach (dynamic item in jObject.r)
				{
					dynamic type = item.t;
					if (type == "FQDN")
					{
						dynamic host = item.r.ToString();
						filters.Add(new PrivDogFilterRule(host));
					}
				}
				_tracer.TraceEvent(TraceEventType.Verbose, 0, "PrivDog rules loading finished.");

				return filters.AsReadOnly();
			}
			catch (RuntimeBinderException ex)
			{
				throw new ApplicationException("The json file structure does not match the filter property.", ex);
			}
		}
		#endregion

		static private readonly TraceSource _tracer = new TraceSource("Engine.PageModel.RuleProviders.PrivDog", SourceLevels.All);

		static private readonly Lazy<PrivDogProvider> _instance =
			new Lazy<PrivDogProvider>(() => new PrivDogProvider(Settings.Default));

		private readonly Settings _settings;
	}
}