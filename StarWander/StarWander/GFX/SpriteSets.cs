using System.IO;

namespace StarWander.GFX
{
    public static class SpriteSets
    {
        public static SpriteSet Tiles { get; private set; }
        public static SpriteSet Editor { get; private set; }
        public static SpriteSet Player { get; private set; }
        public static SpriteSet Common { get; private set; }

        public static void Init()
        {
            Tiles = SpriteSet.FromDirectory("Tiles Sprite Set", Path.Combine(Content.Root, "spritesets", "tiles"), true, true);
            Editor = SpriteSet.FromDirectory("Editor Sprite Set", Path.Combine(Content.Root, "spritesets", "editor"), true, false);
            Player = SpriteSet.FromDirectory("Player Sprite Set", Path.Combine(Content.Root, "spritesets", "player"), true, false);
            Common = SpriteSet.FromDirectory("Common Sprite Set", Path.Combine(Content.Root, "spritesets", "common"), true, false);
        }
    }
}
