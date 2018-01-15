using System;

using EtsyRobot.Storage.Model;

namespace EtsyRobot.Worker
{
	internal abstract class JobHandler
	{
		public abstract void Handle(Job job);

		static public JobHandler CreateGameHandler()
		{
			return new GameHandler(true);
		}

		static public JobHandler CreateTestScrapingHandler()
		{
			return new GameHandler(false);
		}

		static public JobHandler CreateComparisonHandler()
		{
			return new GameHandler(false);
		}
	}
}