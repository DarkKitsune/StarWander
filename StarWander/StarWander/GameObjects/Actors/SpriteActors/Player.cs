using VulpineLib.Util;
using StarWander.GFX;

namespace StarWander.GameObjects.Actors.SpriteActors
{
    public class Player : SpriteActor
    {
        public Player(Region region, Vector3<decimal> position)
            : base(region, position, Team.Player, 500.0, SpriteSets.Player["Idle"])
        {
            CollisionHull.AddShape(new CollisionShape(Vector3<float>.Zero, 0.5f));
            HasShadow = true;
            ShadowSize = 0.6f;
            ShadowAlpha = 0.6f;
        }
    }
}
