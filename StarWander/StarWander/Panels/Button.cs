using System;
using OpenTK;
using VulpineLib.Util;
using StarWander.GFX;

namespace StarWander.Panels
{
    public class Button : Control
    {
        public Button(string name, Vector2<int> leftTop, Vector2<int> size) : base(name, leftTop, size)
        {
        }

        protected override void OnDraw(Panel panel, SpriteRenderer spriteRenderer)
        {
            spriteRenderer.DrawSprite(
                    SpriteSets.Editor["Button"],
                    new VulpineLib.Geometry.Rectangle(LeftTop.To<float>(), Size.To<float>()),
                    VulpineLib.Util.Color.White,
                    BlendType.AlphaPremultiplied
                );
        }

        protected override void OnLeftMouseButtonPress()
        {
            Log.Info(Name, Name);
        }
    }
}
