using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace XmlReceiptReader
{
    public partial class Progress : Form
    {
        private static System.Timers.Timer timer;

        public static string fileName = String.Empty;

        public Progress()
        {
            InitializeComponent();
        }

        delegate void SetTextCallback();

        private void SetData()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (textBoxLog.InvokeRequired)
            {
                SetTextCallback delegated = new SetTextCallback(SetData);
                this.Invoke(delegated);
            }
            else
            {
                textBoxLog.Text = String.Empty;

                
            }
        }

        private void GetData(object source, ElapsedEventArgs e)
        {
            SetData();
        }

        private void Progress_Load(object sender, EventArgs e)
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(GetData);
            timer.Interval = 10;
            timer.Enabled = true;
        }
    }
}
