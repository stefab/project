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
using System.Data.Entity;
using NLog;
using System.Diagnostics;
 

namespace EtsyRobot
{
    //public class Utils2
    //{
    //    myBindingSource.DataSource = (from m in context.myTable
    //                          where m.PROPERTY MEETS CONDITION
    //                          select m).ToList<TYPE>();
    //    // extension method
    //    public static IEnumerable<T> Filter<T>(this IEnumerable<T> list, Func<T, bool> filterParam)
    //    {
    //        return list.Where(filterParam);
    //    }
    //}
    

    public partial class MainFrm : Form
    {
        private CoreContext _dbContext;
        static private readonly Logger _log = LogManager.GetLogger("MainFrm");
        static private readonly TraceSource _tracer = new TraceSource("EtsyRobot.MainFrm", SourceLevels.All);

        public MainFrm()
        {
            InitializeComponent();
            //gameTypeLookupBindingSource.DataSource = Game.getGameTypeLookUp2();;
            //gameTypeDataGridViewColumn.DataSource = Game.getGameTypeLookUp().ToList<Object>();
            //gameTypeDataGridViewColumn.DisplayMember = "Name";
            //gameTypeDataGridViewColumn.ValueMember = "Value";
            _dbContext = new CoreContext();
            //DataSet
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadData();
        }

        private void  loadData()
        {
            _dbContext.Games.Load();
            _dbContext.Posts.Load();
            //DataView 
            //DataTable postsTable = new DataTable();
            postBindingSource.DataSource = _dbContext.Posts.Local.ToBindingList();
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
     
        
        private void btnAddPosts_Click(object sender, EventArgs e)
        {
            foreach (string postStr in edPosts.Lines)
            {
                Console.WriteLine(postStr.Trim());
                string preparedPostStr = postStr.Trim();
                if (preparedPostStr.Length > 0)
                {
                    Post post = new Post(preparedPostStr);
                    _dbContext.Posts.Add(post);
                }
            }
            this.Validate();
            _dbContext.SaveChanges();
            //postBindingSource.DataSource = null;
            postBindingSource.ResetBindings(true);
            //postBindingSource.DataSource = _dbContext.Posts.Local;

            postDataGridView.Update();
            postDataGridView.Refresh();
        }

        private void categoryBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            postDataGridView.EndEdit();
            _dbContext.SaveChanges();
        }

        private void postsBindingRefresh_Click(object sender, EventArgs e)
        {
            _dbContext.Posts.Load();
            postDataGridView.Refresh();
        }

        private void advancedDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void postDataGridView_SortStringChanged(object sender, EventArgs e)
        {
            postBindingSource.Sort = postDataGridView.SortString;
        }

        private void postDataGridView_FilterStringChanged(object sender, EventArgs e)
        {
            //postBindingSource.SupportsFiltering
            postBindingSource.Filter = postDataGridView.FilterString;
            //if (string.IsNullOrEmpty(this.postDataGridView.FilterString))
            //{
            //    this.postBindingSource.DataSource = _dbContext.Posts.Local.ToBindingList();
            //}
            //else
            //{
            //    var filteredData = _dbContext.Posts.Local.Where(x => x.PostAndUrl.Contains("g"));
            //    this.postBindingSource.DataSource = filteredData;
            //}

            //postBindingSource.ResetBindings(true);

            //  postDataGridView.Update();
            //  postDataGridView.Refresh();
        }

        private void gameBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private  async void btnFindGames_Click(object sender, EventArgs e)
        {
            GameFinderHandler handler = new GameFinderHandler(); 
            Job job = Job.Create("https://www.etsy.com/teams");
            job.EtsyUser = @"Alegraflowers";
            job.Password = @"konfetka39";
            //handler.Handle(job);
            try
            {
                var lst = await handler.HandleTaskAsync(job);
                _log.Debug("find games {0}", lst.Count());
            }
            catch(Exception ex)
            {
                _log.Debug("find games Error {0}", ex.Message);
            }
            
        }
    }
}
