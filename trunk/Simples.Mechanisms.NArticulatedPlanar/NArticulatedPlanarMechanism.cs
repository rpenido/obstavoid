using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Runtime.Serialization;
using Simples.Mechanisms;


namespace Simples.Mechanisms.NArticulatedPlanar
{
    public static class NArticulatedPlanarMechanism
    {
        public static Mechanism Create(Model linkModel, Vector3 linkTranslation, Vector3 boundboxMin, Vector3 boundboxMax, int linkCount, Matrix world)
        {
            Mechanism mechanism = new Mechanism();
                
            Link baseLink = new Link(world);
            RevoluteJoint nextJoint = new RevoluteJoint(baseLink, Vector3.Zero, 0.0f, Vector3.UnitZ, -180, 180, 1.0);
            mechanism.Joints.Add(nextJoint);
            for (int i = 0; i < linkCount; i++)
            {
                Link link = new Link(nextJoint);
                link.Model = linkModel;
                OrientedBoundingBox obb = new OrientedBoundingBox(boundboxMin, boundboxMax);
                link.AddBoundingBox(obb);
                mechanism.Links.Add(link);
                if (i != linkCount - 1)
                {
                    nextJoint = new RevoluteJoint(link, linkTranslation, 0, Vector3.UnitZ, -180, +180, i);
                    mechanism.Joints.Add(nextJoint);
                }                
            }

            return mechanism;
        }



    }
}
