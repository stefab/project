using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EtsyRobot.Worker;
using EtsyRobot.Storage.Model;
using System.Text.RegularExpressions;
using EtsyRobot.Storage.Infrastructure;

namespace EtsyRobot
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        private void btnTestClick(object sender, EventArgs e)
        {
            using (var db = new CoreContext())
            {
                db.Database.Initialize(true);
                db.Jobs.Add(Job.Create("http://cxxcxcx"));
                db.SaveChanges();
            }

            String pattern = @"(post.*?(?<gr_post>\d+).*?(favorite|fave?).*?(?<gr_fave>\d+))|" +
                             @"((favorite|fave?).*?(?<gr_fave>\d+).*?post.*?(?<gr_post>\d+))";

            String data =   "Post 134, " +
                            "Fave 50 above You. Great promo game! " +
                            "Favorite 137,  " +
                            "Posting 54 above You. Great promo game!";

            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            MatchCollection matches = rgx.Matches(data);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Console.WriteLine("'{0}' repeated at positions {1} and {2}",
                                  groups["gr_post"].Value,
                                  groups["gr_fave"].Value,
                                  groups[0].Index);
            }

            // init db
            // Data Source="P:\var\EtsyRobot\etsy_robot.db;Version=3;Journal Mode=Persist;"


            GameHandler handler = new GameHandler(true); //JobHandler.CreateGameHandler();
            Job job =  Job.Create("https://www.etsy.com/teams");
            job.EtsyUser = @"Alegraflowers";
            job.Password = @"konfetka39";
            handler.Handle(job);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void jobBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void jobDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void gameDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
