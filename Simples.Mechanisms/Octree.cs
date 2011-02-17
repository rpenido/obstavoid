using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace Simples.Mechanisms
{
    enum OctreePosition { FLB=0, FLT, FRB, FRT, BLB, BLT, BRB, BRT };

    public class OctreeNode
    {  
        private const int maxLevel = 2;
        private OctreeNode[] octree;
        private OrientedBoundingBox obb;
        public List<TriangleData> triangles;
        private Vector3 min;
        private Vector3 max;
        private int level;
        public VertexList vertexList;

        BasicEffect effect;

        public void AddTriangle(TriangleData triangle)
        {
            if (!triangles.Contains(triangle))
            {
                triangles.Add(triangle);
            }
        }

        public OctreeNode(Vector3 min, Vector3 max, int level)
        {
            this.min = min;
            this.max = max;
            this.level = level;
            this.obb = new OrientedBoundingBox(min, max);
            triangles = new List<TriangleData>();
        }

        public bool IsColliding(Link link)
        {
            foreach (TriangleData tri in triangles)
            {
                if (link.Intersects(tri))
                {
                    return true;
                }
            }
            return false;
         
            /*
            if (link.Intersects(obb))
            {
                if (octree != null)
                {
                    foreach (OctreeNode node in octree)
                    {
                        if (node.IsColliding(link))
                        {
                            return true;
                        }
                    }
                }
                else if (triangles.Count > 0)
                {
                    return true;
                }
                

            }
            return false;
            */
        }
        public bool IsColliding(Mechanism mechanism)
        {
            foreach (Link link in mechanism.Links)
            {
                if (IsColliding(link))
                {
                    return true;
                }
            }
            return false;
        }

        public void Divide()
        {
            if (triangles.Count < 5)
                return;
          
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
            
            Vector3 mid = (max + min) / 2;
            foreach (TriangleData triangle in triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 pos = triangle[i] - mid;
                    bool[] bits = new bool[] { pos.X > 0, pos.Y > 0, pos.Z > 0 };
                    quadIndex = (Convert.ToByte(bits[0]) << 2) +
                        (Convert.ToByte(bits[1]) << 1) +
                        Convert.ToByte(bits[2]);
                        octree[quadIndex].AddTriangle(triangle);
                    
                }
            }

            if (level < maxLevel)
            {
                foreach (OctreeNode oct in octree)
                {
                    oct.Divide();
                }
            }
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix projection, Matrix view)
        {
            if (triangles.Count == 0)
                return;

            int[] indices = new int[]
            {
                0, 1,
                0, 2,
                0, 3,
                1, 6,
                1, 5,
                2, 4,
                2, 5,
                3, 6,
                3, 4,
                4, 7,
                6, 7,
                5, 7
            };

            VertexPositionColor[] corners = new VertexPositionColor[8];
            corners[0] = new VertexPositionColor(min, Color.Beige);
            corners[1] = new VertexPositionColor(new Vector3(max.X, min.Y, min.Z), Color.Red);
            corners[2] = new VertexPositionColor(new Vector3(min.X, max.Y, min.Z), Color.Red);
            corners[3] = new VertexPositionColor(new Vector3(min.X, min.Y, max.Z), Color.Red);

            corners[4] = new VertexPositionColor(new Vector3(min.X, max.Y, max.Z), Color.Red);
            corners[5] = new VertexPositionColor(new Vector3(max.X, max.Y, min.Z), Color.Red);

            corners[6] = new VertexPositionColor(new Vector3(max.X, min.Y, max.Z), Color.Red);

            corners[7] = new VertexPositionColor(max, Color.Red);

            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice);

                effect.World = Matrix.Identity;

                effect.VertexColorEnabled = true;
               
            }
            effect.View = view;
            effect.Projection = projection;
            VertexPositionColor[] edges = new VertexPositionColor[24];
           
            
            
            //effect.Begin();
            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {
                //p.Begin();
                p.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                                 PrimitiveType.LineList,
                                 corners,
                                 0,
                                 8,
                                 indices,
                                 0,
                                 indices.Length / 2);                    
                
                //p.End();
            }
            //effect.End();
            if (octree != null)
            {
              
                foreach (OctreeNode oct in octree)
                {
                    oct.Draw(graphicsDevice, projection, view);
                }
            }

        }
        

    }
}
