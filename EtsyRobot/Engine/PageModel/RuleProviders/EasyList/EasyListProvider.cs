using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

using EtsyRobot.Properties;

namespace EtsyRobot.Engine.PageModel.RuleProviders.EasyList
{
	internal class EasyListProvider : IFilterRuleProvider
	{
		private EasyListProvider(Settings settings)
		{
			this._settings = settings;
		}

		static public EasyListProvider Instance
		{
			get { return _instance.Value; }
		}

		#region IFilterRuleProvider Members
		public ReadOnlyCollection<IFilterRule> LoadRules()
		{
            //_tracer.TraceEvent(TraceEventType.Verbose, 0, "EasyList loading started...");

            //if (!Directory.Exists(this._settings.EasyListSourcePath))
            //{
            //	throw new ApplicationException("The EasyList rules folder does not exist.");
            //}

            //IEnumerable<string> files = Directory.EnumerateFiles(this._settings.EasyListSourcePath, "*.txt",
            //                                                     SearchOption.AllDirectories);
            List<EasyListFilterRule> rules = new List<EasyListFilterRule> { };

			//List<EasyListFilterRule> rules = (from file in files
			//                                  from line in File.ReadLines(file)
			//                                  select new EasyListFilterRule(line)).ToList();

			//_tracer.TraceEvent(TraceEventType.Verbose, 0, "EasyList loading finished.");

			return rules.Cast<IFilterRule>().ToList().AsReadOnly();
		}
		#endregion

		static private readonly TraceSource _tracer = new TraceSource("Engine.PageModel.RuleProviders.EasyList", SourceLevels.All);

		static private readonly Lazy<EasyListProvider> _instance =
			new Lazy<EasyListProvider>(() => new EasyListProvider(Settings.Default));

		private readonly Settings _settings;
	}
}