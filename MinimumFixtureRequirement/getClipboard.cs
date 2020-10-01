using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinimumFixtureRequirement
{
    public partial class getClipboard : Form
    {
        public getClipboard()
        {
            InitializeComponent();
            webBrowser1.Navigate("http://localhost:8080/#/minimumfixturerequirement");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //e.Cancel = true;
            //Process process = Process.Start("https://google.com");
        }

        private void getClipboard_Load(object sender, EventArgs e)
        {

        }
    }
}
