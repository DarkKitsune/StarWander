using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using VulpineLib.Util;

namespace StarWander.Input
{
    public static class GamePad
    {
        private static GamePadState GamePadState;

        /// <summary>
        /// The dead zone for analog inputs
        /// </summary>
        public static float Deadzone = 0.2f;

        /// <summary>
        /// Invoked when the left mouse button is pressed; return true to cancel the event for further subscribed delegates
        /// </summary>
        public static event Func<string, bool> ButtonPress;

        /// <summary>
        /// Invoked when the left mouse button is released; return true to cancel the event for further subscribed delegates
        /// </summary>
        public static event Func<string, bool> ButtonRelease;

        /// <summary>
        /// Names of available buttons
        /// </summary>
        public static IEnumerable<string> ButtonNames => typeof(GamePadButtons).GetProperties()
            .Where(e => e.PropertyType == typeof(ButtonState))
            .Select(e => e.Name);

        public static void Update()
        {
            var oldGamePadState = GamePadState;
            GamePadState = OpenTK.Input.GamePad.GetState(0);
            foreach (string button in ButtonNames)
            {
                if (GamePadState.IsButtonDown(button))
                {
                    if (!oldGamePadState.IsButtonDown(button))
                    {
                        foreach (var dele in ButtonPress?.GetInvocationList() ?? new Delegate[] { })
                        {
                            if ((bool)dele.DynamicInvoke(button))
                                break;
                        }
                    }
                }
                else if (oldGamePadState.IsButtonDown(button))
                {
                    foreach (var dele in ButtonRelease?.GetInvocationList() ?? new Delegate[] { })
                    {
                        if ((bool)dele.DynamicInvoke(button))
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Get the state of a button by name
        /// </summary>
        /// <param name="button"></param>
        /// <param name="gamePadState"></param>
        /// <returns></returns>
        private static bool IsButtonDown(this GamePadState gamePadState, string button)
        {
            var prop = typeof(GamePadButtons).GetProperty(button);
            if (prop is null)
                throw new ArgumentException($"Button {button} is not a valid button", nameof(button));
            return (ButtonState)prop.GetValue(gamePadState.Buttons) == ButtonState.Pressed;
        }

        /// <summary>
        /// Check if a button is pressed
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsButtonPressed(string button)
        {
            return GamePadState.IsButtonDown(button);
        }

        /// <summary>
        /// The left stick axis
        /// </summary>
        public static Vector2<float> LeftStick => new Vector2<float>(
                GamePadState.ThumbSticks.Left.X.FactorDeadzone(),
                GamePadState.ThumbSticks.Left.Y.FactorDeadzone()
            );

        /// <summary>
        /// The right stick axis
        /// </summary>
        public static Vector2<float> RightStick => new Vector2<float>(
                GamePadState.ThumbSticks.Right.X.FactorDeadzone(),
                GamePadState.ThumbSticks.Right.Y.FactorDeadzone()
            );

        /// <summary>
        /// The left trigger axis
        /// </summary>
        public static float LeftTrigger => GamePadState.Triggers.Left.FactorDeadzone();

        /// <summary>
        /// The right trigger axis
        /// </summary>
        public static float RightTrigger => GamePadState.Triggers.Right.FactorDeadzone();

        private static float FactorDeadzone(this float n)
        {
            if (n.EqualsSafe(0f, Deadzone))
                return 0f;
            return n;
        }
    }
}