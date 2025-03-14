using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Village1 : BaseRoom
    {
        private Vector2I shimbleHouseAtlasCoords = new(0, 17);
        private Vector2I zenHouseAtlasCoords = new(21, 10);
        private TileMap tiles;

        public override void _Ready()
        {
            base._Ready();
            tiles = GetNode<TileMap>("TileMap");

            if (global.PlayerData.HasSolvedShimbleChair)
            {
                tiles.SetCell(1, new Vector2I(22, 22), 1, shimbleHouseAtlasCoords);
            }

            if (global.PlayerData.HasSolvedZenHouse)
            {
                tiles.SetCell(1, new Vector2I(24, 8), 1, zenHouseAtlasCoords);
            }
        }
    }
}
