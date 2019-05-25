using VulpineLib.Util;
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GFX
{
    internal class VertexAttributes : IGLObject
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
        private VertexAttributes()
        {
            Name = "";
            VertexAttributeNames = new string[] { };
        }

        /// <summary>
        /// Construct a VertexAttributes object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handle"></param>
        private VertexAttributes(string name, int handle)
        {
            Name = name;
            VertexAttributeNames = new string[GL.GetInteger(GetPName.MaxVertexAttribs)];
            GL.BindVertexArray(handle);
            GL.ObjectLabel(ObjectLabelIdentifier.VertexArray, handle, name.Length, name);
            Handle = new GLHandle(handle);
        }

        /// <summary>
        /// Called when finalizing
        /// </summary>
        ~VertexAttributes()
        {
            Dispose();
        }

        /// <summary>
        /// Construct a VertexAttributes object
        /// </summary>
        /// <param name="name"></param>
        public static VertexAttributes Create(string name)
        {
            return new VertexAttributes(name, GL.GenVertexArray());
        }

        /// <summary>
        /// Keeps track of the names of vertex attribute layouts
        /// </summary>
        private string[] VertexAttributeNames;

        /// <summary>
        /// Enable/disable vertex attribute
        /// </summary>
        /// <param name="layoutNum">Layout number of the attribute in the shader</param>
        /// <param name="enabled">Whether this attribute is enabled</param>
        public void SetVertexAttributeEnabled(int layoutNum, bool enabled)
        {
            this.Assert();

            Bind();
            if (enabled)
                GL.EnableVertexAttribArray(layoutNum);
            else
                GL.DisableVertexAttribArray(layoutNum);
        }
        /// <summary>
        /// Set the layout info for a vertex attribute
        /// </summary>
        /// <param name="layoutNum">Layout number of the attribute in the shader</param>
        /// <param name="name">Name of the vertex attribute layout</param>
        /// <param name="type">Type of the vertex attribute</param>
        /// <param name="normalized">Normalized?</param>
        /// <param name="stride">The stride of the data</param>
        /// <param name="offset">The offset of the data</param>
        /// <param name="vertexBuffer"></param>
        public void SetVertexAttributeLayout<T>(int layoutNum, string name, AttributeType type, bool normalized, int stride, int offset, Buffer<T> vertexBuffer)
            where T : unmanaged
        {
            this.Assert();

            vertexBuffer.Bind(BufferTarget.ArrayBuffer);
            VertexAttributeNames[layoutNum] = name;
            var glAttribType = type.ToVertexAttribPointerType(out var size);
            GL.VertexAttribPointer(layoutNum, size, glAttribType, normalized, stride, offset);
        }

        /// <summary>
        /// Bind this object in the state
        /// </summary>
        public void Bind()
        {
            this.Assert();

            GL.BindVertexArray(Handle.Handle);
        }

        /// <summary>
        /// Unbind all from the state
        /// </summary>
        public static void Unbind()
        {
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposed)
                return;

            Handle = GLHandle.Null;
            GL.DeleteVertexArray(Handle.Handle);
            Disposed = true;
        }

        public override string ToString()
        {
            return this.DefaultToString();
        }
    }
}
