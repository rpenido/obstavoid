﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Camera
{
    public class OrbitCamera: ICamera
    {

        public float cameraAngleX = 0f;
        public float cameraAngleY = 0f;

        private float zoom = 2500;
        public float Zoom
        {
            get { return zoom; }
            set { zoom = Math.Max(value,50); }
        }

        public void GoHome()
        {
            cameraAngleX = 0f;
            cameraAngleY = 0f;

            zoom = 2500;
        }
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 1, 10000);

        private Matrix getView()
        {
            Vector3 point = new Vector3(0.0f, 0.0f, Zoom);
             
            Vector3 cameraPosition = Vector3.Transform(point,
                Matrix.CreateRotationX(MathHelper.ToRadians(cameraAngleX)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(cameraAngleY)));

            return Matrix.CreateLookAt(cameraPosition,
                Vector3.Zero,
                Math.Sign(Math.Cos(MathHelper.ToRadians(cameraAngleX))) * Vector3.Up);
        }

        private Matrix getProjection()
        {          
            return projection;
        }


        #region ICamera Members

        public Matrix View
        {
            get { return getView(); }
        }

        public Matrix Projection
        {
            get { return getProjection(); }
        }

        #endregion


    }
}
