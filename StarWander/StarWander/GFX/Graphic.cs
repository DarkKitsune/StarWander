using System;
using VulpineLib.Animation;
using VulpineLib.Util;

namespace StarWander.GFX
{
    /// <summary>
    /// Struct representing a graphic object
    /// </summary>
    public struct Graphic
    {
        /// <summary>
        /// Is the graphic valid
        /// </summary>
        public bool Initialized { get; private set; }

        /// <summary>
        /// Is the graphic visible
        /// </summary>
        public bool Visible;

        /// <summary>
        /// The animation player
        /// </summary>
        public AnimationPlayer AnimationPlayer;

        /// <summary>
        /// The graphic's image
        /// </summary>
        public Sprite? Sprite { get; private set; }

        /// <summary>
        /// Graphic position
        /// </summary>
        public Vector3<float> Position;

        /// <summary>
        /// Graphic size
        /// </summary>
        public Vector2<float> Size;

        /// <summary>
        /// Graphic color
        /// </summary>
        public Color Color;

        /// <summary>
        /// Graphic blend type
        /// </summary>
        public BlendType BlendType;

        /// <summary>
        /// Construct a graphic object
        /// </summary>
        /// <param name="animation"></param>
        public Graphic(Vector3<float> position, Vector2<float> size, Animation? animation)
        {
            Initialized = true;
            Visible = true;
            Position = position;
            Size = size;
            Color = Color.White;
            BlendType = BlendType.AlphaPremultiplied;
            AnimationPlayer = new AnimationPlayer(animation);
            Sprite = null;
        }

        /// <summary>
        /// Update graphic
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(double deltaTime)
        {
            foreach (var iinst in AnimationPlayer.Play(deltaTime))
            {
                if (iinst is GraphicInstruction graphicInst)
                {
                    switch (graphicInst.Code)
                    {
                        case GraphicInstruction.InstructionCode.SetSprite:
                            Sprite = graphicInst.Args[0] as Sprite;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Draw graphic
        /// </summary>
        /// <param name="renderer"></param>
        public void Draw(SpriteRenderer renderer)
        {
            if (Sprite is null)
                return;
            renderer.DrawSprite(Sprite, Position, Size, Color, BlendType);
        }
    }
}
