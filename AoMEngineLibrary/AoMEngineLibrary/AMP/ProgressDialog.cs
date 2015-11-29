using MaxCustomControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AoMEngineLibrary.AMP
{
    public partial class ProgressDialog : MaxForm
    {
        public ProgressDialog()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.progressBar.Style = ProgressBarStyle.Marquee;
        }

        public void SetProgressText(string text)
        {
            if (progressLabel.InvokeRequired)
            {
                progressLabel.BeginInvoke(new Action(() =>
                {
                    progressLabel.Text = text;
                }));
            }
            else
            {
                progressLabel.Text = text;
            }
        }

        public void SetProgressValue(int value)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.BeginInvoke(
                    new Action(() =>
                    {
                        progressBar.Value = value;
                    }
                ));
            }
            else
            {
                progressBar.Value = value;
            }

            if (percentageLabel.InvokeRequired)
            {
                percentageLabel.BeginInvoke(new Action(() =>
                {
                    percentageLabel.Text = string.Format("{0}%", value);
                }));
            }
            else
            {
                percentageLabel.Text = string.Format("{0}%", value);
            }
        }
    }
}
