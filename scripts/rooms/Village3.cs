using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Village3 : BaseRoom
    {
        private Vector2I tavernAtlasCoords = new(21, 0);
        private TileMap tiles;

        public override void _Ready()
        {
            base._Ready();
            tiles = GetNode<TileMap>("TileMap");

            if (global.PlayerData.HasSolvedTavernGlitch)
            {
                tiles.SetCell(1, new Vector2I(15, 24), 1, tavernAtlasCoords);
            }
        }
    }

}