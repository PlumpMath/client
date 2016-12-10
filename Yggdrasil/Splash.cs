using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yggdrasil
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            this.Show();
            status.Text = Properties.strings.Loading;
        }
    }
}
