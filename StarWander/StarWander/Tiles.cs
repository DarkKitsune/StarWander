using System.Collections.Generic;
using StarWander.GFX;

namespace StarWander
{
    public enum Tile
    {
        None,
        Stone,
    }

    public static class TileExtensions
    {
        public static SpriteSet SpriteSet => SpriteSets.Tiles;

        public struct TileData
        {
            public bool Invisible;
            public bool Solid;
            public bool Translucent;
            public string? Sprite;

            public bool DoesNotBlockFaces => Invisible || Translucent;
        }

        private static Dictionary<Tile, TileData> TileDataDict = new Dictionary<Tile, TileData>()
        {
            { Tile.None, new TileData   { Invisible = true,     Solid = false,  Translucent = true,     Sprite = null, } },
            { Tile.Stone, new TileData  { Invisible = false,    Solid = true,   Translucent = false,    Sprite = "Dirt", } },
        };

        public static TileData Data(this Tile tile)
        {
            return TileDataDict[tile];
        }
    }
}
