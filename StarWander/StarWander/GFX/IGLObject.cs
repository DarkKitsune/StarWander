using System;
using VulpineLib.Util;

namespace StarWander.GFX
{
    /// <summary>
    /// An interface for GL objects with handles
    /// </summary>
    public interface IGLObject : INameable, IDisposable
    {
        /// <summary>
        /// Whether this object has been disposed
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// Get the GL handle of this object
        /// </summary>
        GLHandle Handle { get; }
    }
    /// <summary>
    /// Extensions for the IGLObject interface
    /// </summary>
    public static class IGLObjectExtensions
    {
        /// <summary>
        /// Assert that this object isn't disposed; throws an ObjectDisposedException if Disposed == true
        /// </summary>
        /// <param name="self"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public static void Assert(this IGLObject self)
        {
            if (self.Disposed)
                throw new ObjectDisposedException(self.Name);
        }
    }
}
