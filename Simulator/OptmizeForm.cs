using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Controls;
using Simples.SampleBased;
using Simples.Simulation.Planar2D;

namespace WindowsGame1
{
    public partial class OptmizeForm : Form
    {
        private RRTOptimizer optmizer;
        private PathSmoothing smoothing;
        private List<double> results;
        private NArticulatedPlanarController controller;

        public OptmizeForm(RRTOptimizer optmizer, NArticulatedPlanarController controller)
        {
            this.optmizer = optmizer;
            results = new List<double>();
            this.controller = controller;
            InitializeComponent();
        }

        private void OptmizeForm_Load(object sender, EventArgs e)
        {
            chart1.AddDataSeries("Test", Color.Blue, Chart.SeriesType.Line, 1, false);
     
            chart1.RangeY.Max = 1200;
            chart1.RangeY.Min = 400;

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            results.Clear();
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
            //double minDist = optmizer.MinDist;
            if (optmizer.bestDestNode != null)
            {
                double minDist = optmizer.bestDestNode.aTotalDist;
                label1.Text = minDist.ToString("0.00");
            }
            /*
            results.Add(Math.Min(minDist, 1000));
            double[,] values = new double[results.Count, 2];

            for (int i = 0; i < results.Count; i++)
            {
                values[i,0] = i;
                values[i,1] = results[i];
            }
            chart1.RangeX.Max = results.Count;
            
          

            
            chart1.UpdateDataSeries("Test", values);
             * */
        }

        private void OptmizeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            optmizer.Stop();
            smoothing.Stop();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Node previousNode;
            Node currentNode = optmizer.bestDestNode;

            controller.Clear();
            while (currentNode != null)
            {
                controller.AddPoint(currentNode.p);
                previousNode = currentNode;
                currentNode = currentNode.aCameFrom;
            }
            controller.running = true;
        }

        private void btnSmooth_Click(object sender, EventArgs e)
        {
            btnSmooth.Enabled = false;
            button1.Enabled = true;
            timer1.Enabled = true;
            /*
            double[] p1 = new double[2]{0,0};
            double[] p2 = new double[2]{0,1};
            double[] p3 = new double[2]{1,1};
            //double[] p4 = new double[2]{1,0};

            Node node1 = new Node(p1);
            
            Node node2 = new Node(p2);
            node2.aCameFrom = node1;
            
            Node node3 = new Node(p3);
            node3.aCameFrom = node2;
            
            List<Edge> edgeList = new List<Edge>();
            edgeList.Add(new Edge(node1, node2, 1, EdgeState.Free));
            edgeList.Add(new Edge(node2, node3, 1, EdgeState.Free));
            */
            smoothing = new PathSmoothing(optmizer.bestDestNode, optmizer.GetMechanism());
            /*
            for (int i = 0; i < 100000; i++)
            {
                PathSmoothing.Smooth();
            }
             */
                //label1.Text = optmizer.bestDestNode.aTotalDist.ToString("0.00");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnSmooth.Enabled = false;
            button1.Enabled = false;
            timer1.Enabled = false;
            smoothing.Stop();
        }
    }
}
