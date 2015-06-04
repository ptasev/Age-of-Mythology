using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AoMModelPlugin
{
    public partial class ModelPlugin : Form
    {
        public BrgFile file;
        public static event EventHandler testEvent;

        public ModelPlugin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (testEvent != null)
            {
                file = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\archer n throwing axeman_attacka.brg", FileMode.Open, FileAccess.Read, FileShare.Read));
                testEvent(sender, e);
            }
        }
    }
}
