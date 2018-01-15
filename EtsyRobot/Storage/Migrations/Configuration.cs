namespace EtsyRobot.Storage.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using EtsyRobot.Storage.Model;

    
    public class Configuration : DbMigrationsConfiguration<EtsyRobot.Storage.Infrastructure.CoreContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "EtsyRobot.Storage.Infrastructure.CoreContext";
        }

        protected override void Seed(EtsyRobot.Storage.Infrastructure.CoreContext context)
        {
            //  This method will be called after migrating to the latest version.

            //var workload = new Workload
            //{
            //    Name = "PrivDog demo workload",
            //};

            //workload.AddItem("http://www.gardenweb.com/");
            //workload.AddItem("http://2ch-c.net/");
            //workload.AddItem("http://www.hln.be/");
            //workload.AddItem("http://www.shape.com/lifestyle/mind-and-body/5-common-body-goals-are-unrealistic");
            //workload.AddItem("http://www.menshealth.com/");
            //workload.AddItem("http://www.wnd.com/");
            //workload.AddItem("http://www.ithov.com/");
            //workload.AddItem("http://www.hotsarcade.net/play/829/flower-girl?gmplay");
            //workload.AddItem("http://bestwapcom.net/mans-head-cut-nigeriairan-football-match/");
            //workload.AddItem("http://www.livetvstreamin.com/");

            //context.Workloads.Add(workload);

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
