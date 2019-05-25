using System;
using VulpineLib.Util;

namespace StarWander.Scenes
{
    public class Scene : INameable
    {
        /// <summary>
        /// Get the Name of this object
        /// </summary>
        public string Name { get; private set; }

        private Scene()
        {
            Name = "";
        }

        protected Scene(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Call when starting the scene
        /// </summary>
        public void Start()
        {
            // Subscribe to mouse events
            Input.Mouse.LeftPress += () =>
            {
                LeftMouseButtonPress();
                return false;
            };
            Input.Mouse.LeftRelease += () =>
            {
                LeftMouseButtonRelease();
                return false;
            };
            Input.Mouse.RightPress += () =>
            {
                RightMouseButtonPress();
                return false;
            };
            Input.Mouse.RightRelease += () =>
            {
                RightMouseButtonRelease();
                return false;
            };

            // Call OnStart
            OnStart();
        }

        /// <summary>
        /// Called when starting the scene
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Call when ending the scene
        /// </summary>
        public void End()
        {
            OnEnd();
        }

        /// <summary>
        /// Called when ending the scene
        /// </summary>
        protected virtual void OnEnd()
        {
        }

        /// <summary>
        /// Call when left mouse button press occurs
        /// </summary>
        public void LeftMouseButtonPress()
        {
            OnLeftMouseButtonPress();
        }

        /// <summary>
        /// Called when left mouse button press occurs
        /// </summary>
        protected virtual void OnLeftMouseButtonPress()
        {
        }

        /// <summary>
        /// Call when left mouse button release occurs
        /// </summary>
        public void LeftMouseButtonRelease()
        {
            OnLeftMouseButtonRelease();
        }

        /// <summary>
        /// Called when left mouse button release occurs
        /// </summary>
        protected virtual void OnLeftMouseButtonRelease()
        {
        }

        /// <summary>
        /// Call when left mouse button press occurs
        /// </summary>
        public void RightMouseButtonPress()
        {
            OnRightMouseButtonPress();
        }

        /// <summary>
        /// Called when left mouse button press occurs
        /// </summary>
        protected virtual void OnRightMouseButtonPress()
        {
        }

        /// <summary>
        /// Call when left mouse button release occurs
        /// </summary>
        public void RightMouseButtonRelease()
        {
            OnRightMouseButtonRelease();
        }

        /// <summary>
        /// Called when left mouse button release occurs
        /// </summary>
        protected virtual void OnRightMouseButtonRelease()
        {
        }

        /// <summary>
        /// Call when the window is resized
        /// </summary>
        public void WindowResized()
        {
        }

        /// <summary>
        /// Time taken in the last Update() call
        /// </summary>
        public double UpdateTime { get; private set; }
        /// <summary>
        /// Time taken in the last Update() call, but smoothed a little
        /// </summary>
        public double SmoothedUpdateTime { get; private set; }
        /// <summary>
        /// Update the scene
        /// </summary>
        /// <param name="time"></param>
        public void Update(double time, double deltaTime)
        {
            // Start timing 
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            OnUpdate(time, deltaTime);

            // End timing and store elapsed seconds in UpdateTime and SmoothedUpdateTime
            sw.Stop();
            UpdateTime = sw.ElapsedTicks / (double)TimeSpan.TicksPerSecond;
            SmoothedUpdateTime = (SmoothedUpdateTime * 9.0 + UpdateTime) / 10.0;
        }
        /// <summary>
        /// Called when updating the scene
        /// </summary>
        /// <param name="time"></param>
        protected virtual void OnUpdate(double time, double deltaTime)
        {
        }

        /// <summary>
        /// Time taken in the last Draw() call
        /// </summary>
        public double DrawTime { get; private set; }
        /// <summary>
        /// Time taken in the last Draw() call, but smoothed a little
        /// </summary>
        public double SmoothedDrawTime { get; private set; }
        /// <summary>
        /// Draw the scene
        /// </summary>
        /// <param name="time"></param>
        public void Draw(double time, double deltaTime)
        {
            // Start timing 
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            OnDraw(time, deltaTime);

            // End timing and store elapsed seconds in DrawTime and SmoothedDrawTime
            sw.Stop();
            DrawTime = sw.ElapsedTicks / (double)TimeSpan.TicksPerSecond;
            SmoothedDrawTime = (SmoothedDrawTime * 9.0 + DrawTime) / 10.0;
        }
        /// <summary>
        /// Called when drawing the scene
        /// </summary>
        /// <param name="time"></param>
        protected virtual void OnDraw(double time, double deltaTime)
        {
        }
    }
}
