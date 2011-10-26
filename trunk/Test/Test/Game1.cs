using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Simples.PathPlan.SampleBased;
using Simples.Mechanisms;
using Simples.Mechanisms.NArticulatedPlanar;
using Simples.Mechanisms.SampleBased;
using Simples.PathPlan.SampleBased.RRT;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Test
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        Model linkModel;
        Model sceneModel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"D:\Projetos\obstavoid\Simples.Simulator\bin\x86\Debug\Content";
            
        }
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            linkModel = Content.Load<Model>("link");
            sceneModel = Content.Load<Model>("scene");
        }

        public void RunTests()
        {
            Matrix world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            Vector3 minBound = new Vector3(-10, -10, -5);
            Vector3 maxBound = new Vector3(120, 10, 5);
            Mechanism robot = NArticulatedPlanarMechanism.Create(linkModel, new Vector3(100, 0, 0), minBound,
                maxBound, 6, world);

            MechanismEnviroment enviroment = new MechanismEnviroment(sceneModel);

           
            
            double[] origin = new double[6] { 150, 0, 0, 0, 0, 0 };
            double[] dest = new double[6] { 90, 0, 0, 0, 0, 0 };
            FileStream fs = new FileStream("test1.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            ManualResetEvent stopEvent = new ManualResetEvent(false);
            for (int i = 0; i < 10; i++)
            {
                Node originNode, destNode;
                DateTime start, end;
                TimeSpan elapsedTime;
                int iterations;

                MechanismCSpace mechanismCSpace;
                CSpaceRRT RRT;

                originNode = null;
                destNode = null;
                mechanismCSpace = null;
                RRT = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();

                mechanismCSpace = new MechanismCSpace(robot, enviroment, i);
                RRT = new CSpaceRRT(mechanismCSpace.CSpace, 10000);
                Thread.Sleep(1000);

                start = DateTime.Now;
                iterations = RRT.generatePath(origin, dest, out originNode, out destNode, GrowConnectionType.Edge, false, double.PositiveInfinity, stopEvent);
                end = DateTime.Now;
                elapsedTime = end - start;
                sw.WriteLine("Edge;Infinity;false;" + i.ToString() + ";" + iterations.ToString() + ";" + elapsedTime.TotalSeconds.ToString());
                sw.Flush();

                for (int j = 0; j < 4; j++)
                {
                    double maxSize;
                    switch (j)
                    {
                        case 0:
                            maxSize = 5.0;
                            break;
                        case 1:
                            maxSize = 10.0;
                            break;
                        case 2:
                            maxSize = 25.0;
                            break;
                        case 3:
                            maxSize = 50.0;
                            break;
                        default:
                            maxSize = double.PositiveInfinity;
                            break;
                    }
  
                    
                    originNode = null;
                    destNode = null;
                    mechanismCSpace = null;
                    RRT = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    mechanismCSpace = new MechanismCSpace(robot, enviroment, i);
                    RRT = new CSpaceRRT(mechanismCSpace.CSpace, 10000);
                    Thread.Sleep(1000);

                    start = DateTime.Now;
                    iterations = RRT.generatePath(origin, dest, out originNode, out destNode, GrowConnectionType.Node, false, maxSize, stopEvent);
                    end = DateTime.Now;
                    elapsedTime = end - start;
                    sw.WriteLine("Node;"  + maxSize.ToString()+ ";false;" + i.ToString() + ";" + iterations.ToString() + ";" + elapsedTime.TotalSeconds.ToString());
                    sw.Flush();

                    originNode = null;
                    destNode = null;
                    mechanismCSpace = null;
                    RRT = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    mechanismCSpace = new MechanismCSpace(robot, enviroment, i);
                    RRT = new CSpaceRRT(mechanismCSpace.CSpace, 10000);
                    Thread.Sleep(1000);

                    start = DateTime.Now;
                    iterations = RRT.generatePath(origin, dest, out originNode, out destNode, GrowConnectionType.Node, true, maxSize, stopEvent);
                    end = DateTime.Now;
                    elapsedTime = end - start;
                    sw.WriteLine(";Node;" + maxSize.ToString()+ ";true;" + i.ToString() + ";" + iterations.ToString() + ";" + elapsedTime.TotalSeconds.ToString());
                    sw.Flush();
                }

            }
            sw.Close();
        }




    }
}
