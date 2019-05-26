using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VulpineLib.Util;
using VulpineLib.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GFX
{
    public static class Camera
    {
        private const int CameraStackCountWarning = 64;

        /// <summary>
        /// Struct for the "block_camera" uniform block
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 128)]
        private struct UniformBlockCamera
        {
            [FieldOffset(0)]
            public Matrix4 Projection;

            [FieldOffset(64)]
            public Matrix4 View;
        }

        private static Stack<UniformBlockCamera> CameraStack = new Stack<UniformBlockCamera>();

        private static Stack<VulpineLib.Geometry.Rectangle> OrthoBoundsStack = new Stack<VulpineLib.Geometry.Rectangle>();

        /// <summary>
        /// Camera uniform buffer
        /// </summary>
        private static Buffer<UniformBlockCamera> CameraUniformBuffer;

        /// <summary>
        /// The projection matrix of the current camera
        /// </summary>
        public static Matrix4<float> Projection => CameraStack.Peek().Projection.ToVulpineMatrix();

        /// <summary>
        /// The view matrix of the current camera
        /// </summary>
        public static Matrix4<float> View => CameraStack.Peek().View.ToVulpineMatrix();

        /// <summary>
        /// The bounds of the camera if it is an ortho camera
        /// </summary>
        public static VulpineLib.Geometry.Rectangle OrthoBounds => OrthoBoundsStack.Peek();

        /// <summary>
        /// Initialize the camera
        /// </summary>
        public static void Init()
        {
            CameraUniformBuffer = Buffer<UniformBlockCamera>.Create(
                    "Camera Uniform Buffer", 1, BufferStorageFlags.MapWriteBit | BufferStorageFlags.ClientStorageBit
                );
            CameraUniformBuffer.BindAtUniformBlockBindingPoint(ReservedBindingPoints.Camera);
            CameraStack.Push(default);
            OrthoBoundsStack.Push(default);
            Set(Matrix4<float>.Identity, Matrix4<float>.Identity);
        }

        /// <summary>
        /// Set the camera view & projection
        /// </summary>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        public static void Set(Matrix4<float> view, Matrix4<float> projection)
        {
            var cam = new UniformBlockCamera
            {
                View = view.ToTKMatrix(),
                Projection = projection.ToTKMatrix()
            };
            CameraStack.Pop();
            CameraStack.Push(cam);
            OrthoBoundsStack.Pop();
            OrthoBoundsStack.Push(default);
            unsafe
            {
                var ptr = (UniformBlockCamera*)CameraUniformBuffer.Map(BufferAccess.WriteOnly);
                ptr[0] = cam;
                CameraUniformBuffer.Unmap();
            }
        }

        /// <summary>
        /// Set the camera to an orthographic camera
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        public static void SetOrtho(VulpineLib.Geometry.Rectangle bounds)
        {

            Set(
                    Matrix4<float>.CreateTranslation(new Vector3<float>(-bounds.Center, 0f)) * Matrix4<float>.CreateScale(new Vector3<float>(1f, -1f, 1f)),
                    Matrix4<float>.CreateOrthographic(bounds.Size, -1f, 1f)
                );
            OrthoBoundsStack.Pop();
            OrthoBoundsStack.Push(bounds);
        }

        /// <summary>
        /// Set the camera to an orthographic camera
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        public static void SetOrtho(Vector2<float> center, Vector2<float> size)
        {
            
            Set(
                    Matrix4<float>.CreateTranslation(new Vector3<float>(-center, 0f)),
                    Matrix4<float>.CreateOrthographic(size, -1f, 1f)
                );
            OrthoBoundsStack.Pop();
            OrthoBoundsStack.Push(new VulpineLib.Geometry.Rectangle(center - size / 2f, center + size / 2f));
        }

        /// <summary>
        /// Push a new camera on the camera stack
        /// </summary>
        public static void Push()
        {
            CameraStack.Push(CameraStack.Peek());
            OrthoBoundsStack.Push(OrthoBoundsStack.Peek());
#if DEBUG
            if (CameraStack.Count == CameraStackCountWarning)
                Log.Warning("Camera stack count is 64; possible stack imbalance", "Camera");
#endif
        }

        /// <summary>
        /// Pop the camera off the top of the camera stack
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Pop()
        {
            if (CameraStack.Count <= 1)
                throw new InvalidOperationException("Camera stack count <= 1; possible stack imbalance");
            CameraStack.Pop();
            OrthoBoundsStack.Pop();
            unsafe
            {
                var ptr = (UniformBlockCamera*)CameraUniformBuffer.Map(BufferAccess.WriteOnly);
                ptr[0] = CameraStack.Peek();
                CameraUniformBuffer.Unmap();
            }
        }

        /// <summary>
        /// Convert world coordinates to screen coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector3<float> WorldToScreen(Vector3<float> position)
        {
            var v3 = (Projection * View * Matrix4<float>.CreateTranslation(position)).Translation;
            return new Vector3<float>(v3.X, v3.Y, v3.Z);
        }
        /*
        /// <summary>
        /// Convert screen coordinates to world coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector3<float> ScreenToWorld(Vector3<float> position)
        {
            var trans = Projection * View;
            trans.Invert();
            var v3 = (trans * Matrix4.CreateTranslation(position.X, position.Y, position.Z)).ExtractTranslation();
            return new Vector3<float>(v3.X, v3.Y, v3.Z);
        }*/
    }
}
