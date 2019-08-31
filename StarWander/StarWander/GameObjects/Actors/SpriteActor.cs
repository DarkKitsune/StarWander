using System;
using VulpineLib.Util;
using StarWander.GFX;
using OpenTK.Graphics.OpenGL4;

namespace StarWander.GameObjects.Actors
{
    public class SpriteActor : Actor
    {
        internal static SpriteRenderer? SpriteRenderer { get; private set; }
        public Sprite? ShadowSprite => SpriteSets.Common["Shadow"];

        public static void Init()
        {
            SpriteRenderer = new SpriteRenderer($"{nameof(SpriteActor)}.{nameof(SpriteRenderer)}");
        }

        public Sprite? Sprite;
        public bool HasShadow = true;
        public float ShadowZInRegion = 0f;
        public float ShadowSize = 1f;
        public float ShadowAlpha = 0.5f;

        public SpriteActor(Region region, Vector3<decimal> position, Team team, double health, Sprite? sprite = null)
            : base(region, position, team, health)
        {
            Sprite = sprite;
        }

        protected override void OnStart()
        {
        }

        protected override void OnEnd()
        {
        }

        protected override void OnDraw(double time, double delta)
        {
            base.OnDraw(time, delta);
            if (Sprite is null)
                return;
            if (SpriteRenderer is null)
                throw new NullReferenceException("SpriteRenderer is null");
            // Rotate
            Transform.Rotation = Quaternion<float>.CreateRotation(
                    -CameraObject.Current.Transform.Forward,
                    CameraObject.Current.Transform.Up
                );
            // Enable depth testing
            GL.Enable(EnableCap.DepthTest);
            // Start render
            SpriteRenderer.Begin();
            // Shadow
            GL.DepthMask(false);
            var mat = Transform.Matrix;
            if (HasShadow && !(ShadowSprite is null))
            {
                var trans = mat.Translation * new Vector3<float>(1, 1, 0) + new Vector3<float>(0, 0, ShadowZInRegion + 0.005f);
                mat = Matrix4<float>.CreateScale(new Vector3<float>(ShadowSize)) * Matrix4<float>.CreateTranslation(trans);
                SpriteRenderer.DrawSprite(ShadowSprite, ref mat, new Color(0f, 0f, 0f, ShadowAlpha, true), BlendType.AlphaPremultiplied);
            }
            // Main sprite
            GL.DepthMask(true);
            mat = Transform.Matrix;
            SpriteRenderer.DrawSprite(Sprite, ref mat, Color.White, BlendType.AlphaPremultiplied);
            // End render
            SpriteRenderer.End();
        }
    }
}
