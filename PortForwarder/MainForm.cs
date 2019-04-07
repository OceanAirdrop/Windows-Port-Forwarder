using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace PortForwarder
{
    public partial class MainForm : Form
    {
        string m_command = "";

        private BackgroundWorker m_bgWorker = null;

        public MainForm()
        {
            InitializeComponent();

            m_bgWorker = new BackgroundWorker();
        }

        protected virtual void ThreadSafe(MethodInvoker method)
        {
            if (InvokeRequired)
                Invoke(method);
            else
                method();
        }


        private void buttonList_Click(object sender, EventArgs e)
        {
            ListPortForwardRules();
        }

        void ListPortForwardRulesThread(object sender, DoWorkEventArgs e)
        {
            try
            {
                m_command = "netsh interface portproxy show all";

                UpdateCommand(m_command);
                UpdateStatus("Getting List of Current Port Forward Rules");

                Process netStat = new Process();
                netStat.StartInfo.UseShellExecute = false;
                netStat.StartInfo.CreateNoWindow = true;
                netStat.StartInfo.FileName = @"cmd.exe";
                netStat.StartInfo.Arguments = string.Format(@"/C {0}", m_command);
                netStat.StartInfo.RedirectStandardOutput = true;
                netStat.Start();
                string output = netStat.StandardOutput.ReadToEnd();

                if (IsNullOrWhiteSpace(output) == true )
                {
                    output = "No portforwarding setup!";
                }

                UpdateOutput(output);

                UpdateStatus("Ready...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null) return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!Char.IsWhiteSpace(value[i])) return false;
            }

            return true;
        }

        private void ListPortForwardRules()
        {
            m_bgWorker.DoWork += ListPortForwardRulesThread;
            m_bgWorker.RunWorkerAsync("Message to worker");
        }

        private void UpdateOutput(string t)
        {
            ThreadSafe(delegate
            {
                richTextBox1.Text = t;
            });
        }

        private void UpdateStatus(string t)
        {
            ThreadSafe(delegate
            {
                toolStripStatusLabel.Text = t;
            });
        }

        private void UpdateCommand(string t)
        {
            ThreadSafe(delegate
            {
                textBoxCommand.Text = t;
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListPortForwardRules();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("You must run this application as administrator to add/delete port forward routes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void labelStatus_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonAddPortForwarder_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonListPortForwarding_Click(object sender, EventArgs e)
        {
            ListPortForwardRules();
        }

        private void toolStripButtonAddPortForward_Click(object sender, EventArgs e)
        {
            AddPortForward dlg = new AddPortForward();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_command = string.Format("netsh interface portproxy add v4tov4 listenport={0} listenaddress={1} connectport={2} connectaddress={3}", dlg.textBoxSourcePort.Text, dlg.textBoxSourceIP.Text, dlg.textBoxDestPort.Text, dlg.textBoxDestIP.Text);

                UpdateCommand(m_command);
                UpdateStatus("Adding Port Forward Rule");

                Process netStat = new Process();
                netStat.StartInfo.UseShellExecute = false;
                netStat.StartInfo.CreateNoWindow = true;
                netStat.StartInfo.FileName = @"cmd.exe";
                netStat.StartInfo.Arguments = string.Format(@"/C {0}", m_command);
                netStat.StartInfo.RedirectStandardOutput = true;
                netStat.Start();
                string output = netStat.StandardOutput.ReadToEnd();

                UpdateOutput(output);

                UpdateStatus("Ready...");
            }
        }

        private void toolStripButtonDeletePortForward_Click(object sender, EventArgs e)
        {
            DeletePortForward dlg = new DeletePortForward();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_command = string.Format("netsh interface portproxy delete v4tov4 listenport={0} listenaddress={1}", dlg.textBoxSourcePort.Text, dlg.textBoxSourceIP.Text);

                UpdateCommand(m_command);
                UpdateStatus("Adding Port Forward Rule");

                Process netStat = new Process();
                netStat.StartInfo.UseShellExecute = false;
                netStat.StartInfo.CreateNoWindow = true;
                netStat.StartInfo.FileName = @"cmd.exe";
                netStat.StartInfo.Arguments = string.Format(@"/C {0}", m_command);
                netStat.StartInfo.RedirectStandardOutput = true;
                netStat.Start();
                string output = netStat.StandardOutput.ReadToEnd();

                UpdateOutput(output);

                UpdateStatus("Ready...");
            }
        }
    }
}
