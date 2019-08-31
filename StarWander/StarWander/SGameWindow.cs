using System;
using VulpineLib.Util;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using StarWander.GFX;
using StarWander.Scenes;

namespace StarWander
{
    internal class SGameWindow : GameWindow, INameable
    {
        public static SGameWindow? Main;

        /// <summary>
        /// The rate at which to run the OnUpdateFrame method
        /// </summary>
        public const double UpdateRate = 60.0;
        /// <summary>
        /// The rate at which to run the OnRenderFrame method
        /// </summary>
        public const double DrawRate = 60.0;
        public const string WindowTitle = "Untitled Game";
        /// <summary>
        /// The flags to use for the OpenGL context
        /// </summary>
        public const GraphicsContextFlags ContextFlags =
#if DEBUG // If targetting Debug then use a debug OpenGL context
            GraphicsContextFlags.Debug | GraphicsContextFlags.ForwardCompatible;
#else
            GraphicsContextFlags.ForwardCompatible;
#endif

        /// <summary>
        /// Get the INameable.Name for this object
        /// </summary>
        public string Name => Title;

        /// <summary>
        /// Returns a graphics mode with some simple default settings
        /// </summary>
        private static GraphicsMode DefaultGraphicsMode => new GraphicsMode(new ColorFormat(8, 8, 8, 8), 16);

        /// <summary>
        /// Return the aspect ratio
        /// </summary>
        public float AspectRatio => (float)ClientSize.Width / ClientSize.Height;

        /// <summary>
        /// Return the size as a vector
        /// </summary>
        public Vector2<int> VectorSize => new Vector2<int>(ClientSize.Width, ClientSize.Height);

        /// <summary>
        /// The scene manager
        /// </summary>
        private SceneManager SceneManager = new SceneManager();

        /// <summary>
        /// Last updated at this time
        /// </summary>
        private double LastUpdate;

        /// <summary>
        /// Last drew at this time
        /// </summary>
        private double LastDraw;

        /// <summary>
        /// Game mode the window is running in
        /// </summary>
        public GameWindowMode Mode { get; private set; }

        /// <summary>
        /// Construct the game window
        /// </summary>
        /// <param name="mode"></param>
        public SGameWindow(GameWindowMode mode = GameWindowMode.Game) : base(
                800, 600, DefaultGraphicsMode, WindowTitle, GameWindowFlags.Default,
                DisplayDevice.Default, State.PreferredVersion.Major, State.PreferredVersion.Minor, ContextFlags
            )
        {
            Mode = mode;
            if (Main == null)
                Main = this;
        }

        /// <summary>
        /// Run with default settings
        /// </summary>
        new public void Run()
        {
            Run(UpdateRate, DrawRate);
        }

        /// <summary>
        /// Called when the game loads
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Get the time
            var time = Program.CurrentTime;

            Log.Info("Game is loading...", ToString());

            State.Init();

            Panels.Panel.Init();

            GameObjects.Actors.SpriteActor.Init();

#if DEBUG
            /*if (Mode == GameWindowMode.WorldEditor)
                SceneManager.StartScene(new EditorScene(), time);
            else*/
                SceneManager.StartScene(new GameScene(), time);
#else
            SceneManager.StartScene(new GameScene(), time);
#endif
        }

        /// <summary>
        /// Update logic
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // Get the time
            var time = Program.CurrentTime;
            if (LastUpdate == 0.0)
                LastUpdate = time;
            var delta = time - LastUpdate;
            LastUpdate = time;

            // Update input
            Input.Mouse.Update();
            //Input.Keyboard.Update();
            Input.GamePad.Update();

            // Update the scene
            SceneManager.Update(time, delta);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Get the time
            var time = Program.CurrentTime;
            if (LastDraw == 0.0)
                LastDraw = time;
            var delta = time - LastDraw;
            LastDraw = time;

            // Clear the frame buffer and resize the viewport
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height);

            // Draw the scene
            SceneManager.Draw(time, delta);
            if (SceneManager.CurrentScene == null)
                throw new NullReferenceException();
            Title = $"Untitled Game | Scene draw time: {SceneManager.CurrentScene.SmoothedDrawTime * 1000.0:0.00}ms";

            // Draw panels
            Panels.Panel.Draw(VectorSize);

            // Swap the frame buffers
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            SceneManager.WindowResized();

            Panels.Panel.WindowResized();
        }

        public override string ToString()
        {
            return this.DefaultToString();
        }
    }
}
