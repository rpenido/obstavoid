#region File Description
//-----------------------------------------------------------------------------
// MainForm.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Windows.Forms;
using Simples.Scene.Camera;
using Simples.Simulation.Planar2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace _2dPlanarSimulation
{
    // System.Drawing and the XNA Framework both define Color types.
    // To avoid conflicts, we define shortcut names for them both.
    using GdiColor = System.Drawing.Color;
    using XnaColor = Microsoft.Xna.Framework.Graphics.Color;

    
    /// <summary>
    /// Custom form provides the main user interface for the program.
    /// In this sample we used the designer to add a splitter pane to the form,
    /// which contains a SpriteFontControl and a SpinningTriangleControl.
    /// </summary>
    public partial class MainForm : Form
    {
        private OrbitCamera _camera;
        private NArticulatedPlanar _robot;

        private Matrix _world;

        private Game _game;

        public MainForm()
        {
            InitializeComponent();
            _world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            _camera = new OrbitCamera();
            _camera.cameraAngleX = -90f;

            _robot = new NArticulatedPlanar(spinningTriangleControl.Services, new Vector3(100, 0, 0), 2, _world,
                _camera);

        }
    }
}
