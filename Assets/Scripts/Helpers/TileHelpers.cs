using System.Linq;
using Board;
using Hex;

namespace Helpers
{
    public static class TileHelpers
    {
        public static bool IsProblematicTile(Tile tile)
        {
            if (!tile.HasNumber) return false;
            
            return tile.HasRedNumber() && tile.NeighbourHasRedNumber();
        }

        public static bool IsSafeTile(Tile tile)
        {
            if (!tile.HasNumber) return false;
            
            return !tile.HasRedNumber() && !tile.NeighbourHasRedNumber();
        }
        
        public static bool NeighbourHasRedNumber(this Tile tile)
        {
            return tile.Neighbours.Any(n => n.HasRedNumber());
        }
        
        public static int GetNumber(this Tile tile)
        {
            return !tile.HasNumber ? 0 : tile.Number.Value;
        }

        public static bool HasRedNumber(this Tile tile)
        {
            if (!tile.HasNumber) return false;
            
            return tile.GetNumber() is HexGrid.ProblematicNumber1 or HexGrid.ProblematicNumber2;
        }
    }
}