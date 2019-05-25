using System;
using VulpineLib.Util;
using VulpineLib.Util.Generics;
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GFX
{
    /// <summary>
    /// A GL buffer
    /// </summary>
    public class Buffer<T> : IGLObject
        where T : unmanaged
    {
        public bool Disposed { get; private set; }

        /// <summary>
        /// The name of this buffer
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The GLHandle pointing to this buffer
        /// </summary>
        public GLHandle Handle { get; private set; }

        /// <summary>
        /// Prevent empty constructor
        /// </summary>
        private Buffer()
        {
            Name = "";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Construct a new buffer
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <param name="flags"></param>
        /// <param name="data"></param>
        private Buffer(
                int handle, string name, IntPtr size, BufferStorageFlags flags = BufferStorageFlags.None, IntPtr data = default
            )
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferStorage(BufferTarget.ArrayBuffer, size, data, flags);
            if (State.VersionIsAtLeast(4, 3))
                GL.ObjectLabel(ObjectLabelIdentifier.Buffer, handle, name.Length, name);
            Handle = new GLHandle(handle);
            Name = name;
        }

        /// <summary>
        /// Called during finalization
        /// </summary>
        ~Buffer()
        {
            Dispose();
        }

        /// <summary>
        /// Generate an empty buffer
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        /// <param name="flags"></param>
        /// <param name="data"></param>
        /// <returns>The new buffer</returns>
        public static unsafe Buffer<T> Create(
                string name, long length, BufferStorageFlags flags = BufferStorageFlags.None, IntPtr data = default
            )
        {
            return new Buffer<T>(GL.GenBuffer(), name, (IntPtr)(TypeInfo<T>.TypeSize * length), flags, data);
        }

        /// <summary>
        /// Bind this buffer at the target in the state
        /// </summary>
        /// <param name="target"></param>
        public void Bind(BufferTarget target)
        {
            this.Assert();

            GL.BindBuffer(target, Handle.Handle);
        }

        /*
        /// <summary>
        /// Bind this buffer at a binding point in an indexed target
        /// </summary>
        public void Bind(BufferRangeTarget target, int bindingPoint)
        {
            this.Assert();

            GL.BindBufferBase(target, bindingPoint, Handle.Handle);
        }

        /// <summary>
        /// Bind a portion of this buffer at a binding point in an indexed target
        /// </summary>
        public void Bind(BufferRangeTarget target, int bindingPoint, long start, long count)
        {
            this.Assert();

            GL.BindBufferRange(
                    target, bindingPoint, Handle.Handle,
                    (IntPtr)(start * GenericMath<T>.TypeSize),
                    (IntPtr)(count * GenericMath<T>.TypeSize)
                );
        }
        */

        /// <summary>
        /// Bind this buffer at a uniform block binding point
        /// </summary>
        /// <param name="bindingPoint"></param>
        public void BindAtUniformBlockBindingPoint(UniformBlockBindingPoint bindingPoint)
        {
            this.Assert();

            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, bindingPoint.Index, Handle.Handle);
        }

        /// <summary>
        /// Bind a portion of this buffer at a binding point in an indexed target
        /// </summary>
        /// <param name="bindingPoint"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void BindAtUniformBlockBindingPoint(UniformBlockBindingPoint bindingPoint, long start, long count)
        {
            this.Assert();

            GL.BindBufferRange(
                    BufferRangeTarget.UniformBuffer, bindingPoint.Index, Handle.Handle,
                    (IntPtr)(start * TypeInfo<T>.TypeSize),
                    (IntPtr)(count * TypeInfo<T>.TypeSize)
                );
        }

        /// <summary>
        /// Unbind all buffers at the target in the state
        /// </summary>
        /// <param name="target"></param>
        public static void Unbind(BufferTarget target)
        {
            GL.BindBuffer(target, 0);
        }

        /// <summary>
        /// Invalidate this buffer's contents
        /// </summary>
        public void Invalidate()
        {
            this.Assert();

            GL.InvalidateBufferData(Handle.Handle);
        }

        /// <summary>
        /// Map the buffer to program memory for writing/reading
        /// </summary>
        /// <param name="bufferAccess"></param>
        public unsafe T* Map(BufferAccess bufferAccess)
        {
            this.Assert();

            Bind(BufferTarget.ArrayBuffer);
            return (T*)GL.MapBuffer(BufferTarget.ArrayBuffer, bufferAccess);
        }

        /// <summary>
        /// Map a portion of the buffer to program memory for writing/reading
        /// </summary>
        /// <param name="bufferAccess"></param>
        /// <param name="start">Element to start at</param>
        /// <param name="count"></param>
        public unsafe T* Map(BufferAccessMask bufferAccess, long start, long count)
        {
            this.Assert();

            Bind(BufferTarget.ArrayBuffer);
            return (T*)GL.MapBufferRange(
                    BufferTarget.ArrayBuffer,
                    (IntPtr)(start * TypeInfo<T>.TypeSize),
                    (IntPtr)(count * TypeInfo<T>.TypeSize),
                    bufferAccess
                );
        }

        /// <summary>
        /// Unmap the buffer from program memory
        /// </summary>
        public void Unmap()
        {
            this.Assert();

            Bind(BufferTarget.ArrayBuffer);
            GL.UnmapBuffer(BufferTarget.ArrayBuffer);
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposed)
                return;

            Handle = GLHandle.Null;
            GL.DeleteBuffer(Handle.Handle);
            Disposed = true;
        }

        public override string ToString()
        {
            return this.DefaultToString();
        }
    }
}
