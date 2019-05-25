using System;
using VulpineLib.Util;

namespace StarWander.Scenes
{
    public class SceneManager
    {
        /// <summary>
        /// The current scene, or, null if there isn't one
        /// </summary>
        public Scene? CurrentScene { get; private set; }

        /// <summary>
        /// End the current scene, if any, and start a scene
        /// </summary>
        /// <param name="scene"></param>
        public void StartScene(Scene scene, double time)
        {
            // End the current scene, if any
            if (CurrentScene != null)
                EndScene();
            // Start new scene
            Log.Info($"Starting scene \"{scene.Name}\"", nameof(SceneManager));
            CurrentScene = scene;
            CurrentScene.Start();
        }

        /// <summary>
        /// End the scene
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void EndScene()
        {
            // Throw an exception if there is no current scene
            if (CurrentScene == null)
                throw new InvalidOperationException("There is no currently active scene");
            // End the current scene, if any
            Log.Info($"Ending scene \"{CurrentScene.Name}\"", nameof(SceneManager));
            CurrentScene.End();
            CurrentScene = null;
        }

        /// <summary>
        /// Update the scene
        /// </summary>
        /// <param name="time"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Update(double time, double deltaTime)
        {
            // Throw an exception if there is no current scene
            if (CurrentScene == null)
                throw new InvalidOperationException("There is no currently active scene");
            CurrentScene.Update(time, deltaTime);
        }

        /// <summary>
        /// Draw the scene
        /// </summary>
        /// <param name="time"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Draw(double time, double deltaTime)
        {
            // Throw an exception if there is no current scene
            if (CurrentScene == null)
                throw new InvalidOperationException("There is no currently active scene");
            CurrentScene.Draw(time, deltaTime);
        }

        /// <summary>
        /// Inform the scene that the window has been resized
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void WindowResized()
        {
            // Throw an exception if there is no current scene
            if (CurrentScene == null)
                throw new InvalidOperationException("There is no currently active scene");
            CurrentScene.WindowResized();
        }
    }
}
