using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;

public class BoardBehavior : MonoBehaviour {
	
	
    public GameObject Piece; // Actor (auto)
    public GameObject PieceAnimal; // Actor (auto)	
	
	public GameObject Tile;  // Hex
    public GameObject Line;  // Path obj
    public GameObject SelectionObject; // ring 
	
	public GameObject Building_Culture;
	public GameObject Building_Health;
	public GameObject Building_Science;
	
	
	public GameObject Actor_Humans;
	public GameObject Actor_Animals;
	
	public GameObject Environments_Tiles;
	
	public GameObject ParticlePrefab;
	public GameObject ParticlePrefab_Firework;
	
	public int Width;
	public int Height;
	
    public const float Spacing = 0.9f;

	GameObject[,] _gameBoard;
    public Game _game;
	List<GameObject> _path;
	
	#region GamePiece Init   TODO: re-write to using factory pattern 
	public List<GameObject> _gamePieces_Humans;
	public List<GameObject> _gamePieces_Animals;
	public List<GameObject> _gamePieces_Buildings;
	
	
	#endregion
	
	// Use this for initialization
	void Awake () {
	
		
		/*if (_gamePieces_Humans == null)
         	  _gamePieces_Humans = new List<GameObject>();
        _gamePieces_Humans.ForEach(Destroy);*/
		_gamePieces_Humans = new List<GameObject>();
		_gamePieces_Animals = new List<GameObject>();
		_gamePieces_Buildings = new List<GameObject>();		
		
		CreateBoard();
		 createHumanPiece();
		createHumanPiece();
		//createHumanPiece();
		/// createAnimalPiece(); // 4~7
		
		//createAnimalPiece();	
		createAnimalPiece();
		
		
        Camera.mainCamera.transform.position = new Vector3(Width / 2.0f * Spacing - Spacing, Height / 2.0f * Spacing - Spacing, -(Width + Height) / 2 - 5);		
        Messenger<TileBehaviour>.AddListener("Tile selected", OnTileSelected);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	

	
	#region Initialize GameBoard 
    private void CreateBoard()
	{
        _game = new Game(Width, Height);
        _gameBoard = new GameObject[Width, Height];
		float rnd;
		
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
		
				var tile = (GameObject)Instantiate(Tile);
				tile.transform.parent =  Environments_Tiles.transform;
				
				_gameBoard[x, y] = tile;
				var tileTransform = tile.transform;
				
                tileTransform.position = GetWorldCoordinates(x, y, 0);

				//var cylinder = tileTransform.Find("Cylinder");
                //var tb = (TileBehaviour)cylinder.GetComponent("TileBehaviour");
				var cylinder = tile;
                var tb = (TileBehaviour)cylinder.GetComponent("TileBehaviour"); // script for manage each tile
				
				// probability
				rnd = Random.value;				
				tb.Tile = _game.GameBoard[x, y];
				
				
				tb.Tile.TileStatus = TILE_STATUS.TILE_STATUS_TERRIAN;
				
				if(rnd < 0.4)
				{
					tb.Tile.CanPass = true;
					tb.Tile.TerrianType = TERRIAN_STATUS.GROUND_GRASS;
				}				
				else
				if(rnd < 0.7)
				{
					tb.Tile.CanPass = true;
					tb.Tile.TerrianType = TERRIAN_STATUS.GROUND_FLOODPLAN;
				}
				else if(rnd < 0.8)
				{
					tb.Tile.CanPass = true; // human can get resource from here
					tb.Tile.TerrianType = TERRIAN_STATUS.MINE;
					
				}
				else if(rnd < 0.85)
				{
					tb.Tile.CanPass = false;
					tb.Tile.TerrianType = TERRIAN_STATUS.FOREST_GROUND;
				}				
				else if(rnd < 0.9)
				{
					tb.Tile.CanPass = false;
					tb.Tile.TerrianType = TERRIAN_STATUS.FOREST;
				}
				else if(rnd < 0.95)
				{
					tb.Tile.CanPass = false;
					tb.Tile.TerrianType = TERRIAN_STATUS.MOUNTAIN;
				}
				else					
				{
					tb.Tile.CanPass = false;
					tb.Tile.TerrianType = TERRIAN_STATUS.WATER;
				}
				
                tb.SetMaterial();				
				//Debug.Log("("+x+","+y+")" );
			}
		}		
		
		/// Todo : assign the Game Attribute
		
	}
	#endregion
	
	
	
	#region Init Human
	
	Point GetAvailablePosition()
	{
			int nRandPosition_X = 0 ;
			int nRandPosition_Y = 0 ;
			//Width;
			//Height;
		
		bool bSelected = false;
		while( !bSelected )
		{
			nRandPosition_X =  UnityEngine.Random.Range(0 , Width);
			nRandPosition_Y =  UnityEngine.Random.Range(0 , Height);
			
			
			Tile TileSelect = _game.AllTiles.Single(o => o.X == nRandPosition_X && o.Y == nRandPosition_Y); // get the tile 
			
			if( TileSelect.CanPass == true  && TileSelect.Tile_StandStatus == TILE_STAND_STATUS.TILE_STAND_STATUS_NONE)
			{
				bSelected = true;
			}
		}
		
		return new Point( nRandPosition_X , nRandPosition_Y);
	}
	
	
	
	public void createHumanPiece()
	{		
		// Todo:  check the position
		//var startPiece = new GamePiece(new Point(2, 2)); // Position provide by
		var startPiece = new GamePiece(GetAvailablePosition()  ); // Position provide by
		GameObject NewHuman = CreatePiece_Human(startPiece, Color.blue);
		NewHuman.transform.parent = Actor_Humans.transform ;
		NewHuman.name = "Human_Adam";
		//Actor_Humans
		_gamePieces_Humans.Add(NewHuman);
		
	}
	
	
    private GameObject CreatePiece_Human(GamePiece piece, Color colour)
    {
        var visualPiece = (GameObject)Instantiate(Piece);
        visualPiece.transform.position = GetWorldCoordinates(piece.X, piece.Y, 0f);
        //var mat = new Material(Shader.Find(" Glossy")) {color = colour};
        //visualPiece.renderer.material = mat;

        var pb = (HumanBehaviour)visualPiece.GetComponent("HumanBehaviour");
        pb.Piece = piece;

        return visualPiece;
    }	

	public void KillHumanPiece(Tile tile)
	{
		//if( tileBehaviour.Tile.Tile_StandStatus == TILE_STAND_STATUS.TILE_STAND_STATUS_HUMAN)
		{
			GameObject human=
			_gamePieces_Humans.Find( o => ( (HumanBehaviour)o.GetComponent("HumanBehaviour")).Piece.X == tile.X  && ( (HumanBehaviour)o.GetComponent("HumanBehaviour")).Piece.Y ==tile.Y );
			_gamePieces_Humans.Remove(human);

			if(human == null)
				Debug.LogError( "Logic Error-destroy human object");
			else
				Destroy(human);

			// update the tile stand status
			tile.Tile_StandStatus = TILE_STAND_STATUS.TILE_STAND_STATUS_NONE;
			
			
			if ( _gamePieces_Humans.Count ==0)
				createHumanPiece();			
			
		}
	}

	public void createAnimalPiece()
	{
		var startPiece = new GamePiece(GetAvailablePosition()  ); // Position provide by
		GameObject NewAnimal = CreatePiece_Animal(startPiece, Color.blue);
		NewAnimal.transform.parent = Actor_Animals.transform ;
		NewAnimal.name = "NewAnimal_Luna";
		//Actor_Humans
		_gamePieces_Animals.Add(NewAnimal);
	}	
	
    private GameObject CreatePiece_Animal(GamePiece piece, Color colour)
    {
        var visualPiece = (GameObject)Instantiate(PieceAnimal );
        visualPiece.transform.position = GetWorldCoordinates(piece.X, piece.Y, 0f);
        //var mat = new Material(Shader.Find(" Glossy")) {color = colour};
        //visualPiece.renderer.material = mat;

        var pb = (AnimalBehaviour)visualPiece.GetComponent("AnimalBehaviour");
        pb.Piece = piece;

        return visualPiece;
    }		
	
	
	public void KillAnimalPiece(Tile tile)
	{
		//if( tile.Tile_StandStatus == TILE_STAND_STATUS.TILE_STAND_STATUS_ANIMAL)
		{
			GameObject animal=
			_gamePieces_Animals.Find( o => ( (AnimalBehaviour)o.GetComponent("AnimalBehaviour")).Piece.X == tile.X  && ( (AnimalBehaviour)o.GetComponent("AnimalBehaviour")).Piece.Y == tile.Y );
			_gamePieces_Animals.Remove(animal);

			if(animal == null)
				Debug.LogError( "Logic Error-destroy Animal object");
			else
				Destroy(animal);

			// update the tile stand status
			tile.Tile_StandStatus = TILE_STAND_STATUS.TILE_STAND_STATUS_NONE;
			
			
			if(  _gamePieces_Animals.Count ==0 )
				createAnimalPiece();			
		}
	}	
	
	
	
	#endregion
	
	
	#region Global utility functions
    static public Vector3 GetWorldCoordinates(int x, int y, float z)
    {
        var yOffset = x % 2 == 0 ? 0 : -Spacing / 2;
        return new Vector3(x * Spacing, y * Spacing + yOffset, -z);
    }
	#endregion
	

	
	#region Customized OnEvent
	
    void OnTileSelected(TileBehaviour tileBehaviour)
    {
		TileChanged(tileBehaviour);
		
		/*
        if (_selectedPiece == null)
            TileChanged(tileBehaviour);
        else
            MovePiece(tileBehaviour);
		*/
    }	
	#endregion
	
	public enum FINGER_BEHAVIOUR{
		FINGER_BEHAVIOUR_KILLANIMAL,
		FINGER_BEHAVIOUR_BUILDING_UPGRADE,
		FINGER_BEHAVIOUR_BUILDING_DESTROY,
		FINGER_BEHAVIOUR_TERRIAN_CHANGE,
		FINGER_BEHAVIOUR_COUNT
	};
	
    void TileChanged(TileBehaviour tileBehaviour)
    {
		// kill animal if animal exist
		Debug.Log("TileChanged-"+ tileBehaviour.Tile.Tile_StandStatus+"("+tileBehaviour.Tile.X+"," + tileBehaviour.Tile.Y+")");
		
		// chance
		
		//tileBehaviour.Tile.CanPass;
		//tileBehaviour.Tile.TerrianType;
		//tileBehaviour.Tile.TileStatus;
		//tileBehaviour.Tile.Tile_StandStatus;		
		
		//GameObject newParticle = (GameObject)Instantiate(  ParticlePrefab , hit.point , Camera.main.transform.rotation);
		
		//int nRandomChoose =  UnityEngine.Random.Range(0 , 5);
		//Debug.Log( "CalculateGameLogic:"+ Piece.X + ","+Piece.Y +"neib # "+AvailableTiles.Count+",choose:"+ nRandomChoose );
			
//GameObject newParticle = (GameObject)Instantiate(  ParticlePrefab , hit.point , Camera.main.transform.rotation);		
		
		
		if( tileBehaviour.Tile.Tile_StandStatus == TILE_STAND_STATUS.TILE_STAND_STATUS_HUMAN)
		{
			
			//KillHumanPiece( tileBehaviour);
			
			//if ( _gamePieces_Humans.Count ==0)
			//	createHumanPiece();
		}
		else
		if( tileBehaviour.Tile.Tile_StandStatus == TILE_STAND_STATUS.TILE_STAND_STATUS_ANIMAL)
		{
			// kill animal option is available
			KillAnimalPiece( tileBehaviour.Tile);
			

			
		}
		else
		if( tileBehaviour.Tile.TileStatus == TILE_STATUS.TILE_STATUS_BUILDING)
		{
			/* // Debug
			//foreach ( GameObject o in _gamePieces_Buildings)
			//{
			//	int nX = ( (BuildingBehavior)o.GetComponent("BuildingBehavior")).Piece.X;
			//	int nY = ( (BuildingBehavior)o.GetComponent("BuildingBehavior")).Piece.Y;
			//	Debug.Log( "all buildingLocation-"+nX+","+ nY+";");
			//}*/
			
			// make sure the building is on top  , get it by name ?
			GameObject Building =
				_gamePieces_Buildings.Find( o => ( (BuildingBehavior)o.GetComponent("BuildingBehavior")).Piece.X == tileBehaviour.Tile.X  && ( (BuildingBehavior)o.GetComponent("BuildingBehavior")).Piece.Y == tileBehaviour.Tile.Y );
			//			GameObject BuildingList = GameObject.Find("Environments_Building").transform;
			
			if(Building == null)
				Debug.LogError( "Logic Error-get Building object");			
			
			int nRandom =  UnityEngine.Random.Range(0 , 2);
			Debug.Log( "Building-"+nRandom);
			
			// destroy
			if(nRandom == 1 )
			{
				/// building Destroy is available.
				_gamePieces_Buildings.Remove(Building);
				Destroy(Building)	;
				
				tileBehaviour.Tile.Tile_StandStatus = TILE_STAND_STATUS.TILE_STAND_STATUS_NONE;
				tileBehaviour.Tile.TileStatus = TILE_STATUS.TILE_STATUS_TERRIAN;
				//tileBehaviour.Tile.TerrianType								
			}
			else
			{
				// building Upgrade 
				//string BuildingLevel = 
				BuildingBehavior Building_info = (BuildingBehavior)Building.GetComponent("BuildingBehavior");
				if (Building_info.UI_StatusText.text== "Building_Level1")
					Building_info.UI_StatusText.text = "Building_Level2";
				
				//GameObject newParticle = (GameObject)Instantiate(  ParticlePrefab_Firework , GetWorldCoordinates( tileBehaviour.Tile.X , tileBehaviour.Tile.Y,0)+ new Vector3(0f,0f,-0.5f)    , Quaternion.identity);
				Instantiate(  ParticlePrefab_Firework , GetWorldCoordinates( tileBehaviour.Tile.X , tileBehaviour.Tile.Y,0)+ new Vector3(0f,0f,-0.5f)    , Quaternion.identity);
			}

			
			
			
			
			
		}
		else
		if( tileBehaviour.Tile.TileStatus ==  TILE_STATUS.TILE_STATUS_TERRIAN && tileBehaviour.Tile.Tile_StandStatus == TILE_STAND_STATUS.TILE_STAND_STATUS_NONE)
		{
					
				// change the terrian
				
				/// Randon Event 
			if( tileBehaviour.Tile.CanPass == true)
			{
		        tileBehaviour.Tile.CanPass = false;
		     	tileBehaviour.Tile.TerrianType = TERRIAN_STATUS.MOUNTAIN;
				//tileBehaviour.Tile.Tile_StandStatus  ;
			}
			else
			{
				tileBehaviour.Tile.CanPass = true;
				tileBehaviour.Tile.TerrianType = TERRIAN_STATUS.GROUND_GRASS;
			}
				tileBehaviour.Tile.TileStatus = TILE_STATUS.TILE_STATUS_TERRIAN;
				tileBehaviour.SetMaterial();
		     //   OnGameStateChanged();		
			// 
			
			Instantiate(  ParticlePrefab , GetWorldCoordinates( tileBehaviour.Tile.X , tileBehaviour.Tile.Y,0)+ new Vector3(0f,0f,-0.5f)    , Quaternion.identity);
		}
		
		
		
		

    }	
	
	
	
}
