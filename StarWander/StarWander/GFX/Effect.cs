using System;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public class Effect : IDisposable, INameable
    {
        /// <summary>
        /// Whether this object has been disposed
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// The name of this object
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Whether Init has been called
        /// </summary>
        protected bool Initialized { get; private set; }

        /// <summary>
        /// The time of the last call to Update
        /// </summary>
        private double LastUpdate;

        /// <summary>
        /// The time of the last call to Draw
        /// </summary>
        private double LastDraw;

        public Effect(string name)
        {
            Name = name;
        }

        ~Effect()
        {
            Dispose();
        }

        /// <summary>
        /// Set a parameter in the effect
        /// </summary>
        /// <param name="key">The key of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void SetParameter(string key, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a parameter in the effect
        /// </summary>
        /// <param name="key">The key of the parameter</param>
        /// <returns>The value of the parameter</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual object GetParameter(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Init the effect
        /// </summary>
        /// <param name="time"></param>
        public void Init(double time)
        {
            Assert();

            LastUpdate = time;
            LastDraw = time;
            OnInit(time);
            Initialized = true;
        }

        /// <summary>
        /// Update the effect
        /// </summary>
        /// <param name="time">The time</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Update(double time)
        {
            Assert();
            if (!Initialized)
                throw new InvalidOperationException("Effect has not been initialized with Init()");

            OnUpdate(time, time - LastUpdate);
            LastUpdate = time;
        }

        /// <summary>
        /// Draw the effect
        /// </summary>
        /// <param name="time">The time</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Draw(double time)
        {
            Assert();
            if (!Initialized)
                throw new InvalidOperationException("Effect has not been initialized with Init()");

            OnDraw(time, time - LastDraw);
            LastDraw = time;
        }

        /// <summary>
        /// Called on init
        /// </summary>
        /// <param name="time">The time</param>
        protected virtual void OnInit(double time)
        {
        }

        /// <summary>
        /// Called on update
        /// </summary>
        /// <param name="time">The time</param>
        /// <param name="delta">Time passed since last update</param>
        protected virtual void OnUpdate(double time, double delta)
        {
        }

        /// <summary>
        /// Called on draw
        /// </summary>
        /// <param name="time">The time</param>
        /// <param name="delta">Time passed since last draw</param>
        protected virtual void OnDraw(double time, double delta)
        {
        }

        /// <summary>
        /// Assert that this object isn't disposed; throws an ObjectDisposedException if Disposed == true
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public void Assert()
        {
            if (Disposed)
                throw new ObjectDisposedException(Name);
        }

        /// <summary>
        /// Dispose any unmanaged resources used by this object
        /// </summary>
        public void Dispose()
        {
            if (Disposed)
                return;
            OnDispose();
            Disposed = true;
        }

        /// <summary>
        /// Called on disposal
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }
}
