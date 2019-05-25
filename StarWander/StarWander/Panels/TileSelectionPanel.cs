using System;
using VulpineLib.Util;
using StarWander.GFX;

namespace StarWander.Panels
{
    public class TileSelectionPanel : Panel
    {
        const float ButtonHeightToPanelHeightRatio = 0.05f;

        private Color SelectedTileColor => Color.White;
        private Color HoveredTileColor => new Color(0.9f, 0.9f, 0.9f, 1f);
        private Color DefaultTileColor => new Color(0.75f, 0.75f, 0.75f, 1f);
        private float SelectedScaleMultiplier => 1.3f;

        /// <summary>
        /// The tile set
        /// </summary>
        public SpriteSet? TileSet => SpriteSets.Tiles;
        /// <summary>
        /// The number of columns to display tiles in
        /// </summary>
        public int TileColumns => 8;
        public Vector2<int> ContentLocalLeftTop => new Vector2<int>(Margin, Margin);
        public Vector2<int> ContentSize => Size - new Vector2<int>(Margin + Margin, Margin + Margin);
        /// <summary>
        /// The size of tiles in the panel
        /// </summary>
        public float TileSize => ContentSize.X / TileColumns;
        /// <summary>
        /// Which tile the mouse is hovering over
        /// </summary>
        private Sprite? HoveredTile;
        /// <summary>
        /// Which tile is selected
        /// </summary>
        public Sprite? SelectedTile { get; private set; }

        public TileSelectionPanel(int tileLayers) :
            base(
                    "Tile Selection Panel",
                    Vector2<int>.Zero,
                    SGameWindow.Main != null ?
                        new Vector2<int>((int)(SGameWindow.Main.ClientSize.Width * 0.175f), SGameWindow.Main.ClientSize.Height) :
                        Vector2<int>.Zero
                )
        {
            var buttonSize = new Vector2<int>(
                    (int)(Size.Y * ButtonHeightToPanelHeightRatio),
                    (int)(Size.Y * ButtonHeightToPanelHeightRatio)
                );
            for (var i = 0; i < tileLayers; i++)
                Controls.Add(new Button($"Layer {i} Button", new Vector2<int>(Margin + buttonSize.X * i, Margin), buttonSize));
        }

        protected override void OnDraw(SpriteRenderer spriteRenderer)
        {
            if (TileSet == null)
                throw new NullReferenceException();
            // Set HoveredTile to null because we do not yet know what tile is hovered over
            HoveredTile = null;
            // Draw selectable tiles (and find the tile hovered over, if any)
            var n = 0;
            foreach (var tile in TileSet.Collection)
            {
                // Calculate what row and column this tile should draw in
                int column = n % TileColumns;
                int row = n / TileColumns;
                // Turn the row and column into a position
                var tilePos = LeftTop.To<float>()
                    + ContentLocalLeftTop.To<float>()
                    + new Vector2<float>(0f, Size.Y * ButtonHeightToPanelHeightRatio)
                    + new Vector2<float>((column + 0.5f) * TileSize, (row + 0.5f) * TileSize);
                // Find if this tile has the mouse hovering over it
                var hovered = Input.Mouse.Position.To<float>().Between(tilePos - TileSize / 2f, tilePos + TileSize / 2f);
                // If the mouse is hovering over the tile then set HoveredTile to this tile
                if (hovered)
                    HoveredTile = tile;
                // Find if this tile is selected
                var selected = SelectedTile! == tile;
                // Draw the tile
                spriteRenderer.DrawSprite(
                        tile,
                        new Vector3<float>(tilePos, 0f),
                        new Vector2<float>(TileSize, TileSize) * (selected ? SelectedScaleMultiplier : 1f),
                        selected ? SelectedTileColor : (hovered ? HoveredTileColor : DefaultTileColor),
                        BlendType.AlphaPremultiplied
                    );
                n++;
            }
        }

        protected override void OnWindowResized(Vector2<int> size)
        {
            if (SGameWindow.Main == null)
                throw new NullReferenceException();
            Size = new Vector2<int>((int)(SGameWindow.Main.ClientSize.Width * 0.175f), SGameWindow.Main.ClientSize.Height);
            foreach (var control in Controls)
            {
                if (control is Button button)
                    button.Size.Y = (int)(Size.Y * ButtonHeightToPanelHeightRatio);
            }
        }

        protected override void OnLeftMouseButtonPress()
        {
            // Select the hovered tile, if any
            if (!(HoveredTile is null))
                SelectedTile = HoveredTile;
        }
    }
}
