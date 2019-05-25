using System;
using System.Linq;
using VulpineLib.Util;

namespace StarWander.GFX
{
    public class GraphicList : INameable
    {
        /// <summary>
        /// Name of the object
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The sprite renderer
        /// </summary>
        private SpriteRenderer SpriteRenderer;

        /// <summary>
        /// Graphics
        /// </summary>
        public Graphic[] Graphics { get; private set; }

        /// <summary>
        /// Don't use!
        /// </summary>
        private GraphicList()
        {
            Name = "";
            SpriteRenderer = new SpriteRenderer($"");
            Graphics = new Graphic[] { };
        }

        /// <summary>
        /// Construct a GraphicsList
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        public GraphicList(string name, int size)
        {
            Name = name;
            SpriteRenderer = new SpriteRenderer($"{name}.{nameof(SpriteRenderer)}");
            Graphics = new Graphic[size];
        }

        /// <summary>
        /// Update the graphics in the GraphicList
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(double deltaTime)
        {
            for (var i = 0; i < Graphics.Length; i++)
            {
                var graphic = Graphics[i];
                if (!graphic.Initialized)
                    continue;
                graphic.Update(deltaTime);
                Graphics[i] = graphic;
            }
        }

        /// <summary>
        /// Draw the GraphicList
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Draw(double deltaTime)
        {
            SpriteRenderer.Begin();
            foreach (var graphic in Graphics.Where(e => e.Visible))
            {
                graphic.Draw(SpriteRenderer);
            }
            SpriteRenderer.End();
        }

        /// <summary>
        /// The index of the last graphic added
        /// </summary>
        private int LastAddIndex;

        /// <summary>
        /// Attempt to add a graphic to the list, and return the index
        /// </summary>
        /// <param name="graphic"></param>
        /// <returns></returns>
        public int? AddGraphic(Graphic graphic)
        {
            var index = Math.Min(LastAddIndex, Graphics.Length - 1);
            for (var i = 0; i < Graphics.Length; i++)
            {
                if (!Graphics[index].Initialized)
                {
                    Graphics[index] = graphic;
                    LastAddIndex = index;
                    return index;
                }
                index++;
                if (index == Graphics.Length)
                    index = 0;
            }
            return default;
        }

        /// <summary>
        /// Remove a graphic from the list
        /// </summary>
        /// <param name="index"></param>
        public void RemoveGraphic(int index)
        {
            if (index < 0 || index > Graphics.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the range of the GraphicsList");

            Graphics[index] = default;
        }
    }
}
