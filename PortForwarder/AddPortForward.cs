using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using System.Windows.Forms;

namespace PortForwarder
{
    public partial class AddPortForward : Form
    {
        public AddPortForward()
        {
            InitializeComponent();
        }

        public string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private void AddPortForward_Load(object sender, EventArgs e)
        {
            textBoxSourceIP.Text = LocalIPAddress();
        }
    }
}
