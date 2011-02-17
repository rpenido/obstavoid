using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Scene.Camera
{
    public interface ICamera
    {
        Matrix View
        {
            get;
        }
        Matrix Projection
        {
            get;
        }
    }
}
