using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using VulpineLib.Util;

namespace StarWander.Input
{
    public static class GamePad
    {
        private class KeyState
        {
            public Dictionary<Key, bool> States = new Dictionary<Key, bool>();

            public bool IsKeyDown(Key key)
            {
                if (States.TryGetValue(key, out var down))
                    return down;
                return false;
            }

            public bool IsKeyUp(Key key)
            {
                if (States.TryGetValue(key, out var down))
                    return !down;
                return true;
            }

            public void SetKeyState(Key key, bool down)
            {
                States[key] = down;
            }
        }

        private class InputStates
        {
            public GamePadState GamePadState;
            //public KeyboardState KeyboardState;
        }

        private static InputStates States;
        private static KeyState KeyboardState;

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

        public static Dictionary<string, Key> KeyboardMappings = new Dictionary<string, Key>
        {
            { "LeftTrigger", Key.S },
            { "RightTrigger", Key.W },
            { "LeftShoulder", Key.Q },
            { "RightShoulder", Key.E },
            { "LeftStickUp", Key.Space },
            { "LeftStickDown", Key.ShiftLeft },
            { "LeftStickLeft", Key.A },
            { "LeftStickRight", Key.D },
            { "RightStickUp", Key.Up },
            { "RightStickDown", Key.Down },
            { "RightStickLeft", Key.Left },
            { "RightStickRight", Key.Right }
        };

        public static int FirstPlayer { get; } = -1;

        static GamePad()
        {
            if (SGameWindow.Main is null)
                throw new NullReferenceException("Main window is null; don't use input until main window exists");
            SGameWindow.Main.KeyDown += Main_KeyDown;
            SGameWindow.Main.KeyUp += Main_KeyUp;
            KeyboardState = new KeyState();
            States = new InputStates
            {
                GamePadState = OpenTK.Input.GamePad.GetState(FirstPlayer),
            };
            for (var i = 0; i < 6; i++)
            {
                var name = OpenTK.Input.GamePad.GetName(i);
                if (!string.IsNullOrEmpty(name))
                {
                    if (FirstPlayer == -1)
                        FirstPlayer = i;
                    Console.WriteLine($"Gamepad: {i} {name}");
                }
            }
        }

        private static void Main_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            var key = e.Key;
            KeyboardState.SetKeyState(key, true);
            var button = KeyboardMappings.FirstOrDefault(buttonKey => buttonKey.Value == key).Key ?? key.KeyToString();
            foreach (var dele in ButtonPress?.GetInvocationList() ?? new Delegate[] { })
            {
                if ((bool)dele.DynamicInvoke(button))
                    break;
            }
        }

        private static void Main_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            var key = e.Key;
            KeyboardState.SetKeyState(key, false);
            var button = KeyboardMappings.FirstOrDefault(buttonKey => buttonKey.Value == key).Key ?? key.KeyToString();
            foreach (var dele in ButtonRelease?.GetInvocationList() ?? new Delegate[] { })
            {
                if ((bool)dele.DynamicInvoke(button))
                    break;
            }
        }

        public static void Update()
        {
            if (FirstPlayer == -1)
                return;
            var oldStates = States;
            States = new InputStates
            {
                GamePadState = OpenTK.Input.GamePad.GetState(FirstPlayer),
                //KeyboardState = Keyboard.GetState()
            };
            foreach (string button in ButtonNames)
            {
                if (States.IsButtonDown(button, true))
                {
                    if (!oldStates.IsButtonDown(button, true))
                    {
                        foreach (var dele in ButtonPress?.GetInvocationList() ?? new Delegate[] { })
                        {
                            if ((bool)dele.DynamicInvoke(button))
                                break;
                        }
                    }
                }
                else if (oldStates.IsButtonDown(button, true))
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
        private static bool IsButtonDown(this InputStates state, string button, bool gamepadOnly = false)
        {
            if (!gamepadOnly && KeyboardMappings.TryGetValue(button, out var key) && KeyboardState.IsKeyDown(key))
                return true;
            var prop = typeof(GamePadButtons).GetProperty(button);
            if (prop is null)
                throw new ArgumentException($"Button {button} is not a valid button", nameof(button));
            return (ButtonState)prop.GetValue(state.GamePadState.Buttons) == ButtonState.Pressed;
        }

        /// <summary>
        /// Check if a button is pressed
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsButtonPressed(string button)
        {
            return States.IsButtonDown(button);
        }

        /// <summary>
        /// The left stick axis
        /// </summary>
        public static Vector2<float> LeftStick
        {
            get
            {
                if (KeyboardMappings.TryGetValue("LeftStickRight", out var rightKey)
                    && KeyboardMappings.TryGetValue("LeftStickLeft", out var leftKey))
                {
                    var kx = (KeyboardState.IsKeyDown(rightKey) ? 1f : 0f)
                        - (KeyboardState.IsKeyDown(leftKey) ? 1f : 0f);
                    float ky = 0;
                    if (KeyboardMappings.TryGetValue("LeftStickUp", out var upKey)
                        && KeyboardMappings.TryGetValue("LeftStickDown", out var downKey))
                    {
                        ky = (KeyboardState.IsKeyDown(upKey) ? 1f : 0f)
                            - (KeyboardState.IsKeyDown(downKey) ? 1f : 0f);
                    }
                    var kv = new Vector2<float>(kx, ky);
                    if (kv.LengthSquared > 0.01f)
                        return kv.Normalized.To<float>();
                }
                return new Vector2<float>(
                    States.GamePadState.ThumbSticks.Left.X.FactorDeadzone(),
                    States.GamePadState.ThumbSticks.Left.Y.FactorDeadzone()
                );
            }
        }

        /// <summary>
        /// The right stick axis
        /// </summary>
        public static Vector2<float> RightStick
        {
            get
            {
                if (KeyboardMappings.TryGetValue("RightStickRight", out var rightKey)
                    && KeyboardMappings.TryGetValue("RightStickLeft", out var leftKey))
                {
                    var kx = (KeyboardState.IsKeyDown(rightKey) ? 1f : 0f)
                        - (KeyboardState.IsKeyDown(leftKey) ? 1f : 0f);
                    var ky = 0f;
                    if (KeyboardMappings.TryGetValue("RightStickUp", out var upKey)
                        && KeyboardMappings.TryGetValue("RightStickDown", out var downKey))
                    {
                        ky = (KeyboardState.IsKeyDown(upKey) ? 1f : 0f)
                            - (KeyboardState.IsKeyDown(downKey) ? 1f : 0f);
                    }
                    var kv = new Vector2<float>(kx, ky);
                    if (kv.LengthSquared > 0.01f)
                        return kv.Normalized.To<float>();
                }
                return new Vector2<float>(
                    States.GamePadState.ThumbSticks.Right.X.FactorDeadzone(),
                    States.GamePadState.ThumbSticks.Right.Y.FactorDeadzone()
                );
            }
        }

        /// <summary>
        /// The left trigger axis
        /// </summary>
        public static float LeftTrigger
        {
            get
            {
                if (KeyboardMappings.TryGetValue("LeftTrigger", out var key) && KeyboardState.IsKeyDown(key))
                    return 1f;
                return States.GamePadState.Triggers.Left.FactorDeadzone();
            }
        }

        /// <summary>
        /// The right trigger axis
        /// </summary>
        public static float RightTrigger
        {
            get
            {
                if (KeyboardMappings.TryGetValue("RightTrigger", out var key) && KeyboardState.IsKeyDown(key))
                    return 1f;
                return States.GamePadState.Triggers.Right.FactorDeadzone();
            }
        }

        private static float FactorDeadzone(this float n)
        {
            if (n.EqualsSafe(0f, Deadzone))
                return 0f;
            return n;
        }

        private static string KeyToString(this Key key)
        {
            var str = key.ToString();
            if (str.Length == 1)
                return $"Key{str}";
            return str;
        }
    }
}