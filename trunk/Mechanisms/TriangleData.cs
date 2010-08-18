using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Robotics.Mechanisms
{
    public class TriangleData
    {
         // indices for the three face vertices
        public int[] indices;
     
        public TriangleData(int count, short[] meshIndices, int offset)
        {
            indices = new int[3];
            for (int i = 0; i < 3; i++)
            {
                indices[i] = meshIndices[count + i] + offset;
            }
        }

        public TriangleData(int count, int[] meshIndices, int offset)
        {
            indices = new int[3];
            for (int i = 0; i < 3; i++)
            {
                indices[i] = meshIndices[count + i] + offset;
            }
        }
    }
}
