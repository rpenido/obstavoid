using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Simulation.Planar2D
{
    public class NArticulatedPlanarController
    {
        private static float VELOCITY = 50.0f;
        private NArticulatedPlanar robot;
        private List<double[]> pointList;
        public bool running = false;
        private bool doNotInterpolate = true;

        public NArticulatedPlanarController(NArticulatedPlanar robot)
        {
            this.robot = robot;
            pointList = new List<double[]>();
        }
        public void AddPoint(double[] p)
        {
            pointList.Add(p);
        }
        public void Clear()
        {
            doNotInterpolate = true;
            pointList.Clear();
        }

        public void Update(GameTime gameTime)
        {
            double maxIncrement = gameTime.ElapsedGameTime.TotalSeconds * VELOCITY;
            if (running)
            {
                if (pointList.Count > 0)
                {
                    if (doNotInterpolate)
                    {
                        for (int i = robot.Mechanism.Joints.Count - 1; i >= 0 ; i--)
                        {
                            robot.Mechanism.Joints[i].Value = pointList[0][i];
                        }
                        pointList.RemoveAt(0);
                        doNotInterpolate = false;
                    }
                    else
                    {
                        double[] delta = new double[robot.Mechanism.Joints.Count];
                        double[] next = new double[robot.Mechanism.Joints.Count];
                        double max = double.NegativeInfinity;
                        int maxIndex = -1;
                        for (int i = 0; i < robot.Mechanism.Joints.Count; i++)
                        {
                            delta[i] = pointList[0][i] - robot.Mechanism.Joints[i].Value;
                            if (Math.Abs(delta[i]) > max)
                            {
                                max = Math.Abs(delta[i]);
                                maxIndex = i;
                            }
                        }
                        

                        double factor;
                        if (Math.Abs(delta[maxIndex]) > maxIncrement)
                        {
                            next[maxIndex] = Math.Sign(delta[maxIndex]) * maxIncrement;
                            factor = next[maxIndex] / delta[maxIndex];
                        }
                        else
                        {
                            next[maxIndex] = delta[maxIndex];
                            factor = 1.0;
                        }



                        for (int i = 0; i < robot.Mechanism.Joints.Count; i++)
                        {
                            next[i] = delta[i] * factor;
                            robot.Mechanism.Joints[i].Value += next[i];
                        }

                        if (factor == 1.0)
                        {
                            pointList.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    running = false;
                }
            }
        }
    }
}
