using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Controls;
using Simples.SampledBased;

namespace WindowsGame1
{
    public partial class OptmizeForm : Form
    {
        private RRTOptimizer optmizer;
        double[,] values = new double[50, 2];
        
        public OptmizeForm(RRTOptimizer optmizer)
        {
            this.optmizer = optmizer;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
      
        }

        private void OptmizeForm_Load(object sender, EventArgs e)
        {
            chart1.AddDataSeries("Test", Color.Blue, Chart.SeriesType.ConnectedDots, 2, false);
            for (int i = 0; i < 50; i++)
            {
                values[i, 0] = i;
               
            }
            chart1.RangeX.Max = 50;
            chart1.RangeX.Min = 0;
            chart1.RangeY.Max = 1200;
            chart1.RangeY.Min = 0;

            chart1.UpdateDataSeries("Test", values);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            optmizer.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            timer1.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            optmizer.Stop();
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            for (int i = 1; i < values.Length/2; i++)
            {
                values[i - 1,1] = values[i,1];
            }

            values[49, 1] = Math.Min(optmizer.MinDist, 1000);
            chart1.UpdateDataSeries("Test", values);
        }
    }
}
