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
using System.Diagnostics;
using Simples.Robotics.Camera;
using Simples.Robotics.Scene;
using Simples.Robotics.Mechanisms;
using Simples.Robotics.Collision;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix _world;
        private OrbitCamera _camera;
        private Model linkModel;

        private static float _STEP = 1;
        
        private Model cube;

        private DrawableN_ArticulatedPlanar2 mechanism;

        private SceneBoxes scene;

        bool reset = true;
        private float oldMouseX, oldMouseY;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
     
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

            _world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            _camera = new OrbitCamera();
            scene = new SceneBoxes(cube, _camera);

            Debug.Assert(linkModel.Bones.Count == 2);
            mechanism = new DrawableN_ArticulatedPlanar2(new Vector3(100, 0, 0), 5, _world, linkModel,
                _camera);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            linkModel = Content.Load<Model>("model1");
            cube = Content.Load<Model>("cube");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
           if (Keyboard.GetState().IsKeyDown(Keys.Escape))
               this.Exit();

            // TODO: Add your update logic here            
            base.Update(gameTime);
            UpdateGamePad();
        }

        private bool isColliding()
        {
            return scene.isColliding(mechanism);
        }

        private void UpdateGamePad()
        {
            KeyboardState state = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            _camera.Zoom = 2500 + mState.ScrollWheelValue;

            if (state.IsKeyDown(Keys.D1))
            {
                mechanism.stepJointAngle(0, _STEP);
            }
            if (state.IsKeyDown(Keys.D2))
            {
                mechanism.stepJointAngle(0, -_STEP);
            }

            if (state.IsKeyDown(Keys.D3))
            {
                mechanism.stepJointAngle(1, _STEP);
            }
            if (state.IsKeyDown(Keys.D4))
            {
                mechanism.stepJointAngle(1, -_STEP);
            }

            if (state.IsKeyDown(Keys.D5))
            {
                mechanism.stepJointAngle(2, _STEP);
            }
            if (state.IsKeyDown(Keys.D6))
            {
                mechanism.stepJointAngle(2, -_STEP);
            }

            if (state.IsKeyDown(Keys.D7))
            {
                mechanism.stepJointAngle(3, _STEP);
            }
            if (state.IsKeyDown(Keys.D8))
            {
                mechanism.stepJointAngle(3, -_STEP);
            }

            if (state.IsKeyDown(Keys.D9))
            {
                mechanism.stepJointAngle(4, _STEP);
            }
            if (state.IsKeyDown(Keys.D0))
            {
                mechanism.stepJointAngle(4, -_STEP);
            }

            if (reset)
            {
                oldMouseX = mState.X;
                oldMouseY = mState.Y;
                reset = false;
            }

            if (mState.MiddleButton == ButtonState.Released)
            {
                reset = true;
            }
            else
            {
                _camera.cameraAngleX += (oldMouseY - mState.Y);
                _camera.cameraAngleY += (oldMouseX - mState.X);

                oldMouseX = mState.X;
                oldMouseY = mState.Y;
            }

        }

        private void DrawModel(Model m, Matrix matrix)
        {

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (isColliding())
            {
                GraphicsDevice.Clear(Color.Yellow);
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
            }

            // TODO: Add your drawing code here
            base.Draw(gameTime);

            scene.Draw(gameTime);
            mechanism.Draw(gameTime);
            Matrix[] transforms = new Matrix[linkModel.Bones.Count];
            linkModel.CopyBoneTransformsTo(transforms);
            /*
            foreach (OrientedBoundingBox Bb in mechanism.BoundingBoxList)
            {
                
                BoundingBoxRenderer.Render(Bb, transforms[1], graphics.GraphicsDevice, _camera.View, _camera.Projection,
                    Color.Red);
                 
            }

            foreach (OrientedBoundingBox Bb in scene.BoundingBoxList)
            {
                
                BoundingBoxRenderer.Render(Bb, transforms[1], graphics.GraphicsDevice, _camera.View, _camera.Projection,
                    Color.Red);
                
            }
            */
        }

    }
}
