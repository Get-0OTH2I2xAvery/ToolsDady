using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

namespace ToolsAveryDady
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void reconButton1_Click(object sender, EventArgs e)
        {
            information_box.Text = fonction("https://api.hackertarget.com/nping/?q=" + Enter_tools.Text);
        }

        public static string fonction(string url_Api)
        {
            WebClient download = new WebClient();
            string result = download.DownloadString(url_Api);
            return result;
        }

        private void reconButton2_Click(object sender, EventArgs e)
        {
            information_box.Text = fonction("https://api.hackertarget.com/dnslookup/?q=" + Enter_tools.Text);
        }

        private void reconButton3_Click(object sender, EventArgs e)
        {
            information_box.Text = fonction("https://api.hackertarget.com/nmap/?q=" + Enter_tools.Text);
        }

        private void reconButton4_Click(object sender, EventArgs e)
        {
            information_box.Text = fonction("https://api.hackertarget.com/whois/?q=" + Enter_tools.Text);
        }

        private void reconButton5_Click(object sender, EventArgs e)
        {
            information_box.Text = fonction("https://api.hackertarget.com/pagelinks/?q=" + Enter_tools.Text);
        }

        private void reconButton6_Click(object sender, EventArgs e)
        {
            information_box.Text = fonction("https://api.hackertarget.com/geoip/?q=" + Enter_tools.Text);
        }

        private void reconForm1_Click(object sender, EventArgs e)
        {

        }

        private void reconButton7_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void reconButton8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Mon discord >𝐀𝐯𝐞𝐫𝐲.✧#0001 / Mon serveur https://discord.gg/mq9EJNn", "A propos ToolsAveryDady", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start("https://discord.gg/mq9EJNn");
        }
    }
}
