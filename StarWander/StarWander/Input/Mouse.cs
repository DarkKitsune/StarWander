using System;
using VulpineLib.Util;
using OpenTK.Input;

namespace StarWander.Input
{
    public static class Mouse
    {
        private static MouseState MouseState;
        public static ButtonState LeftButton => MouseState.LeftButton;
        public static ButtonState RightButton => MouseState.RightButton;
        /// <summary>
        /// Invoked when the left mouse button is pressed; return true to cancel the event for further subscribed delegates
        /// </summary>
        public static event Func<bool> LeftPress;

        /// <summary>
        /// Invoked when the left mouse button is released; return true to cancel the event for further subscribed delegates
        /// </summary>
        public static event Func<bool> LeftRelease;

        /// <summary>
        /// Invoked when the right mouse button is pressed; return true to cancel the event for further subscribed delegates
        /// </summary>
        public static event Func<bool> RightPress;

        /// <summary>
        /// Invoked when the right mouse button is released; return true to cancel the event for further subscribed delegates
        /// </summary>
        public static event Func<bool> RightRelease;

        public static Vector2<int> Position
        {
            get
            {
                if (SGameWindow.Main == null)
                    throw new NullReferenceException();
                var localPoint = SGameWindow.Main.PointToClient(new System.Drawing.Point(MouseState.X, MouseState.Y));
                return new Vector2<int>(localPoint.X, localPoint.Y);
            }
        }

        public static Vector2<float> PositionFraction
        {
            get
            {
                if (SGameWindow.Main == null)
                    throw new NullReferenceException();
                var localPoint = SGameWindow.Main.PointToClient(new System.Drawing.Point(MouseState.X, MouseState.Y));
                return new Vector2<float>(
                        localPoint.X / (float)SGameWindow.Main.ClientSize.Width,
                        localPoint.Y / (float)SGameWindow.Main.ClientSize.Height
                    );
            }
        }

        public static void Update()
        {
            var oldMouseState = MouseState;
            MouseState = OpenTK.Input.Mouse.GetCursorState();

            if (MouseState.LeftButton != oldMouseState.LeftButton)
            {
                if (MouseState.LeftButton == ButtonState.Pressed)
                {
                    foreach (var dele in LeftPress?.GetInvocationList() ?? new Delegate[] { })
                    {
                        if ((bool)dele.DynamicInvoke())
                            break;
                    }
                }
                else
                {
                    foreach (var dele in LeftRelease?.GetInvocationList() ?? new Delegate[] { })
                    {
                        if ((bool)dele.DynamicInvoke())
                            break;
                    }
                }
            }
            if (MouseState.RightButton != oldMouseState.RightButton)
            {
                if (MouseState.RightButton == ButtonState.Pressed)
                {
                    foreach (var dele in RightPress?.GetInvocationList() ?? new Delegate[] { })
                    {
                        if ((bool)dele.DynamicInvoke())
                            break;
                    }
                }
                else
                {
                    foreach (var dele in RightRelease?.GetInvocationList() ?? new Delegate[] { })
                    {
                        if ((bool)dele.DynamicInvoke())
                            break;
                    }
                }
            }
        }
    }
}
