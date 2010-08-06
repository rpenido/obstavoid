using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Simples.Robotics.Mechanisms
{
    enum OctreePosition { FLB=0, FLT, FRB, FRT, BLB, BLT, BRB, BRT };

    public class OctreeNode
    {
        private const int maxLevel = 1;
        private OctreeNode[] octree;
        private List<Vector3> vertices;
        private Vector3 min;
        private Vector3 max;
        private int level;

        public void AddVertice(Vector3 vertice)
        {
            vertices.Add(vertice);
        }

        public OctreeNode(Vector3 min, Vector3 max, int level)
        {
            this.min = min;
            this.max = max;
            vertices = new List<Vector3>();
        }

        public void Divide()
        {
            /*
            if (vertices.Count == 0)
            {
                return;
            }
            */
            octree = new OctreeNode[8];
            Vector3 step = (max - min)/2;
            int quadIndex = 0;
            for (float x = min.X; x < max.X; x += step.X)
            {
                for (float y = min.Y; y < max.Y; y += step.Y)
                {
                    for (float z = min.Z; z < max.Z; z += step.Z)
                    {
                        Vector3 childMin2 = new Vector3(x, y, z);
                        Vector3 childMax2 = childMin2 + step;
                        octree[quadIndex] = new OctreeNode(
                            childMin2,
                            childMax2,
                            level + 1);
                        quadIndex++;
                    }
                }
            }
            //
            Vector3 childMin;
            Vector3 childMax;
            childMin = min;
            childMax = (max - min) / 2;
            octree[(int)OctreePosition.BLB] = new OctreeNode(
                childMin,
                childMax,
                level + 1);

            childMin = min;
            childMax = (max - min) / 2;
            childMin.Z = (max.Z - min.Z) / 2;
            childMax.Z = max.Z;
            octree[(int)OctreePosition.BLT] = new OctreeNode(
                childMin,
                childMax,
                level + 1);

            childMin = min;
            childMax = (max - min) / 2;
            childMin.Y = (max.Y - min.Y) / 2;
            childMax.Y = max.Y;
            octree[(int)OctreePosition.BRB] = new OctreeNode(
                childMin,
                childMax,
                level + 1);

            childMin = (max - min) / 2;
            childMax = max;
            childMin.X = min.X;
            childMax.X = (max.X - min.X) / 2;
            octree[(int)OctreePosition.BRT] = new OctreeNode(
                childMin,
                childMax,
                level + 1);

            childMin = min;
            childMax = (max - min) / 2;
            childMin.X = (max.X - min.X) / 2;
            childMax.X = max.X;
            octree[(int)OctreePosition.FLB] = new OctreeNode(
                childMin,
                childMax,
                level + 1);

            childMin = (max - min) / 2;
            childMax = max;
            childMin.Y = min.Y;
            childMax.Y = (max.Y - min.Y) / 2;
            octree[(int)OctreePosition.FLT] = new OctreeNode(
                childMin,
                childMax,
                level + 1);

            childMin = (max - min) / 2;
            childMax = max;
            childMin.Z = min.Z;
            childMax.Z = (max.Z - min.Z) / 2;
            octree[(int)OctreePosition.FRB] = new OctreeNode(
                childMin,
                childMax,
                level + 1);

            childMin = (max - min) / 2;
            childMax = max;
            octree[(int)OctreePosition.FRT] = new OctreeNode(
                childMin,
                childMax,
                level + 1);



            Vector3 mid = (max + min) / 2;
            foreach (Vector3 vert in vertices)
            {
                Vector3 pos = vert - mid;
                bool[] bits = new bool[] { pos.X > 0, pos.Y > 0, pos.Z > 0 };
                quadIndex = (Convert.ToByte(bits[0]) << 2) +
                    (Convert.ToByte(bits[1]) << 1) +
                    Convert.ToByte(bits[2]);

                octree[quadIndex].AddVertice(vert);
            }

            if (level < maxLevel)
            {
                foreach (OctreeNode oct in octree)
                {
                    oct.Divide();
                }
            }
        }


    }
}
