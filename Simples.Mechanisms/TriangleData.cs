using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Mechanisms
{
    public class VertexList
    {
        //private Vector3[] vertexData;
        private List<Vector3> vertexData;

        public Vector3 this[int index]
        {
            get { return vertexData[index]; }
            set { vertexData[index] = value; }
        }

        /*
        public VertexList(int size)
        {
            this.vertexData = new Vector3[size];
        }
         * */
        public VertexList()
        {
            this.vertexData = new List<Vector3>();
        }
    }

    public class TriangleData
    {
        //private int[] indices;
        private Vector3[] vertexList;
        //private VertexList vertexList;

        public Vector3 this[int index]
        {
            get { return vertexList[index]; }
        }

        private Vector3 normal = Vector3.Zero;
        public Vector3 Normal
        {
            get
            {
                if (normal == Vector3.Zero)
                {
                    normal = Vector3.Cross(this[0], this[1]);
                    normal.Normalize();
                }
                return normal;
            }
        }

        private Vector3 center = Vector3.Zero;
        public Vector3 Center
        {
            get
            {
                if (center == Vector3.Zero)
                {
                    center = Vector3.Barycentric(this[0], this[1], this[2], 1, 1);
                    center = this[0] + this[1] + this[2];
                    center = center/3;
                }
                return center;
            }
        }

        private Vector3[] edges;
        public Vector3[] Edges
        {
            get
            {
                if (edges == null)
                {
                    edges = new Vector3[3];
                    /*
                    edges[0] = this[1] - this[0];
                    edges[1] = this[2] - this[1];
                    edges[2] = this[0] - this[2];
                    */
                    edges[0] = this[0];
                    edges[1] = this[1];
                    edges[2] = this[2];
                }
                return edges;
            }

        }
        /*
        public TriangleData(int count, short[] meshIndices, int offset, VertexList vertexList)
        {
            this.vertexList = vertexList;

            indices = new int[3];
            for (int i = 0; i < 3; i++)
            {
                indices[i] = meshIndices[count + i] + offset;
            }
        }

        public TriangleData(int count, int[] meshIndices, int offset, VertexList vertexList)
        {
            this.vertexList = vertexList;

            indices = new int[3];
            for (int i = 0; i < 3; i++)
            {
                indices[i] = meshIndices[count + i] + offset;
            }
        }
        */
        public TriangleData(ref Vector3[] vertices)
        {
            vertexList = vertices;
        }
    }
}
