using System.Collections.Generic;

namespace Chess.Pieces
{
    public class RookManager : PieceManager
    {
        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix)
        {
            List<Cell> cells = new List<Cell>();
            List<Coord> coords = GenerateCoords(matrix);

            // TODO

            return cells;
        }

        private List<Coord> GenerateCoords(Cell[,] matrix)
        {
            List<Coord> coords = new List<Coord>();

            Coord activeCoord = ActiveCell.coord;
            Coord coord;
            
            coord = activeCoord;

            while (coord.x >= 0)
            {
                --coord.x;

                coords.Add(new Coord
                {
                    x = coord.x,
                    y = coord.y
                });
            }

            coord = activeCoord;

            while (coord.x <= (maxColumnIdx - 1))
            {
                ++coord.x;
                
                coords.Add(new Coord
                {
                    x = coord.x,
                    y = coord.y
                });
            }

            coord = activeCoord;

            while (coord.y >= 0)
            {
                --coord.y;
                
                coords.Add(new Coord
                {
                    x = coord.x,
                    y = coord.y
                });
            }

            coord = activeCoord;

            while (coord.y <= (maxRowIdx - 1))
            {
                ++coord.y;
                
                coords.Add(new Coord
                {
                    x = coord.x,
                    y = coord.y
                });
            }

            return coords;
        }
    
        private bool HasLineOfSightToCoord(Coord coord)
        {
            // TODO
            
            return false;
        }
    }
}