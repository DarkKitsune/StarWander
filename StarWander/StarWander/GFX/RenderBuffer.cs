using System;
using OpenTK.Graphics.OpenGL4;
using VulpineLib.Util;

namespace StarWander.GFX
{
    internal class RenderBuffer : IGLObject
    {
        public bool Disposed { get; set; }

        /// <summary>
        /// The name of this object
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The GLHandle pointing to this object
        /// </summary>
        public GLHandle Handle { get; set; }

        /// <summary>
        /// Don't allow empty constructor
        /// </summary>
        private RenderBuffer()
        {
            Name = "";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Construct a render buffer
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        private RenderBuffer(string name, int handle)
        {
            Name = name;
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, handle);
            if (State.VersionIsAtLeast(4, 3))
                GL.ObjectLabel(ObjectLabelIdentifier.Renderbuffer, handle, name.Length, name);
            Handle = new GLHandle(handle);
        }

        /// <summary>
        /// Create a render buffer
        /// </summary>
        /// <param name="name">The name for the object</param>
        /// <param name="dimensions">The dimensions of the render buffer</param>
        /// <param name="format">The format of the render buffer</param>
        public RenderBuffer Create(string name, Vector2<int> dimensions, RenderbufferStorage format = RenderbufferStorage.DepthComponent)
        {
            var rbuffer = new RenderBuffer(name, GL.GenRenderbuffer());
            rbuffer.Storage(dimensions, format);
            return rbuffer;
        }

        /// <summary>
        /// Bind the render buffer to the state
        /// </summary>
        public void Bind()
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Handle.Handle);
        }

        /// <summary>
        /// Define the render buffer's storage
        /// </summary>
        /// <param name="dimensions">The dimensions of the buffer</param>
        /// <param name="format">The format of the buffer</param>
        private void Storage(Vector2<int> dimensions, RenderbufferStorage format)
        {
            Bind();
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, format, dimensions.X, dimensions.Y);
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposed)
                return;

            Handle = GLHandle.Null;
            GL.DeleteShader(Handle.Handle);
            Disposed = true;
        }

        public override string ToString()
        {
            return this.DefaultToString();
        }
    }
}
