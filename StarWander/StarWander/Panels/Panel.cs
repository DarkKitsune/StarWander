using System;
using System.Collections.Generic;
using VulpineLib.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using StarWander.GFX;

namespace StarWander.Panels
{
    /// <summary>
    /// Panel class
    /// </summary>
    public class Panel : INameable
    {
        private static Sprite DefaultSprite => SpriteSets.Editor["Panel"];
        private static List<Panel> ShownPanels = new List<Panel>();
        private static SpriteRenderer? SpriteRenderer;

        public static void Init()
        {
            Input.Mouse.LeftPress += Mouse_LeftPress;
            SpriteRenderer = new SpriteRenderer("Panel Sprite Renderer");
        }

        public static void Draw(Vector2<int> windowSize)
        {
            if (SpriteRenderer == null)
                throw new NullReferenceException();

            var floatWindowSize = windowSize.To<float>();

            Camera.Push();
            Camera.SetOrtho(new VulpineLib.Geometry.Rectangle(Vector2<float>.Zero, floatWindowSize));
            SpriteRenderer.Begin();
            foreach (var panel in ShownPanels)
                panel.Draw(SpriteRenderer);
            SpriteRenderer.End();
            Camera.Pop();
        }

        public static void WindowResized()
        {
            if (SGameWindow.Main == null)
                throw new NullReferenceException();
            var size = SGameWindow.Main.VectorSize;
            foreach (var panel in ShownPanels)
                panel.WindowResized(size);
        }

        private static bool Mouse_LeftPress()
        {
            for (var i = ShownPanels.Count - 1; i >= 0; i--)
            {
                var panel = ShownPanels[i];
                if (panel.MouseOver)
                {
                    panel.LeftMouseButtonPress();
                    return true;
                }
            }
            return false;
        }

        public string Name { get; private set; }

        public Vector2<int> LeftTop { get; protected set; }
        public Vector2<int> Size { get; protected set; }
        public Vector2<int> RightBottom => LeftTop + Size;
        public Vector2<float> Center => LeftTop.To<float>() + Size.To<float>() / 2f;
        public Vector2<int> LocalMousePosition => new Vector2<int>(
                Input.Mouse.Position.X - LeftTop.X,
                Input.Mouse.Position.Y - LeftTop.Y
            );
        public bool MouseOver => LocalMousePosition.BetweenOrEqual(Vector2<int>.Zero, Size - 1);
        public bool Shown { get; private set; }
        public int Margin { get; protected set; } = 3;
        public static List<Control> Controls { get; private set; } = new List<Control>();

        /// <summary>
        /// Don't allow empty constructor 
        /// </summary>
        private Panel()
        {
            Name = "";
        }

        public Panel(string name, Vector2<int> leftTop, Vector2<int> size)
        {
            Name = name;
            LeftTop = leftTop;
            Size = size;
        }

        public void Show()
        {
            if (Shown)
                return;
            ShownPanels.Add(this);
            OnShow();
        }

        protected virtual void OnShow()
        {
        }

        public void Close()
        {
            if (!Shown)
                return;
            Shown = false;
            ShownPanels.Remove(this);
            OnClose();
        }

        protected virtual void OnClose()
        {
        }

        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        /// <summary>
        /// Draw this panel
        /// </summary>
        /// <param name="spriteRenderer"></param>
        public void Draw(SpriteRenderer spriteRenderer)
        {
            spriteRenderer.MagFilter = TextureMagFilter.Nearest;
            spriteRenderer.MinFilter = TextureMinFilter.Nearest;

            var mat = Matrix4<float>.CreateScale(new Vector3<float>(Size.To<float>(), 0f))
                * Matrix4<float>.CreateTranslation(new Vector3<float>(Center.To<float>(), 0f));
            spriteRenderer.DrawSprite(
                    DefaultSprite,
                    ref mat,
                    VulpineLib.Util.Color.White,
                    BlendType.AlphaPremultiplied
                );

            OnDraw(spriteRenderer);

            foreach (var control in Controls)
                control.Draw(this, spriteRenderer);
        }

        /// <summary>
        /// Called when the panel is drawn
        /// </summary>
        /// <param name="spriteRenderer"></param>
        protected virtual void OnDraw(SpriteRenderer spriteRenderer)
        {
        }

        /// <summary>
        /// Window has been resized
        /// </summary>
        /// <param name="size"></param>
        private void WindowResized(Vector2<int> size)
        {
            OnWindowResized(size);
        }

        /// <summary>
        /// Called when the window is resized
        /// </summary>
        /// <param name="size"></param>
        protected virtual void OnWindowResized(Vector2<int> size)
        {
        }

        /// <summary>
        /// Simulate a left click
        /// </summary>
        public void LeftMouseButtonPress()
        {
            for (var i = Controls.Count - 1; i >= 0; i--)
            {
                if (Controls[i].MouseOver)
                {
                    Controls[i].LeftMouseButtonPress();
                    return;
                }
            }
            OnLeftMouseButtonPress();
        }

        /// <summary>
        /// Called when the panel is left clicked
        /// </summary>
        protected virtual void OnLeftMouseButtonPress()
        {
        }
    }
}
