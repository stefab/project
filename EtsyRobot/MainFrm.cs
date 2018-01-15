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
            GameHandler handler = new GameHandler(true); //JobHandler.CreateGameHandler();
            Job job =  Job.Create("http://obkom.net.ua");
            handler.Handle(job);
        }
    }
}
