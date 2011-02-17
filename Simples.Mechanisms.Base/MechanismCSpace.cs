using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.Mechanisms;
using Simples.Scene;
using Simples.SampleBased;
using Simples.SampleBased.RRT;


namespace Simples.Mechanisms.Base
{
    public class MechanismCSpace: ICloneable
    {
        CSpace cSpace;
        Mechanism mechanism;
        MechanismScene scene;

        public MechanismCSpace(Mechanisms mechanism, SceneBoxes scene)
        {
            this.mechanism = mechanism;
            this.scene = scene;


            double[] dimensionLowLimit = new double[mechanism.Joints.Count];
            double[] dimensionHighLimit = new double[mechanism.Joints.Count];
            double[] dimensionWeight = new double[mechanism.Joints.Count];
            for (int i = 0; i < mechanism.Joints.Count; i++)
            {
                dimensionLowLimit[i] = -180;
                dimensionHighLimit[i] = 180;
                dimensionWeight[i] = 1;

            }

            this.cSpace = new CSpace(mechanism.Joints.Count, dimensionLowLimit, dimensionHighLimit, dimensionWeight, CheckCollision);
        }

        public bool CheckCollision(double[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                mechanism.Joints[i].Value = p[i];
            }
            return scene.isColliding(mechanism);
        }

        public object Clone()
        {
            Mechanism cloneMechanism = (Mechanism)mechanism.Clone();
            MechanismCSpace cObsSpace = new MechanismCSpace(cloneMechanism, scene);

            return cObsSpace;
        }

    }
}
