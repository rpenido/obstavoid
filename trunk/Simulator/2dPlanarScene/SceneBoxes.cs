using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simples.Scene.Camera;
using Simples.Robotics.Mechanisms;
using System.Runtime.InteropServices;

namespace Simples.Simulation.Planar2D
{

    public class SceneBoxes: DrawableGameComponent
    {
        private ICamera _camera;

        private List<OrientedBoundingBox> obstacleList;
        private List<Matrix> _boxes;
        public Model _boxModel;
        

        public SceneBoxes(Game game, ICamera camera)
            :base(game)
        {
            
            this._boxModel = game.Content.Load<Model>("scene"); ;
            this._camera = camera;

            obstacleList = new List<OrientedBoundingBox>();
            _boxes = new List<Matrix>();
            /*
            createBox(180, 0, 10);
            createBox(100, 0, 10);
            createBox(150, 0, -150);
            createBox(-100, 0, 120);
            createBox(-200, 0, -100);
            */
            
            createBox(300, 100, 0);
            createBox(200, 10, 0);
            createBox(250, -300, 0);
            createBox(-100, 120, 0);
            createBox(-400, 100, 0);

            createBox(100, 100, 0);
            Initialize();
        }

        public List<OrientedBoundingBox> BoundingBoxList
        {
            get { return obstacleList; }
        }

        private void createBox(float X, float Y, float Z)
        {
            Vector3 origin = new Vector3(X, Y, Z);
            
            Matrix matrix = Matrix.CreateWorld(origin, Vector3.Forward, Vector3.Up);
            _boxes.Add(matrix);
            
            Vector3 min = new Vector3(0, 0, 0);
            Vector3 max = new Vector3(50, 50, 50);

            OrientedBoundingBox bb = new OrientedBoundingBox(min, max);
            bb.Transforms = matrix;
            obstacleList.Add(bb);
        }

        public bool isColliding(Mechanism mechanism)
        {
            foreach (Link link in mechanism.Links)
            {
                foreach (OrientedBoundingBox sceneBb in obstacleList)
                {
                    if (link.Intersects(sceneBb))
                    {
                        return true;
                    }
                }
            }
            return false;              
        }

        public override void Draw(GameTime gameTime)
        {
            
            Matrix[] transforms = new Matrix[_boxModel.Bones.Count];
            _boxModel.CopyBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in _boxModel.Meshes)
            {
                
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = _camera.View;

                    effect.Projection = _camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index];
                    mesh.Draw();
                }
                
            }
             
          
            foreach (OrientedBoundingBox sceneBb in obstacleList)
            {
                sceneBb.Draw(GraphicsDevice, _camera.View, _camera.Projection);
            }
            /*
            foreach (Matrix b in _boxes)
            {
                drawBox(b);
            }
             */ 
        }

        private void drawBox(Matrix matrix)
        {
            Matrix[] transforms = new Matrix[_boxModel.Bones.Count];
            _boxModel.CopyBoneTransformsTo(transforms);
            
            int sizeinbytes = _boxModel.Meshes[0].VertexBuffer.SizeInBytes;
            int count = sizeinbytes/Marshal.SizeOf(Vector3.Zero);
            Vector3[] vertices = new Vector3[count];
            _boxModel.Meshes[0].VertexBuffer.GetData<Vector3>(vertices);
            foreach (ModelMesh mesh in _boxModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = _camera.View;

                    effect.Projection = _camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index] *
                        matrix;
                }
                mesh.Draw();
            }        
        }

        public TriangleData[] GetFaces()
        {
            VertexList vertexList;
            Matrix rootTransform = _boxModel.Root.Transform;

            int totalNumFaces = 0;
            int totalNumVertices = 0;

            foreach (ModelMesh mesh in _boxModel.Meshes)
            {
                int numberVertices, numberIndices;
                numberVertices =
                    mesh.VertexBuffer.SizeInBytes / mesh.MeshParts[0].VertexStride;
                if (mesh.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
                    numberIndices = mesh.IndexBuffer.SizeInBytes / sizeof(short);
                else
                    numberIndices = mesh.IndexBuffer.SizeInBytes / sizeof(int);

                totalNumVertices += numberVertices;
                totalNumFaces += numberIndices / 3;
            }

            vertexList = new VertexList(totalNumVertices);
            TriangleData[] faces = new TriangleData[totalNumFaces];
            int vertexCount = 0;
            int faceCount = 0;
            foreach (ModelMesh mesh in _boxModel.Meshes)
            {
                int numberVertices = mesh.VertexBuffer.SizeInBytes / mesh.MeshParts[0].VertexStride;
                if (mesh.MeshParts[0].VertexStride == 16)
                {
                    VertexPositionColor[] meshVertices =
                        new VertexPositionColor[numberVertices];
                    mesh.VertexBuffer.GetData<VertexPositionColor>(meshVertices);

                    for (int i = 0; i < numberVertices; i++)
                        vertexList[i + vertexCount] = Vector3.Transform(meshVertices[i].Position, rootTransform);
                }
                if (mesh.MeshParts[0].VertexStride == 20)
                {
                    VertexPositionTexture[] meshVertices =
                                               new VertexPositionTexture[numberVertices];
                    mesh.VertexBuffer.GetData<VertexPositionTexture>(meshVertices);

                    for (int i = 0; i < numberVertices; i++)
                        vertexList[i + vertexCount] = Vector3.Transform(meshVertices[i].Position, rootTransform);
                }
                else if (mesh.MeshParts[0].VertexStride == 24)
                {
                    VertexPositionColorTexture[] meshVertices =
                        new VertexPositionColorTexture[numberVertices];

                    mesh.VertexBuffer.GetData<VertexPositionColorTexture>(meshVertices);

                    for (int i = 0; i < numberVertices; i++)
                        vertexList[i + vertexCount] = Vector3.Transform(meshVertices[i].Position, rootTransform);
                }
                else
                    if (mesh.MeshParts[0].VertexStride == 32)
                    {
                        VertexPositionNormalTexture[] meshVertices =
                            new VertexPositionNormalTexture[numberVertices];
                        mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(meshVertices);

                        for (int i = 0; i < numberVertices; i++)
                            vertexList[i + vertexCount] = Vector3.Transform(meshVertices[i].Position, rootTransform);
                    }
                    else
                        System.Diagnostics.Debug.Assert(false, "Unsupported vertex format!");

                int numberFaces = 0;

                if (mesh.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
                {
                    short[] meshIndices =
                            new short[mesh.IndexBuffer.SizeInBytes / sizeof(short)];
                    mesh.IndexBuffer.GetData<short>(meshIndices);

                    int count = 0;
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        for (int i = 0; i < meshPart.PrimitiveCount; i++)
                        {
                            int vertexOffset = vertexCount + meshPart.BaseVertex;
                            faces[numberFaces + faceCount] = new TriangleData(count, meshIndices, vertexOffset, vertexList);
                            count += 3;
                            numberFaces++;
                        }
                    }
                }
                else
                {
                    int[] meshIndices =
                            new int[mesh.IndexBuffer.SizeInBytes / sizeof(int)];
                    mesh.IndexBuffer.GetData<int>(meshIndices);

                    int count = 0;
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        for (int i = 0; i < meshPart.PrimitiveCount; i++)
                        {
                            int vertexOffset = vertexCount + meshPart.BaseVertex;
                            faces[numberFaces + faceCount] = new TriangleData(count, meshIndices, vertexOffset, vertexList);
                            count += 3;
                            numberFaces++;
                        }
                    }
                }

                vertexCount += numberVertices;
                faceCount += numberFaces;
            }

            return faces;            
            
        }
    }
}
