#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
#endregion
namespace Simples.Mechanisms
{
    [Serializable]
    public class OrientedBoundingBox
    {

        protected Vector3 min;
        protected Vector3 max;
        protected Vector3 center;
        protected Vector3[] extents;
        protected Matrix transforms = Matrix.Identity;
        protected Matrix boxTransform = Matrix.Identity;
        
        [NonSerialized]
        BasicEffect effect;

        public OrientedBoundingBox()
        {
        }

        public OrientedBoundingBox(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;

            UpdateFromMinMax();
        }

        //======================================================================

        public Vector3 Min
        {
            get { return min; }
            set { min = value; UpdateFromMinMax(); }
        }

        public Vector3 Max
        {
            get { return max; }
            set { max = value; UpdateFromMinMax(); }
        }

        public Vector3 Center
        {
            get { return center; }
        }

        public Matrix Transforms
        {
            get { return transforms; }
            set { transforms = value; }
        }

        public Matrix BoxTransform
        {
            get { return boxTransform; }
            set { boxTransform = value; }
        }

        public static double GetExtentProjection(Vector3 axis, Matrix transform, Vector3[] extents)
        {
            Quaternion rotation = Quaternion.CreateFromRotationMatrix(transform);
            /*
            double r0 = Math.Abs(Vector3.Dot(transform.Forward, axis)* extents.Z);
            double r1 = Math.Abs(Vector3.Dot(transform.Left, axis) * extents.X);
            double r2 = Math.Abs(Vector3.Dot(transform.Up, axis) * extents.Y);
            */
            double r0 = Math.Abs(Vector3.Dot(Vector3.Transform(extents[0], rotation), axis));
            double r1 = Math.Abs(Vector3.Dot(Vector3.Transform(extents[1], rotation), axis));
            double r2 = Math.Abs(Vector3.Dot(Vector3.Transform(extents[2], rotation), axis)); 
            /*
            double r = Math.Max(r0, r1);
            r = Math.Max(r, r2);
            return r;
            */
            return r0 + r1 + r2; 
            /*
            Debug.Assert(axis.Length() < 1.1f && axis.Length() > 0.9f);
            return Math.Abs(Vector3.Dot(extents, axis));
            */
        }
        
        public static double GetExtentProjection2(Vector3 axis, Matrix transform, Vector3[] vertices)
        {
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            foreach (Vector3 vert in vertices)
            {
                double proj = Math.Abs(Vector3.Dot(vert, axis));
                min = Math.Min(min, proj);
                max = Math.Max(max, proj);
            }
            return (max - min)/2;
            /*
            Quaternion rotation = Quaternion.CreateFromRotationMatrix(transform);

            double r0 = Math.Abs(Vector3.Dot(Vector3.Transform(edges[0], rotation), axis));
            double r1 = Math.Abs(Vector3.Dot(Vector3.Transform(edges[1], rotation), axis));
            double r2 = Math.Abs(Vector3.Dot(Vector3.Transform(edges[2], rotation), axis));             

            double r = Math.Max(r0, r1);
            r = Math.Max(r, r2);
            return r;*/
        }
        
        public static bool IsSeparationAxis(Vector3 axis, Vector3 distance, Vector3[] extents, Matrix transform,
            Vector3[] otherExtents, Matrix otherTransform)
        {
            if (axis.Length() <= 0.1f)
            {
                return false;
            }
            axis.Normalize();

            double r, r1, r2;
            r = Math.Abs(Vector3.Dot(distance, axis));
            r1 = GetExtentProjection(axis, transform, extents);
            r2 = GetExtentProjection(axis, otherTransform, otherExtents);
            
            return (r > r1 + r2);
                
        }
        
        public static bool IsSeparationAxis2(Vector3 axis, Vector3 distance, Vector3[] extents, Matrix transform,
                    Vector3[] edges, Matrix otherTransform)
        {
            if (axis == Vector3.Zero)
            {
                return false;
            }
            axis.Normalize();

            double r, r1, r2;
            r = Math.Abs(Vector3.Dot(distance, axis));
            r1 = GetExtentProjection(axis, transform, extents);
            r2 = GetExtentProjection2(axis, otherTransform, edges);

            return (r > r1 + r2);

        }
        
        public bool Intersects(OrientedBoundingBox other)
        {
            //Matrix toMe = other.Transforms * Matrix.Invert(Transforms);Matrix toMe = other.Transforms * Matrix.Invert(Transforms);
            Matrix m = Transforms;
            Matrix otherM = other.Transforms;
            //Vector3 scale, translation;
            //Quaternion rotation;
            //toMe.Decompose(out scale, out rotation, out translation);
            //
            Vector3[] extentsOther = other.extents;
            Vector3 myCenter = Vector3.Transform(center, m);
            Vector3 centerOther = Vector3.Transform(other.Center, otherM);

            Vector3 distance = centerOther - myCenter;
            
            /******************* Old Code ***************************
            Matrix3 rotations = new Matrix3(toMe);
            Matrix3 absRotations = Utility.Abs(rotations);

           
            
            
            float r, r0, r1, r01;
            //--- Test case 1 - X axis            

            r = Math.Abs(distance.X);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(0));
            r01 = extents.X + r1;

            if (r > r01)
                return false;

            //--- Test case 1 - Y axis
            r = Math.Abs(distance.Y);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(1));
            r01 = extents.Y + r1;

            if (r > r01)
                return false;

            //--- Test case 1 - Z axis
            r = Math.Abs(distance.Z);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(2));
            r01 = extents.Z + r1;

            if (r > r01)
                return false;
            

         
            
            //--- Test case 2 - X axis            
            r = Math.Abs(Vector3.Dot(rotations.Row(0), distance));
            r0 = Vector3.Dot(extents, absRotations.Row(0));
            r01 = r0 + extentsOther.X;

            if (r > r01)
                return false;
                        
            //--- Test case 2 - Y axis
            r = Math.Abs(Vector3.Dot(rotations.Row(1), distance));
            r0 = Vector3.Dot(extents, absRotations.Row(1));
            r01 = r0 + extentsOther.Y;

            if (r > r01)
                return false;
            
            
            //--- Test case 2 - Z axis
            r = Math.Abs(Vector3.Dot(rotations.Row(2), distance));
            r0 = Vector3.Dot(extents, absRotations.Row(2));
            r01 = r0 + extentsOther.Z;

            if (r > r01)
                return false;
            
            //--- Test case 3 # 1            
            r = Math.Abs(distance.Z * rotations[0, 1] - distance.Y * rotations[0, 2]);
            r0 = extents.Y * absRotations[0, 2] + extents.Z * absRotations[0, 1];
            r1 = extentsOther.Y * absRotations[2, 0] + extentsOther.Z * absRotations[1, 0];
            r01 = r0 + r1;

            if (r > r01)
                return false;
            
            
            //--- Test case 3 # 2
            r = Math.Abs(distance.Z * rotations[1, 1] - distance.Y * rotations[1, 2]);
            r0 = extents.Y * absRotations[1, 2] + extents.Z * absRotations[1, 1];
            r1 = extentsOther.X * absRotations[2, 0] + extentsOther.Z * absRotations[0, 0];
            r01 = r0 + r1;

            if (r > r01)
                return false;

            //--- Test case 3 # 3
            r = Math.Abs(distance.Z * rotations[2, 1] - distance.Y * rotations[2, 2]);
            r0 = extents.Y * absRotations[2, 2] + extents.Z * absRotations[2, 1];
            r1 = extentsOther.X * absRotations[1, 0] + extentsOther.Y * absRotations[0, 0];
            r01 = r0 + r1;

            if (r > r01)
                return false;

            //--- Test case 3 # 4
            r = Math.Abs(distance.X * rotations[0, 2] - distance.Z * rotations[0, 0]);
            r0 = extents.X * absRotations[0, 2] + extents.Z * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 1] + extentsOther.Z * absRotations[1, 1];
            r01 = r0 + r1;

            if (r > r01)
                return false;

            //--- Test case 3 # 5
            r = Math.Abs(distance.X * rotations[1, 2] - distance.Z * rotations[1, 0]);
            r0 = extents.X * absRotations[1, 2] + extents.Z * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 1] + extentsOther.Z * absRotations[0, 1];
            r01 = r0 + r1;

            if (r > r01)
                return false;

            //--- Test case 3 # 6
            r = Math.Abs(distance.X * rotations[2, 2] - distance.Z * rotations[2, 0]);
            r0 = extents.X * absRotations[2, 2] + extents.Z * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 1] + extentsOther.Y * absRotations[0, 1];
            r01 = r0 + r1;

            if (r > r01)
                return false;

            //--- Test case 3 # 7
            r = Math.Abs(distance.Y * rotations[0, 0] - distance.X * rotations[0, 1]);
            r0 = extents.X * absRotations[0, 1] + extents.Y * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 2] + extentsOther.Z * absRotations[1, 2];
            r01 = r0 + r1;

            if (r > r01)
                return false;

            //--- Test case 3 # 8
            r = Math.Abs(distance.Y * rotations[1, 0] - distance.X * rotations[1, 1]);
            r0 = extents.X * absRotations[1, 1] + extents.Y * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 2] + extentsOther.Z * absRotations[0, 2];
            r01 = r0 + r1;

            if (r > r01)
                return false;

            //--- Test case 3 # 9
            r = Math.Abs(distance.Y * rotations[2, 0] - distance.X * rotations[2, 1]);
            r0 = extents.X * absRotations[2, 1] + extents.Y * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 2] + extentsOther.Y * absRotations[0, 2];
            r01 = r0 + r1;

            if (r > r01)
                return false;
            ********************* End Old Code ***************************/
            
            /*
            if (IsSeparationAxis(transforms.Forward, distance, extents, transforms, extentsOther, toMe))
                return false;

            if (IsSeparationAxis(transforms.Left, distance, extents, transforms, extentsOther, toMe))
                return false;

            if (IsSeparationAxis(transforms.Up, distance, extents, transforms, extentsOther, toMe))
                return false;

            if (IsSeparationAxis(toMe.Forward, distance, extents, transforms, extentsOther, toMe))
                return false;

            if (IsSeparationAxis(toMe.Left, distance, extents, transforms, extentsOther, toMe))
                return false;

            if (IsSeparationAxis(toMe.Up, distance, extents, transforms, extentsOther, toMe))
                return false;

            if (IsSeparationAxis(Vector3.Cross(transforms.Forward, toMe.Forward), distance, extents, transforms, extentsOther, toMe))
                return false;
            if (IsSeparationAxis(Vector3.Cross(transforms.Forward, toMe.Left), distance, extents, transforms, extentsOther, toMe))
                return false;
            if (IsSeparationAxis(Vector3.Cross(transforms.Forward, toMe.Up), distance, extents, transforms, extentsOther, toMe))
                return false;
            if (IsSeparationAxis(Vector3.Cross(transforms.Left, toMe.Forward), distance, extents, transforms, extentsOther, toMe))
                return false;
            if (IsSeparationAxis(Vector3.Cross(transforms.Left, toMe.Left), distance, extents, transforms, extentsOther, toMe))
                return false;
            if (IsSeparationAxis(Vector3.Cross(transforms.Left, toMe.Up), distance, extents, transforms, extentsOther, toMe))
                return false;
            if (IsSeparationAxis(Vector3.Cross(transforms.Up, toMe.Forward), distance, extents, transforms, extentsOther, toMe))
                return false;
            if (IsSeparationAxis(Vector3.Cross(transforms.Up, toMe.Left), distance, extents, transforms, extentsOther, toMe))
                return false;
            if (IsSeparationAxis(Vector3.Cross(transforms.Up, toMe.Up), distance, extents, transforms, extentsOther, toMe))
                return false;
            */

            if (IsSeparationAxis(m.Forward, distance, extents, transforms, extentsOther, otherM))
                return false;

            if (IsSeparationAxis(m.Left, distance, extents, transforms, extentsOther, otherM))
                return false;

            if (IsSeparationAxis(m.Up, distance, extents, transforms, extentsOther, otherM))
                return false;

            if (IsSeparationAxis(otherM.Forward, distance, extents, transforms, extentsOther, otherM))
                return false;

            if (IsSeparationAxis(otherM.Left, distance, extents, transforms, extentsOther, otherM))
                return false;

            if (IsSeparationAxis(otherM.Up, distance, extents, transforms, extentsOther, otherM))
                return false;

            if (IsSeparationAxis(Vector3.Cross(m.Forward, otherM.Forward), distance, extents, transforms, extentsOther, otherM))
                return false;
            if (IsSeparationAxis(Vector3.Cross(m.Forward, otherM.Left), distance, extents, transforms, extentsOther, otherM))
                return false;
            if (IsSeparationAxis(Vector3.Cross(m.Forward, otherM.Up), distance, extents, transforms, extentsOther, otherM))
                return false;
            if (IsSeparationAxis(Vector3.Cross(m.Left, otherM.Forward), distance, extents, transforms, extentsOther, otherM))
                return false;
            if (IsSeparationAxis(Vector3.Cross(m.Left, otherM.Left), distance, extents, transforms, extentsOther, otherM))
                return false;
            if (IsSeparationAxis(Vector3.Cross(m.Left, otherM.Up), distance, extents, transforms, extentsOther, otherM))
                return false;
            if (IsSeparationAxis(Vector3.Cross(m.Up, otherM.Forward), distance, extents, transforms, extentsOther, otherM))
                return false;
            if (IsSeparationAxis(Vector3.Cross(m.Up, otherM.Left), distance, extents, transforms, extentsOther, otherM))
                return false;
            if (IsSeparationAxis(Vector3.Cross(m.Up, otherM.Up), distance, extents, transforms, extentsOther, otherM))
                return false;
            return true;
        }

        public bool Intersects(TriangleData other)
        {
            Matrix m = Transforms;
            Matrix otherM = Matrix.Identity;
            //Vector3 scale, translation;
            //Quaternion rotation;
            //toMe.Decompose(out scale, out rotation, out translation);
            //
            Vector3[] extentsOther = other.Edges;
            Vector3 myCenter = Vector3.Transform(center, m);
            Vector3 centerOther = Vector3.Transform(other.Center, otherM);

            Vector3 distance = centerOther - myCenter;
            /*
             * 
box vs tri axes
---------------
tri.normal
box.dir0
box.dir1
box.dir2

box.dir0 x tri.edge0
box.dir0 x tri.edge1
box.dir0 x tri.edge2

box.dir1 x tri.edge0
box.dir1 x tri.edge1
box.dir1 x tri.edge2

box.dir2 x tri.edge0
box.dir2 x tri.edge1
box.dir2 x tri.edge2*/

            if (IsSeparationAxis2(other.Normal, distance, extents, m, other.Edges, otherM))
                return false;

            if (IsSeparationAxis2(m.Forward, distance, extents, m, other.Edges, otherM))
                return false;

            if (IsSeparationAxis2(m.Left, distance, extents, m, other.Edges, otherM))
                return false;

            if (IsSeparationAxis2(m.Up, distance, extents, m, other.Edges, otherM))
                return false;

            if (IsSeparationAxis2(Vector3.Cross(m.Forward, other.Edges[0]), distance, extents, transforms, other.Edges, otherM))
                return false;
            if (IsSeparationAxis2(Vector3.Cross(m.Forward, other.Edges[1]), distance, extents, transforms, other.Edges, otherM))
                return false;
            if (IsSeparationAxis2(Vector3.Cross(m.Forward, other.Edges[2]), distance, extents, transforms, other.Edges, otherM))
                return false;

            if (IsSeparationAxis2(Vector3.Cross(m.Left, other.Edges[0]), distance, extents, transforms, other.Edges, otherM))
                return false;
            if (IsSeparationAxis2(Vector3.Cross(m.Left, other.Edges[1]), distance, extents, transforms, other.Edges, otherM))
                return false;
            if (IsSeparationAxis2(Vector3.Cross(m.Left, other.Edges[2]), distance, extents, transforms, other.Edges, otherM))
                return false;

            if (IsSeparationAxis2(Vector3.Cross(m.Up, other.Edges[0]), distance, extents, transforms, other.Edges, otherM))
                return false;
            if (IsSeparationAxis2(Vector3.Cross(m.Up, other.Edges[1]), distance, extents, transforms, other.Edges, otherM))
                return false;
            if (IsSeparationAxis2(Vector3.Cross(m.Up, other.Edges[2]), distance, extents, transforms, other.Edges, otherM))
                return false;

             
            /*
             * tri vs tri axes
---------------
tri0.normal
tri1.normal

tri0.edge0 x tri1.edge0
tri0.edge0 x tri1.edge1
tri0.edge0 x tri1.edge2

tri0.edge1 x tri1.edge0
tri0.edge1 x tri1.edge1
tri0.edge1 x tri1.edge2

tri0.edge2 x tri1.edge0
tri0.edge2 x tri1.edge1
tri0.edge2 x tri1.edge2


box vs tri axes
---------------
tri.normal
box.dir0
box.dir1
box.dir2

box.dir0 x tri.edge0
box.dir0 x tri.edge1
box.dir0 x tri.edge2

box.dir1 x tri.edge0
box.dir1 x tri.edge1
box.dir1 x tri.edge2

box.dir2 x tri.edge0
box.dir2 x tri.edge1
box.dir2 x tri.edge2

             */
            return true;
        }
        //======================================================================

        protected void UpdateFromMinMax()
        {
            center = (min + max) * 0.5f;
            Vector3 ext = (max - min) * 0.5f;
            extents = new Vector3[3];
            extents[0] = new Vector3(ext.X, 0, 0);
            extents[1] = new Vector3(0, ext.Y, 0);
            extents[2] = new Vector3(0, 0, ext.Z);

        }

        //======================================================================
        public void Draw(GraphicsDevice graphicsDevice, Matrix projection, Matrix view)
        {
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

                effect.VertexColorEnabled = true;
                //effect.LightingEnabled = true;

            }
            
            effect.World = transforms;
            effect.View = view;
            effect.Projection = projection;
            VertexPositionColor[] edges = new VertexPositionColor[24];




            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {

                p.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                                 PrimitiveType.LineList,
                                 corners,
                                 0,
                                 8,
                                 indices,
                                 0,
                                 indices.Length / 2);
            }
        }

    }
/*
    class Utility
    {
        public static Vector3 Multiply(Vector3 v, Matrix m)
        {
            return
                new Vector3(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31 + m.M41,
                            v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32 + m.M42,
                            v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33 + m.M43);
        }

        public static Matrix3 Abs(Matrix3 m3)
        {
            Matrix3 absMatrix = new Matrix3(0);

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    absMatrix[r, c] = Math.Abs(m3[r, c]);
                }
            }

            return absMatrix;
        }

    }

    struct Matrix3
    {
        public Matrix3(float f)
        {
            this.Elements = new float[3, 3];
        }

        public Matrix3(Matrix m)
        {
            this.Elements = new float[3, 3];

            this.Elements[0, 0] = m.M11;
            this.Elements[0, 1] = m.M12;
            this.Elements[0, 2] = m.M13;

            this.Elements[1, 0] = m.M21;
            this.Elements[1, 1] = m.M22;
            this.Elements[1, 2] = m.M23;

            this.Elements[2, 0] = m.M31;
            this.Elements[2, 1] = m.M32;
            this.Elements[2, 2] = m.M33;
        }

        public Matrix3(Matrix3 m)
        {
            this.Elements = new float[3, 3];

            this.Elements[0, 0] = m[0, 0];
            this.Elements[0, 1] = m[0, 1];
            this.Elements[0, 2] = m[0, 2];

            this.Elements[1, 0] = m[1, 0];
            this.Elements[1, 1] = m[1, 1];
            this.Elements[1, 2] = m[1, 2];

            this.Elements[2, 0] = m[2, 0];
            this.Elements[2, 1] = m[2, 1];
            this.Elements[2, 2] = m[2, 2];
        }

        public float this[int row, int column]
        {
            get
            {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                return this.Elements[row, column];
            }
            set
            {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                this.Elements[row, column] = value;
            }
        }


        public Vector3 Row(int row)
        {
            return new Vector3(this.Elements[row, 0],
                               this.Elements[row, 1],
                               this.Elements[row, 2]);
        }

        public Vector3 Column(int column)
        {
            return new Vector3(this.Elements[0, column],
                               this.Elements[1, column],
                               this.Elements[2, column]);
        }

        float[,] Elements;
    }
*/
}
