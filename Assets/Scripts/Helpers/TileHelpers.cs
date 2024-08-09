using System.Linq;
using Board;
using Board.Generation;
using Board.Pieces;
using Hex;

namespace Helpers
{
    public static class TileHelpers
    {
        /// <summary>
        /// Checks if the tile's number is 6 or 8 while also having a neighbour with any of the two numbers
        /// </summary>
        /// <param name="tile">Tile to check</param>
        /// <returns>True if tile is problematic</returns>
        public static bool IsProblematicTile(Tile tile)
        {
            if (!tile.HasNumber) return false;
            
            return tile.HasRedNumber() && tile.NeighbourHasRedNumber();
        }

        /// <summary>
        /// Checks if the tile and its neighbours do not have a 6 or 8
        /// </summary>
        /// <param name="tile">Tile to check</param>
        /// <returns>True if tile is safe</returns>
        public static bool IsSafeTile(Tile tile)
        {
            if (!tile.HasNumber) return false;
            
            return !tile.HasRedNumber() && !tile.NeighbourHasRedNumber();
        }

        /// <summary>
        /// Checks if any of the tile's neighbours have a 6 or 8
        /// </summary>
        /// <param name="tile">Tile to check</param>
        /// <returns>True if any neighbours have a 6 or 8</returns>
        private static bool NeighbourHasRedNumber(this Tile tile)
        {
            return tile.Neighbours.Any(n => n.HasRedNumber());
        }

        /// <summary>
        /// Gets the number of the tile
        /// </summary>
        /// <returns>The tile's number, and 0 if the tile has no number</returns>
        public static int GetNumber(this Tile tile)
        {
            return !tile.HasNumber ? 0 : tile.Number.Value;
        }

        /// <summary>
        /// Checks if the tile has a 6 or 8
        /// </summary>
        /// <param name="tile">Tile to check</param>
        /// <returns>True if the tile has a 6 or 8</returns>
        private static bool HasRedNumber(this Tile tile)
        {
            if (!tile.HasNumber) return false;
            
            return tile.GetNumber() is BoardGenerator.ProblematicNumber1 or BoardGenerator.ProblematicNumber2;
        }
        
        /// <summary>
        /// Check if swapping two tiles would create a conflict
        /// </summary>
        public static bool WouldCreateConflict(Tile tile1, Tile tile2)
        {
            // Temporarily swap numbers
            var tempNumber = tile1.Number;
            tile1.AssignNumber(tile2.Number);
            tile2.AssignNumber(tempNumber);

            // Check for conflicts
            var conflict = (tile1.HasRedNumber() && tile1.NeighbourHasRedNumber()) || (tile2.HasRedNumber() && tile2.NeighbourHasRedNumber());

            // Swap back
            tile2.AssignNumber(tile1.Number);
            tile1.AssignNumber(tempNumber);

            return conflict;
        }
        
        /// <summary>
        /// Swap numbers between two tiles
        /// </summary>
        public static void SwapNumbers(Tile tile1, Tile tile2)
        {
            var tempNumber = tile1.Number;
            tile1.AssignNumber(tile2.Number);
            tile2.AssignNumber(tempNumber);
        }

        /// <summary>
        /// Adds tile neighbours to the tile's neighbour list
        /// </summary>
        public static void AddTileNeighbours(this Tile tile)
        {
            foreach (var corner in tile.Data.Corners)
            foreach (var neighbour in corner.AdjacentTiles)
            {
                if (neighbour == tile) continue;

                tile.AddNeighbour(neighbour);
            }
        }
    }
}