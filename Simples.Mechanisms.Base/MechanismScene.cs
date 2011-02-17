using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.Scene;

namespace Simples.Mechanism.Base

{
    public abstract class MechanismScene: Scene
    {
        private ICamera camera;

        private List<OrientedBoundingBox> obstacleList;
        //private List<Matrix> _boxes;
        public Model sceneModel;
        OctreeNode oct;

    }
}
