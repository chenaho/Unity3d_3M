using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind;

namespace Model
{
	public enum TERRIAN_STATUS {FOREST,FOREST_GROUND,GROUND_FLOODPLAN,GROUND_GRASS,MINE,MOUNTAIN,WATER,TERRIAN_STATUS_COUNT };
	
	public enum TILE_STATUS
	{
		TILE_STATUS_NONE,
		TILE_STATUS_BUILDING,
		TILE_STATUS_TERRIAN
	};
	
	
	public enum TILE_STAND_STATUS	
	{
		TILE_STAND_STATUS_NONE,
		TILE_STAND_STATUS_HUMAN,
		TILE_STAND_STATUS_ANIMAL,
		TILE_STAND_STATUS_BOTH, // Human and Animal => Battle 
	}
	
	
	
	
    public class Tile : SpacialObject, IHasNeighbours<Tile>
    {
        public Tile(int x, int y)
            : base(x, y)
        {
            CanPass = true;
			IsBattleField = false;
			TerrianType = TERRIAN_STATUS.FOREST;
        }

        public bool CanPass { get; set; }
		public bool IsBattleField { get; set; }
		
		// 
//		public TERRIAN_STATUS TerrianType{ get; set; }
		public TERRIAN_STATUS TerrianType{ get; set; }
		public TILE_STATUS TileStatus{ get; set; }
		public TILE_STAND_STATUS  Tile_StandStatus{ get; set; }
		

        public IEnumerable<Tile> AllNeighbours { get; set; }
        public IEnumerable<Tile> Neighbours { get { return AllNeighbours.Where(o => o.CanPass); } } // return the available Neighbours 
		public IEnumerable<Tile> NeighboursWithHuman { get { return AllNeighbours.Where(o => o.CanPass   || o.Tile_StandStatus==TILE_STAND_STATUS.TILE_STAND_STATUS_HUMAN   ); } } // return the available Neighbours 

        public void FindNeighbours(Tile[,] gameBoard)
        {
            var neighbours = new List<Tile>();

            var possibleExits = X % 2 == 0 ? EvenNeighbours : OddNeighbours;
			
			/// check all the neighbours 
            foreach (var vector in possibleExits)
            {
                var neighbourX = X + vector.X;
                var neighbourY = Y + vector.Y;

                if (neighbourX >= 0 && neighbourX < gameBoard.GetLength(0) && neighbourY >= 0 && neighbourY < gameBoard.GetLength(1))
                    neighbours.Add(gameBoard[neighbourX, neighbourY]);
            }

            AllNeighbours = neighbours;
        }

        public static List<Point> EvenNeighbours
        {
            get
            {
                return new List<Point>
                {
                    new Point(0, 1),
                    new Point(1, 1),
                    new Point(1, 0),
                    new Point(0, -1),
                    new Point(-1, 0),
                    new Point(-1, 1),
                };
            }
        }

        public static List<Point> OddNeighbours
        {
            get
            {
                return new List<Point>
                {
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, -1),
                    new Point(0, -1),
                    new Point(-1, 0),
                    new Point(-1, -1),
                };
            }
        }
    }
}