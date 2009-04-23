using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int k = 15;
            int N = 100;
            Bitmap myCSpace = pictureBox1.Image as Bitmap;
            Graphics graphicsObj = Graphics.FromImage(myCSpace);

            Boolean[,] obsMatrix = new  Boolean[myCSpace.Width, myCSpace.Height];
            for (int x = 0; x < myCSpace.Width -1; x++)
                for (int y = 0; y < myCSpace.Height - 1; y++)
                {
                    obsMatrix[x, y] = myCSpace.GetPixel(x, y).ToArgb() == Color.Black.ToArgb();
                }
            CObsSpace2d cObsSpace = new CObsSpace2d(obsMatrix);



            CSpacePRM tst = new CSpacePRM(2, new int[] { 800, 600 }, cObsSpace, N, k, PRMSampleMethod.Lattice);

            Node originNode;
            Node destNode;

            int[] origin = new int[2] { 10, 10 };
            int[] dest = new int[2] { 800 - 10, 600 - 10 };

            tst.generatePath(origin, dest, k, out originNode, out destNode);

            Pen myPen = new Pen(System.Drawing.Color.Lime, 1);
            foreach (Node node in tst.nodeList)
            {
                foreach (Edge edge in node.childs)
                {
                    graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                        edge.node2.p[0], edge.node2.p[1]);
                }
            }


            myPen = new Pen(System.Drawing.Color.Blue, 1);

            foreach (Edge edge in originNode.childs)
            {
                graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                    edge.node2.p[0], edge.node2.p[1]);
            }

            foreach (Edge edge in destNode.childs)
            {
                graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                    edge.node2.p[0], edge.node2.p[1]);
            }

            graphicsObj.DrawEllipse(myPen, originNode.p[0], originNode.p[1] - 1, 3, 3);

            graphicsObj.DrawEllipse(myPen, destNode.p[0], destNode.p[1] - 1, 3, 3);

            myPen = new Pen(System.Drawing.Color.Red, 2);

            Node previousNode = destNode;
            Node currentNode = destNode.aCameFrom;

            while (currentNode != null)
            {
                graphicsObj.DrawLine(myPen, previousNode.p[0], previousNode.p[1],
                    currentNode.p[0], currentNode.p[1]);

                previousNode = currentNode;
                currentNode = currentNode.aCameFrom;
            }

            myPen = new Pen(System.Drawing.Color.Red, 1);
            foreach (int[] a in tst.sampleList)
            {
                graphicsObj.DrawEllipse(myPen, (int)a[0] - 1, (int)a[1] - 1, 3, 3);
            }


            pictureBox1.Image = myCSpace;
        }

        public Bitmap drawResults(CSpaceRRT cSpace, Node originNode, Node destNode)
        {

            Bitmap image = new Bitmap(pictureBox1.Image);
            Graphics graphicsObj = Graphics.FromImage(image);

            Pen myPen = new Pen(System.Drawing.Color.Blue, 1);
            foreach (Edge edge in cSpace.startTree.edgeList)
            {
                graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                    edge.node2.p[0], edge.node2.p[1]);
            }


            myPen = new Pen(System.Drawing.Color.Green, 1);
            foreach (Edge edge in cSpace.goalTree.edgeList)
            {
                graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                    edge.node2.p[0], edge.node2.p[1]);
            }

            myPen = new Pen(System.Drawing.Color.Red, 2);

            Node previousNode = destNode;
            Node currentNode = destNode.aCameFrom;

            while (currentNode != null)
            {
                graphicsObj.DrawLine(myPen, previousNode.p[0], previousNode.p[1],
                    currentNode.p[0], currentNode.p[1]);

                previousNode = currentNode;
                currentNode = currentNode.aCameFrom;
            }

            return image;

        }

        public Bitmap drawResults(CSpacePRM cSpace, Node originNode, Node destNode)
        {
            Bitmap image = new Bitmap(pictureBox1.Image);

            Graphics graphicsObj = Graphics.FromImage(image);


            Pen myPen = new Pen(System.Drawing.Color.Lime, 1);
            foreach (Node node in cSpace.nodeList)
            {
                foreach (Edge edge in node.childs)
                {
                    graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                        edge.node2.p[0], edge.node2.p[1]);
                }
            }


            myPen = new Pen(System.Drawing.Color.Blue, 1);

            foreach (Edge edge in originNode.childs)
            {
                graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                    edge.node2.p[0], edge.node2.p[1]);
            }

            foreach (Edge edge in destNode.childs)
            {
                graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                    edge.node2.p[0], edge.node2.p[1]);
            }

            graphicsObj.DrawEllipse(myPen, originNode.p[0], originNode.p[1] - 1, 3, 3);

            graphicsObj.DrawEllipse(myPen, destNode.p[0], destNode.p[1] - 1, 3, 3);

            myPen = new Pen(System.Drawing.Color.Red, 2);

            Node previousNode = destNode;
            Node currentNode = destNode.aCameFrom;

            while (currentNode != null)
            {
                graphicsObj.DrawLine(myPen, previousNode.p[0], previousNode.p[1],
                    currentNode.p[0], currentNode.p[1]);

                previousNode = currentNode;
                currentNode = currentNode.aCameFrom;
            }

            myPen = new Pen(System.Drawing.Color.Red, 1);
            foreach (int[] a in cSpace.sampleList)
            {
                graphicsObj.DrawEllipse(myPen, (int)a[0] - 1, (int)a[1] - 1, 3, 3);
            }

            return image;

        }

        public SimulationData simulate(int[] origin, int[] dest, int N, int k, CObsSpace cObsSpace)
        {
            SimulationData data = new SimulationData();
            
            CSpaceRRT cSpace = new CSpaceRRT(2, new int[] { 800, 600 }, cObsSpace, k);

            DateTime counter = DateTime.Now;
            data.sampleTime = DateTime.Now.Subtract(counter);
            
            Node originNode;
            Node destNode;
            
            counter = DateTime.Now;
            cSpace.generatePath(origin, dest,out originNode, out destNode);
            data.graphSearchTime = DateTime.Now.Subtract(counter);

            data.k = k;
            data.N = N;
            data.dist = destNode.aDist;
            data.image = drawResults(cSpace, originNode, destNode);

            if (destNode.aCameFrom == null)
            {
                data.nodeCount = -1;
            }
            else
            {
                Node currentNode = destNode.aCameFrom;
                data.nodeCount = 0;
                while (currentNode != null)
                {
                    data.nodeCount = data.nodeCount + 1;
                    currentNode = currentNode.aCameFrom;
                }

            }
            
            return data;
         
        }

        public SimulationData simulateRandom(int[] origin, int[] dest, int N, int k, CObsSpace cObsSpace)
        {
            SimulationData data = new SimulationData();

            CSpacePRM cSpace = new CSpacePRM(2, new int[] { 800, 600 }, cObsSpace, N, k, PRMSampleMethod.Random);

            DateTime counter = DateTime.Now;
            data.sampleTime = DateTime.Now.Subtract(counter);

            counter = DateTime.Now;
            data.roadMapTime = DateTime.Now.Subtract(counter);

            Node originNode;
            Node destNode;

            counter = DateTime.Now;
            cSpace.generatePath(origin, dest, k, out originNode, out destNode);
            data.graphSearchTime = DateTime.Now.Subtract(counter);

            data.k = k;
            data.N = N;
            data.dist = destNode.aDist;
            data.image = drawResults(cSpace, originNode, destNode);

            if (destNode.aCameFrom == null)
            {
                data.nodeCount = -1;
            }
            else
            {
                Node currentNode = destNode.aCameFrom;
                data.nodeCount = 0;
                while (currentNode != null)
                {
                    data.nodeCount = data.nodeCount + 1;
                    currentNode = currentNode.aCameFrom;
                }

            }

            return data;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap myCObsSpace = pictureBox1.Image as Bitmap;
            Graphics graphicsObj = Graphics.FromImage(myCObsSpace);

            Boolean[,] obsMatrix = new Boolean[myCObsSpace.Width, myCObsSpace.Height];
            for (int x = 0; x < myCObsSpace.Width - 1; x++)
                for (int y = 0; y < myCObsSpace.Height - 1; y++)
                {
                    obsMatrix[x, y] = myCObsSpace.GetPixel(x, y).ToArgb() == Color.Black.ToArgb();
                }

            CObsSpace2d cObsSpace = new CObsSpace2d(obsMatrix);
            StreamWriter sw = new StreamWriter("result.txt");

            int[] origin = new int[2] { 10, 10 };
            int[] dest = new int[2] { 800 - 10, 600 - 10 };

            int k = 4;
            int n = 200;

            SimulationData data;

            while (k <= 80)
            {
                data = simulate(origin, dest, n, k, cObsSpace);
                data.image.Save(k.ToString().PadLeft(2, '0') + ".bmp");
                sw.WriteLine(data.N.ToString() + ";"+
                    data.k.ToString() + ";" +
                    data.dist.ToString() + ";" +
                    data.nodeCount.ToString() + ";"+
                    data.sampleTime.TotalSeconds.ToString() + ";" +
                    data.roadMapTime.TotalSeconds.ToString() + ";" +
                    data.graphSearchTime.TotalSeconds.ToString() + ";");
                sw.Flush();
                k = k + 2;

            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap myCObsSpace = pictureBox1.Image as Bitmap;
            Graphics graphicsObj = Graphics.FromImage(myCObsSpace);

            Boolean[,] obsMatrix = new Boolean[myCObsSpace.Width, myCObsSpace.Height];
            for (int x = 0; x < myCObsSpace.Width - 1; x++)
                for (int y = 0; y < myCObsSpace.Height - 1; y++)
                {
                    obsMatrix[x, y] = myCObsSpace.GetPixel(x, y).ToArgb() == Color.Black.ToArgb();
                }

            CObsSpace2d cObsSpace = new CObsSpace2d(obsMatrix);
            StreamWriter sw = new StreamWriter("result.txt");

            int[] origin = new int[2] { 10, 10 };
            int[] dest = new int[2] { 800 - 10, 600 - 10 };

            int k = 30;
            int n = 200;
            int i = 1;

            SimulationData data;

            data = simulate(origin, dest, n, k, cObsSpace);
            data.image.Save("0.bmp");
            sw.WriteLine(data.N.ToString() + ";" +
                data.k.ToString() + ";" +
                data.dist.ToString() + ";" +
                data.nodeCount.ToString() + ";" +
                data.sampleTime.TotalSeconds.ToString() + ";" +
                data.roadMapTime.TotalSeconds.ToString() + ";" +
                data.graphSearchTime.TotalSeconds.ToString() + ";");
            sw.Flush();

            while (i <= 50)
            {
                data = simulateRandom(origin, dest, n, k, cObsSpace);
                data.image.Save(i.ToString().PadLeft(2, '0') + ".bmp");
                sw.WriteLine(data.N.ToString() + ";" +
                    data.k.ToString() + ";" +
                    data.dist.ToString() + ";" +
                    data.nodeCount.ToString() + ";" +
                    data.sampleTime.TotalSeconds.ToString() + ";" +
                    data.roadMapTime.TotalSeconds.ToString() + ";" +
                    data.graphSearchTime.TotalSeconds.ToString() + ";");
                sw.Flush();
                i++;

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap myCSpace = pictureBox1.Image as Bitmap;
            Graphics graphicsObj = Graphics.FromImage(myCSpace);

            Boolean[,] obsMatrix = new Boolean[myCSpace.Width, myCSpace.Height];
            for (int x = 0; x < myCSpace.Width - 1; x++)
                for (int y = 0; y < myCSpace.Height - 1; y++)
                {
                    obsMatrix[x, y] = myCSpace.GetPixel(x, y).ToArgb() == Color.Black.ToArgb();
                }

            CObsSpace2d cObsSpace = new CObsSpace2d(obsMatrix);

            int[] origin = new int[2] { 10, 10 };
            int[] dest = new int[2] { 800 - 10, 600 - 10 };

            int k = 5000;
            CSpaceRRT cSpace = new CSpaceRRT(2, new int[] { 800, 600 },cObsSpace, k);


            Node originNode;
            Node destNode;

            cSpace.generatePath(origin, dest, out originNode, out destNode);

            Pen myPen = new Pen(System.Drawing.Color.Blue, 1);
            foreach (Edge edge in cSpace.startTree.edgeList)
            {
                graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                    edge.node2.p[0], edge.node2.p[1]);
            }


            myPen = new Pen(System.Drawing.Color.Green, 1);
            foreach (Edge edge in cSpace.goalTree.edgeList)
            {
                graphicsObj.DrawLine(myPen, edge.node1.p[0], edge.node1.p[1],
                    edge.node2.p[0], edge.node2.p[1]);
            }


            myPen = new Pen(System.Drawing.Color.Red, 1);
            foreach (int[] p in cSpace.sampleList)
            {
                graphicsObj.DrawEllipse(myPen, p[0] - 1, p[1] - 1, 3, 3);
            }

            myPen = new Pen(System.Drawing.Color.Red, 2);

            Node previousNode = destNode;
            Node currentNode = destNode.aCameFrom;

            while (currentNode != null)
            {
                graphicsObj.DrawLine(myPen, previousNode.p[0], previousNode.p[1],
                    currentNode.p[0], currentNode.p[1]);

                previousNode = currentNode;
                currentNode = currentNode.aCameFrom;
            }
            
            pictureBox1.Image = myCSpace;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap myCObsSpace = pictureBox1.Image as Bitmap;
            Graphics graphicsObj = Graphics.FromImage(myCObsSpace);

            Boolean[,] obsMatrix = new Boolean[myCObsSpace.Width, myCObsSpace.Height];
            for (int x = 0; x < myCObsSpace.Width - 1; x++)
                for (int y = 0; y < myCObsSpace.Height - 1; y++)
                {
                    obsMatrix[x, y] = myCObsSpace.GetPixel(x, y).ToArgb() == Color.Black.ToArgb();
                }

            CObsSpace2d cObsSpace = new CObsSpace2d(obsMatrix);
            StreamWriter sw = new StreamWriter("result.txt");

            int[] origin = new int[2] { 10, 10 };
            int[] dest = new int[2] { 800 - 10, 600 - 10 };

            int k = 10;
            int n = 200;

            SimulationData data;

            while (k <= 1000)
            {
                data = simulate(origin, dest, n, k, cObsSpace);
                data.image.Save(k.ToString().PadLeft(2, '0') + ".bmp");
                sw.WriteLine(data.N.ToString() + ";" +
                    data.k.ToString() + ";" +
                    data.dist.ToString() + ";" +
                    data.nodeCount.ToString() + ";" +
                    data.sampleTime.TotalSeconds.ToString() + ";" +
                    data.roadMapTime.TotalSeconds.ToString() + ";" +
                    data.graphSearchTime.TotalSeconds.ToString() + ";");
                sw.Flush();
                k = k + 20;

            }
        }
    }
    public class SimulationData
    {
        public int N;
        public int k;
        public TimeSpan sampleTime;
        public TimeSpan roadMapTime;
        public TimeSpan graphSearchTime;
        public Bitmap image;

        public double dist;
        public int nodeCount;
    }

}
