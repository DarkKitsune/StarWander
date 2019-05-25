using System;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public class Renderer : INameable, IDisposable
    {
        /// <summary>
        /// Whether this object has been disposed
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Get the name of this object
        /// </summary>
        public string Name { get; private set; }

        private static Renderer? CurrentlyDrawing;
        protected bool Drawing { get; private set; }

        protected Renderer(string name)
        {
            if (CurrentlyDrawing != null)
                throw new InvalidOperationException($"Renderer \"{CurrentlyDrawing.Name}\" is already currently drawing; cannot construct");
            Name = name;
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            if (Disposed)
                return;
            Disposed = true;
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }

        public void Assert()
        {
            if (Disposed)
                throw new ObjectDisposedException(Name);
        }

        public void Begin()
        {
            Assert();

            if (Drawing)
                throw new InvalidOperationException("Already drawing; End() must be called first");
            if (CurrentlyDrawing != null)
                throw new InvalidOperationException($"Renderer \"{CurrentlyDrawing.Name}\" is already currently drawing");
            CurrentlyDrawing = this;
            Drawing = true;
            OnBegin();
        }

        protected virtual void OnBegin()
        {
        }

        /// <summary>
        /// Finish drawing sprites
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void End()
        {
            Assert();

            if (!Drawing)
                throw new InvalidOperationException("Not drawing; Begin() must be called first");
            Drawing = false;
            OnEnd();
            CurrentlyDrawing = null;
        }

        protected virtual void OnEnd()
        {
        }
    }
}
