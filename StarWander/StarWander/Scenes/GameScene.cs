using System;
using VulpineLib.Util;
using VulpineLib.Animation;
using OpenTK.Graphics.OpenGL4;
using StarWander.GFX;
using StarWander.GameObjects.Actors.Ships;

namespace StarWander.Scenes
{
    class GameScene : Scene
    {
        private PlayerShip? PlayerShip;
        private MeshRenderer<Vertex<Vector3<float>, Vector3<float>>>? MeshRender;
        private GPUMesh<Vertex<Vector3<float>, Vector3<float>>>? TestMesh;

        public GameScene() : base(nameof(GameScene))
        {
        }

        protected override void OnStart()
        {
            PlayerShip = new PlayerShip(Vector3<float>.Zero);
            MeshRender = new MeshRenderer<Vertex<Vector3<float>, Vector3<float>>>("Test MeshRenderer");
            var vertices = new[]
            {
                Vertex.VertexP(new Vector3<float>(-1f, 0f)),
                Vertex.VertexP(new Vector3<float>(1f, 0f)),
                Vertex.VertexP(new Vector3<float>(0f, 1f))
            };
            var indices = new[] { 0, 1, 2 };
            var mesh = Mesh<Vertex<Vector3<float>, Vector3<float>>>.CreateCubeSphere(Vector3<float>.Zero, 1f, 0);
            for (var i = 0; i < mesh.Vertices.Length; i++)
            {
                var vert = mesh.Vertices[i];
                vert.V1 = vert.V0.Normalized.To<float>();
                mesh.Vertices[i] = vert;
            }
            TestMesh = new GPUMesh<Vertex<Vector3<float>, Vector3<float>>>(mesh);
        }

        protected override void OnEnd()
        {
        }

        protected override void OnUpdate(double time, double deltaTime)
        {
            if (PlayerShip is null)
                throw new NullReferenceException();

            PlayerShip.Update(time, deltaTime);
        }

        protected override void OnDraw(double time, double deltaTime)
        {
            if (MeshRender is null)
                throw new NullReferenceException();
            if (TestMesh is null)
                throw new NullReferenceException();
            if (PlayerShip is null)
                throw new NullReferenceException();
            if (SGameWindow.Main is null)
                throw new NullReferenceException();

            // Push a new first person camera
            Camera.Push();
            PlayerShip.SetCamera((double)SGameWindow.Main.VectorSize.X / SGameWindow.Main.VectorSize.Y);

            // TODO: Draw demo stuff in first person here, moved forward in the X direction so it's not inside the camera
            MeshRender.Begin();
            var trans = Matrix4<float>.CreateTranslation(new Vector3<float>(3f, 0f, 2f));
            GL.Enable(EnableCap.DepthTest);
            MeshRender.DrawMesh(TestMesh, ref trans, Color.White, BlendType.AlphaPremultiplied);
            MeshRender.End();

            Camera.Pop();
            // No longer in first person
        }
    }
}
