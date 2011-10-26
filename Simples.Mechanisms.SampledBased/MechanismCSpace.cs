using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.Mechanisms;
using Simples.Camera;
using Simples.PathPlan.SampleBased;
using Simples.PathPlan.SampleBased.RRT;


namespace Simples.Mechanisms.SampleBased
{
    public class MechanismCSpace
    {
        private CSpace cSpace;
        private Mechanism mechanism;
        private MechanismEnviroment enviroment;

        public CSpace CSpace
        {
            get { return cSpace; }
        }

        public MechanismCSpace(Mechanism mechanism, MechanismEnviroment scene, int randomSeed)
        {
            this.mechanism = (Mechanism)mechanism.Clone();
            this.enviroment = scene;


            double[] dimensionLowLimit = new double[mechanism.Joints.Count];
            double[] dimensionHighLimit = new double[mechanism.Joints.Count];
            double[] dimensionWeight = new double[mechanism.Joints.Count];
            for (int i = 0; i < mechanism.Joints.Count; i++)
            {
                dimensionLowLimit[i] = -180;
                dimensionHighLimit[i] = 180;
                dimensionWeight[i] = 1;

            }

            this.cSpace = new CSpace(mechanism.Joints.Count, dimensionLowLimit, dimensionHighLimit, dimensionWeight, CheckCollision, randomSeed);
        }

        public bool CheckCollision(double[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                mechanism.Joints[i].Value = p[i];
            }
            return enviroment.IsColliding(mechanism);
        }
    }
}
