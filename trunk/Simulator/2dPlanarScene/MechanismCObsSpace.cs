using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.Robotics.Mechanisms;
using Simples.SampledBased;


namespace Simples.Simulation.Planar2D
{
    public class MechanismCObsSpace : CObsSpace
    {
        Mechanism mechanism;
        SceneBoxes scene;

        public MechanismCObsSpace(Mechanism mechanism, SceneBoxes scene)
            :base(mechanism.Joints.Count, new double[mechanism.Joints.Count])
        {
            this.mechanism = mechanism;
            this.scene = scene;

            for (int i = 0; i < dimensionCount; i++)
            {
                dimensionSize[i] = 359;
            }
        }

        public override bool CheckCollision(double[] p)
        {
            base.CheckCollision(p);
            for (int i = 0; i < p.Length; i++)
            {
                mechanism.Joints[i].Value = p[i];
            }
            return scene.isColliding(mechanism);
        }

        public override object Clone()
        {
            Mechanism cloneMechanism = (Mechanism)mechanism.Clone();
            MechanismCObsSpace cObsSpace = new MechanismCObsSpace(cloneMechanism, scene);
            return cObsSpace;
        }

    }
}
