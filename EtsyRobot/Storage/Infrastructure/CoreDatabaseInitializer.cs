using System;
using System.Data.Entity;

using EtsyRobot.Storage.Model;
using EtsyRobot.Migrations;

namespace EtsyRobot.Storage.Infrastructure
{
    //internal class CoreDatabaseInitializer : DropCreateDatabaseIfModelChanges<CoreContext>
    internal class CoreDatabaseInitializer : MigrateDatabaseToLatestVersion<CoreContext, Configuration>
	{
        //protected override void Seed(CoreContext context)
        //{
        //    var workload = new Workload
        //        {
        //            Name = "PrivDog demo workload",
        //        };

        //    workload.AddItem("http://www.gardenweb.com/");
        //    workload.AddItem("http://2ch-c.net/");
        //    workload.AddItem("http://www.hln.be/");
        //    workload.AddItem("http://www.shape.com/lifestyle/mind-and-body/5-common-body-goals-are-unrealistic");
        //    workload.AddItem("http://www.menshealth.com/");
        //    workload.AddItem("http://www.wnd.com/");
        //    workload.AddItem("http://www.ithov.com/");
        //    workload.AddItem("http://www.hotsarcade.net/play/829/flower-girl?gmplay");
        //    workload.AddItem("http://bestwapcom.net/mans-head-cut-nigeriairan-football-match/");
        //    workload.AddItem("http://www.livetvstreamin.com/");

        //    context.Workloads.Add(workload);
        //}
	}
}