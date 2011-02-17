using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Simples.Mechanisms
{
    public class MechanismEnviroment
    {
        private List<OrientedBoundingBox> obstacleList;
        private Model sceneModel;
        private OctreeNode octree;

        public Model SceneModel
        {
            get { return sceneModel; }
        }
        
        public OctreeNode Octree
        {
            get { return octree; }
        }

        public MechanismEnviroment(Model sceneModel)
        {

            this.sceneModel = sceneModel;

            obstacleList = new List<OrientedBoundingBox>();

            List<TriangleData> triangles = GetFaces();

            octree = new OctreeNode(new Vector3(-450, -450, -50), new Vector3(550, 550, 100), 0);



            foreach (TriangleData face in triangles)
            {
                octree.AddTriangle(face);
            }
            octree.Divide();

        }

        public List<TriangleData> GetFaces()
        {
            Matrix rootTransform = sceneModel.Root.Transform;

            List<TriangleData> faceList = new List<TriangleData>();// new TriangleData[totalNumFaces];

            foreach (ModelMesh mesh in sceneModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    VertexPositionColorTexture[] meshPartVertices = new VertexPositionColorTexture[meshPart.NumVertices];
                    meshPart.VertexBuffer.GetData<VertexPositionColorTexture>(meshPartVertices);

                    if (meshPart.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
                    {
                        short[] meshIndices = new short[meshPart.IndexBuffer.IndexCount];
                        meshPart.IndexBuffer.GetData<short>(meshIndices);

                        for (int cFaces = 0; cFaces < meshPart.PrimitiveCount; cFaces++)
                        {
                            Vector3[] vertices = new Vector3[3];
                            for (int cFaceVertice = 0; cFaceVertice < 3; cFaceVertice++)
                            {
                                vertices[cFaceVertice] = Vector3.Transform(meshPartVertices[meshIndices[meshPart.VertexOffset + (cFaces * 3) + cFaceVertice]].Position, rootTransform);
                            }
                            TriangleData triangleData = new TriangleData(ref vertices);
                            faceList.Add(triangleData);

                        }

                    }
                    else
                    {
                        int[] meshIndices = new int[meshPart.IndexBuffer.IndexCount];
                        meshPart.IndexBuffer.GetData<int>(meshIndices);

                        for (int cFaces = 0; cFaces < meshPart.PrimitiveCount; cFaces++)
                        {
                            Vector3[] vertices = new Vector3[3];
                            for (int cFaceVertice = 0; cFaceVertice < 3; cFaceVertice++)
                            {
                                vertices[cFaceVertice] = Vector3.Transform(meshPartVertices[meshIndices[meshPart.VertexOffset + (cFaces * 3) + cFaceVertice]].Position, rootTransform);
                            }
                            TriangleData triangleData = new TriangleData(ref vertices);
                            faceList.Add(triangleData);

                        }

                    }

                }
            }
            return faceList;
        }

        public bool IsColliding(Mechanism mechanism)
        {
            return octree.IsColliding(mechanism);
            /*
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
            */
        }


    }
}
