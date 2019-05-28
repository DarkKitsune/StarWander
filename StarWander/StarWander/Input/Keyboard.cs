/*
using System;
using OpenTK.Input;

namespace StarWander.Input
{
    public static class Keyboard
    {
        private static KeyboardState KeyboardState;

        /// <summary>
        /// Invoked when the left mouse button is pressed; return true to cancel the event for further subscribed delegates
        /// </summary>
        public static event Func<Key, bool> KeyPress;

        /// <summary>
        /// Invoked when the left mouse button is released; return true to cancel the event for further subscribed delegates
        /// </summary>
        public static event Func<Key, bool> KeyRelease;


        public static void Update()
        {
            var oldKeyboardState = KeyboardState;
            KeyboardState = OpenTK.Input.Keyboard.GetState(0);
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (KeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key))
                {
                    foreach (var dele in KeyPress?.GetInvocationList() ?? new Delegate[] { })
                    {
                        if ((bool)dele.DynamicInvoke(key))
                            break;
                    }
                }
                if (KeyboardState.IsKeyUp(key) && oldKeyboardState.IsKeyDown(key))
                {
                    foreach (var dele in KeyRelease?.GetInvocationList() ?? new Delegate[] { })
                    {
                        if ((bool)dele.DynamicInvoke(key))
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Check if a key is pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyPressed(Key key)
        {
            return KeyboardState.IsKeyDown(key);
        }
    }
}
*/