using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Simples.Robotics.Collision;


namespace Simples.Robotics.Mechanisms
{
    public class N_ArticulatedPlanar2: Mechanism
    {
        private Matrix _world;
      
        private Vector3 _linkTranslation;

        public N_ArticulatedPlanar2(Vector3 linkTranslation, int linkCount, Matrix world)
        {
            this._world = world;
            this._linkTranslation = linkTranslation;

            RevoluteJoint nextJoint = new RevoluteJoint(world, 0, world.Up);
            Joints.Add(nextJoint);
            for (int i = 0; i < linkCount; i++)
            {
                Vector3 min = new Vector3(-10, -5, -10);
                Vector3 max = new Vector3(120, 5, 10);
                OrientedBoundingBox obb = new OrientedBoundingBox(min, max);
                Link link = new Link(nextJoint, obb);
                Links.Add(link);
                if (i != linkCount - 1)
                {
                    nextJoint = new RevoluteJoint(link, _linkTranslation, 0, world.Up);
                    Joints.Add(nextJoint);
                }                
            }
        }


        public void setJointAngle(int index, float angle)
        {
            ((RevoluteJoint)Joints[index]).Angle = angle;
            for (int i = index; i < Joints.Count; i++)
            {
                Joints[i].setPending();
            }
        }

        public void stepJointAngle(int index, float stepAngle)
        {
            ((RevoluteJoint)Joints[index]).Angle += stepAngle;
            for (int i = index; i < Joints.Count; i++)
            {
                Joints[i].setPending();
            }

        }
    }
}
