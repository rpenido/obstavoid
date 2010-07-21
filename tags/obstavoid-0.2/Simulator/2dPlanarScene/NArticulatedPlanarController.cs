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
        private List<int[]> pointList;
        public bool running = false;

        public NArticulatedPlanarController(NArticulatedPlanar robot)
        {
            this.robot = robot;
            pointList = new List<int[]>();
        }
        public void AddPoint(int[] p)
        {
            pointList.Add(p);
        }
        public void Clear()
        {
            pointList.Clear();
        }

        public void Update(GameTime gameTime)
        {
            float maxIncrement = (float)gameTime.ElapsedGameTime.TotalSeconds * VELOCITY;
            if (running)
            {
                if (pointList.Count > 0)
                {
                    float delta1 = pointList[0][0] - robot.Mechanism.Joints[0].Value;
                    float delta2 = pointList[0][1] - robot.Mechanism.Joints[1].Value;
                    float next1;
                    float next2;

                    float factor;
                    if (Math.Abs(delta1) > Math.Abs(delta2))
                    {
                        next1 = Math.Sign(delta1)* Math.Min(maxIncrement, Math.Abs(delta1));
                        factor = next1 / delta1;
                        next2 = delta2 * factor;
                    }
                    else
                    {
                        next2 = Math.Sign(delta2) * Math.Min(maxIncrement, Math.Abs(delta2));
                        factor = next2 / delta2;
                        next1 = delta1 * factor;
                    }

                    robot.Mechanism.Joints[0].Value += next1;
                    robot.Mechanism.Joints[1].Value += next2;

                    if ((Math.Abs(robot.Mechanism.Joints[0].Value - pointList[0][0]) < 0.1f) &&
                        (Math.Abs(robot.Mechanism.Joints[1].Value - pointList[0][1]) < 0.1f))
                    {
                        robot.Mechanism.Joints[1].Value = pointList[0][1];
                        robot.Mechanism.Joints[0].Value = pointList[0][0];
                        

                        pointList.RemoveAt(0);
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
