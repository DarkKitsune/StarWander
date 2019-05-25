using System;
using System.Collections.Generic;
using VulpineLib.Util;
using OpenTK.Graphics.OpenGL4;
using StarWander.GFX;

namespace StarWander.Panels
{
    /// <summary>
    /// Control class to be extended for panel controls
    /// </summary>
    public class Control : INameable
    {
        public string Name { get; private set; }

        public Vector2<int> LeftTop;
        public Vector2<int> Size;

        public Vector2<int> LocalMousePosition => new Vector2<int>(
                Input.Mouse.Position.X - LeftTop.X,
                Input.Mouse.Position.Y - LeftTop.Y
            );

        public bool MouseOver => LocalMousePosition.BetweenOrEqual(Vector2<int>.Zero, Size - 1);

        /// <summary>
        /// Don't allow empty constructor
        /// </summary>
        private Control()
        {
            Name = "";
        }

        /// <summary>
        /// Construct a control
        /// </summary>
        /// <param name="leftTop"></param>
        /// <param name="size"></param>
        protected Control(string name, Vector2<int> leftTop, Vector2<int> size)
        {
            Name = name;
            LeftTop = leftTop;
            Size = size;
        }

        /// <summary>
        /// Simulate a left click
        /// </summary>
        public void LeftMouseButtonPress()
        {
            OnLeftMouseButtonPress();
        }

        /// <summary>
        /// Called when the control is left clicked
        /// </summary>
        protected virtual void OnLeftMouseButtonPress()
        {
        }

        /// <summary>
        /// Draw this panel
        /// </summary>
        /// <param name="spriteRenderer"></param>
        public void Draw(Panel panel, SpriteRenderer spriteRenderer)
        {
            OnDraw(panel, spriteRenderer);
        }

        /// <summary>
        /// Called when the panel is drawn
        /// </summary>
        /// <param name="spriteRenderer"></param>
        protected virtual void OnDraw(Panel panel, SpriteRenderer spriteRenderer)
        {
        }
    }
}
