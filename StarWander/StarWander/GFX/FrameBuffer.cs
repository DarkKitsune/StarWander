using System;
using OpenTK.Graphics.OpenGL4;
using VulpineLib.Util;

namespace StarWander.GFX
{
    internal class FrameBuffer : IGLObject
    {
        public bool Disposed { get; private set; }

        /// <summary>
        /// The name of this object
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The GLHandle pointing to this object
        /// </summary>
        public GLHandle Handle { get; private set; }

        /// <summary>
        /// Don't allow empty constructor
        /// </summary>
        private FrameBuffer()
        {
            Name = "";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Construct a frame buffer
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        private FrameBuffer(string name, int handle)
        {
            Name = name;
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
            if (State.VersionIsAtLeast(4, 3))
                GL.ObjectLabel(ObjectLabelIdentifier.Framebuffer, handle, name.Length, name);
            Handle = new GLHandle(handle);
        }

        /// <summary>
        /// Create a frame buffer
        /// </summary>
        /// <param name="name">The name for the object</param>
        public FrameBuffer Create(string name)
        {
            var fbuffer = new FrameBuffer(name, GL.GenFramebuffer());
            return fbuffer;
        }

        /// <summary>
        /// Bind the frame buffer to the target in the state
        /// </summary>
        /// <param name="target"></param>
        public void Bind(FramebufferTarget target)
        {
            this.Assert();

            GL.BindFramebuffer(target, Handle.Handle);
        }

        /// <summary>
        /// Unbinds any frame buffer from the target in the state
        /// </summary>
        /// <param name="target"></param>
        public static void Unbind(FramebufferTarget target)
        {
            GL.BindFramebuffer(target, 0);
        }

        /// <summary>
        /// Attaches a render buffer
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="renderBuffer"></param>
        public void AttachRenderBuffer(FramebufferAttachment attachment, RenderBuffer renderBuffer)
        {
            this.Assert();

            Bind(FramebufferTarget.Framebuffer);
            renderBuffer.Bind();
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, RenderbufferTarget.Renderbuffer, renderBuffer.Handle.Handle);
        }

        /// <summary>
        /// Attaches a render buffer
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="texture2D"></param>
        /// <param name="level"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AttachTexture(FramebufferAttachment attachment, Texture2D texture2D, int level)
        {
            this.Assert();

            if (level < 0 || level >= texture2D.Levels)
                throw new ArgumentOutOfRangeException(nameof(level));
            Bind(FramebufferTarget.Framebuffer);
            texture2D.Bind(TextureTarget.Texture2D);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D, texture2D.Handle.Handle, level);
        }

        /// <summary>
        /// Set the buffers to draw to
        /// </summary>
        /// <param name="drawBuffers"></param>
        public void SetDrawBuffers(params DrawBuffersEnum[] drawBuffers)
        {
            this.Assert();

            Bind(FramebufferTarget.DrawFramebuffer);
            GL.DrawBuffers(drawBuffers.Length, drawBuffers);
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
