using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Simples.Robotics.Mechanisms
{
    [Serializable]
    public class Mechanism: ICloneable
    {
        
        private List<Joint> jointList;
        
        public List<Joint> Joints
        {
            get { return jointList; }
        }
         
        private List<Link> linkList;
        
        public List<Link> Links
        {
            get { return linkList; }
        }

        public Mechanism()
        {
            jointList = new List<Joint>();
            linkList = new List<Link>();
        }

        #region ICloneable Members

        public object Clone()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            ms.Seek(0, SeekOrigin.Begin);
            return bf.Deserialize(ms);               
        }

        #endregion
    }
}
