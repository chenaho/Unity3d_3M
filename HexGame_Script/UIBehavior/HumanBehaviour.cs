using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Model;
using System.Linq;

public class HumanBehaviour : MonoBehaviour {
	
	
	// HumanType : normal People / Hero
	
	public GamePiece Piece;
	Vector3 Position_Destination;
	
	// TODO by using Message
	GameObject CoreGame;
	BoardBehavior BoardClass ;
	
	// UI
	GUIText  UI_StatusText;
	Vector3 TextCoords;
	
	bool  m_bDistroy;	
	
	public enum HUMAN_BEHAVIOUR 
	{
	 HUMAN_BEHAVIOUR_IDLE, // Do Nothing
	 HUMAN_BEHAVIOUR_GETRESOURCE, // Get Resource in Mine
	 HUMAN_BEHAVIOUR_FIGHT, // Fight with Animal
	 HUMAN_BEHAVIOUR_BUILD, // Building 
	 HUMAN_BEHAVIOUR_BUILD_UPGRADE, // upgrade the Building , do this only when world have enough culture point
	 HUMAN_BEHAVIOUR_BUILD_GREAT, // Building the world wonder , need more time
	 HUMAN_BEHAVIOUR_BUILD_USE, // generate the point 
	}; // In_building-> 

	
	public HUMAN_BEHAVIOUR m_HumanPiece_Behaviour_Prev;
	public HUMAN_BEHAVIOUR m_HumanPiece_Behaviour;
	
	
	
	
	
	public enum HUMAN_STATUS
	{
		HUMAN_STATUS_INIT,
		//HUMAN_STATUS_LOGIC,
		HUMAN_STATUS_MOVING,
		HUMAN_STATUS_BEHAVIOUR,
		HUMAN_STATUS_FINISH
	};
	
	int m_nCurrentFrame;
	public HUMAN_STATUS m_HumanStatus;
	
	// HUMAN Type: Normal , Elite
	// TODO : redifine as a game behavoir (component pattern)
	public enum HUMAN_TYPE
	{
		HUMAN_TYPE_NORMAL,
		HUMAN_TYPE_ELITE,
	}
	
	public HUMAN_TYPE m_HumanType;
	
	
	
	// Use this for initialization
	void Start () 
	{
		m_HumanType = HUMAN_TYPE.HUMAN_TYPE_NORMAL;
//		Piece.m_StrongValue=2;  //
		CoreGame = GameObject.Find("coreGame");
		BoardClass =    CoreGame.GetComponent("BoardBehavior") as  BoardBehavior;

		
		
		/// Init the TextUI
		GameObject go  = new GameObject("Text_"+ this.name);
		UI_StatusText = (GUIText)go.AddComponent( typeof(GUIText) );
		UI_StatusText.transform.parent = transform;

		Font BroadwayFont = (Font)Resources.Load("Fonts/Arial");
		UI_StatusText.font = BroadwayFont;
		UI_StatusText.text = "Init";
		UI_StatusText.fontSize=16;
		// GText.material.color = Color.Green;		
		UI_StatusText.transform.position=new Vector3(0,0,0);
		TextCoords = new Vector3(0,0,0);

		
		/// Status control init 
		m_HumanPiece_Behaviour =m_HumanPiece_Behaviour_Prev =  HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_IDLE;
		m_HumanStatus = HUMAN_STATUS.HUMAN_STATUS_INIT; 
		m_nCurrentFrame = 0 ;
		
		m_bDistroy = false;
		
		UI_StatusText.enabled = ( CoreGame.GetComponent("MainGameWorld") as MainGameWorld).bDebugShowMsg_Creature;
	}
	
	
	
	// Update is called once per frame
	void Update()
	{
		/// text for showing the status			
			TextCoords = Camera.main.WorldToScreenPoint(transform.position);
			UI_StatusText.transform.position = new Vector3(TextCoords.x / Screen.width ,TextCoords.y / Screen.height,0);		
	}
	
	void UpdateGameLogic(int nCurrentSec)
	{
		// GUITimer 
		// Result
		
		//just calculate all the logic 
		if( m_HumanStatus == HUMAN_STATUS.HUMAN_STATUS_INIT )
		{
			// check the activity
			CalculateGameLogic();
						
			// do the init 
			m_HumanStatus = HUMAN_STATUS.HUMAN_STATUS_MOVING;
			m_nCurrentFrame+=1;
			UI_StatusText.text = "Moving-"+(nCurrentSec)%5;
		}
		
		if( m_HumanStatus== HUMAN_STATUS.HUMAN_STATUS_MOVING )
		{
			//if(m_nCurrentFrame >=1000)
			if(  (nCurrentSec)%5  <=0)
			{
			 	// Moving 	
				UI_StatusText.text = "Moving-"+ (nCurrentSec)%5;
				transform.position = Vector3.Lerp(transform.position, Position_Destination,   Time.deltaTime*3.5f  );
				
			}
			else
			{
				// Finish the Moving 
				transform.position = Position_Destination;
				m_HumanStatus = HUMAN_STATUS.HUMAN_STATUS_BEHAVIOUR;
				UI_StatusText.text = "Behaviour-"+ (nCurrentSec)%5;
			}
			
			m_nCurrentFrame+=1;
		 }
		

		if( m_HumanStatus== HUMAN_STATUS.HUMAN_STATUS_BEHAVIOUR )
		{
			
			//if(m_nCurrentFrame >=3000)
			if(  (nCurrentSec)%5  <=3)
			{				
				// Building Behavior
				if (m_HumanPiece_Behaviour == HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_BUILD)
				{
					// Show the Building Animation.					
					// Add the Building on this tile.
					
					//BoardClass.Building_Culture     ;
					UI_StatusText.text = "Behaviour-Building-"+ (nCurrentSec)%5;	
				}
				else
				if( m_HumanPiece_Behaviour == HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_BUILD_USE)
				{
					UI_StatusText.text = "Behaviour-BuildingUse-"+ (nCurrentSec)%5;
				}
				else
				if( m_HumanPiece_Behaviour == HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_GETRESOURCE)
				{
					UI_StatusText.text = "Behaviour-Get Resource-"+ (nCurrentSec)%5;
				}
				else
				if(  m_HumanPiece_Behaviour == HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_FIGHT )
				{
					UI_StatusText.text = "Behaviour-Fight-"+ (nCurrentSec)%5;
				}

				
			}
			else
			{
				GameObject newBuilding;
				if (m_HumanPiece_Behaviour == HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_BUILD)
				{
					/// create new building 
					Quaternion rotation = Quaternion.identity;
					rotation.eulerAngles = new Vector3(180,0,0);
					
					BuildingBehavior.BUILDING_TYPE  nBuildType = (BuildingBehavior.BUILDING_TYPE)UnityEngine.Random.Range( 0 , (int)BuildingBehavior.BUILDING_TYPE.BUILDING_TYPE_COUNT);
					if(nBuildType == BuildingBehavior.BUILDING_TYPE.BUILDING_TYPE_CULTURE)
						newBuilding = Instantiate( BoardClass.Building_Culture , Position_Destination, rotation)  as GameObject;	
					else
					if(nBuildType == BuildingBehavior.BUILDING_TYPE.BUILDING_TYPE_HEALTH)	
						newBuilding = Instantiate( BoardClass.Building_Health , Position_Destination, rotation)  as GameObject;	
					else
					if(nBuildType == BuildingBehavior.BUILDING_TYPE.BUILDING_TYPE_SCIENCE)	
						newBuilding = Instantiate( BoardClass.Building_Science , Position_Destination, rotation)  as GameObject;	
					else
						newBuilding = Instantiate( BoardClass.Building_Culture , Position_Destination, rotation)  as GameObject;	
					
					
					
					// add location info to
			        BuildingBehavior pb = (BuildingBehavior)newBuilding.GetComponent("BuildingBehavior");
					pb.m_BuildingType = nBuildType;
					//m_BuildingType
					
					
					newBuilding.transform.parent =   GameObject.Find("Environments_Building").transform;
					pb.Piece = new GamePiece(  new Point(  Piece.X , Piece.Y) );
					BoardClass._gamePieces_Buildings.Add(newBuilding);
				}
				else
				if (m_HumanPiece_Behaviour == HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_BUILD_USE)
				{
					 // Get the building on the tile
					// Position_Destination
					
					// "AddPoint_Culture"
					MainGameWorld MainGameWorldClass =    CoreGame.GetComponent("MainGameWorld") as  MainGameWorld;
					MainGameWorldClass.BroadcastMessage("AddPoint_Culture");					
				}
				else
				if (m_HumanPiece_Behaviour == HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_GETRESOURCE)
				{
					 // Get the building on the tile
					// Position_Destination
					//  "AddPoint_Mine"
					//123 
						
					MainGameWorld MainGameWorldClass =    CoreGame.GetComponent("MainGameWorld") as  MainGameWorld;
					MainGameWorldClass.BroadcastMessage("AddPoint_Mine");
				}				
				else
				if(  m_HumanPiece_Behaviour == HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_FIGHT )
				{
					//UI_StatusText.text = "Behaviour-Fight-"+ (nCurrentSec)%5;
					//m_bDistroy = true;
				}
				
				m_HumanStatus = HUMAN_STATUS.HUMAN_STATUS_FINISH;
				UI_StatusText.text = "Finish-"+(nCurrentSec)%5;
			}
			
			m_nCurrentFrame+=1;
		 }		
	
		
		
		if( m_HumanStatus== HUMAN_STATUS.HUMAN_STATUS_FINISH )
		{
			
			m_nCurrentFrame+=1;
			
			//if(m_nCurrentFrame >=5000)
			
			if(  (nCurrentSec)%5  ==4)
			{
				UI_StatusText.text = "Finish-"+ (nCurrentSec)%5;
				
					if(m_bDistroy == true)
					{
						// DistroySelf - show the animation
						Tile TileStand = BoardClass._game.AllTiles.Single(o => o.X == Piece.Location.X && o.Y == Piece.Location.Y); // get the tile 
						BoardClass.KillHumanPiece( TileStand);
					}
			}
			else
			{

						m_HumanStatus = HUMAN_STATUS.HUMAN_STATUS_INIT;
						m_nCurrentFrame=0;
						UI_StatusText.text = "Init"+(nCurrentSec)%5;
			}
		}
		
		
	}
	
	
	// decide the logic 
	// 
	void CalculateGameLogic()
	{
		//if(BoardClass == null) return;
		
		/// first: get all the tile and data nearby (max totally 6 )
		
		
		
			// 1. get the tile i stand now.
			Tile TileStand = BoardClass._game.AllTiles.Single(o => o.X == Piece.Location.X && o.Y == Piece.Location.Y); // get the tile 
		
			// First , find if the Animal Stand on the same place 
			GameObject AnimalOnTile = BoardClass._gamePieces_Animals.Find( o => ( (AnimalBehaviour)o.GetComponent("AnimalBehaviour")).Piece.X == TileStand.X  && ( (AnimalBehaviour)o.GetComponent("AnimalBehaviour")).Piece.Y == TileStand.Y );
			// the same place ... Battle !
			if(AnimalOnTile)
			{
				//TileStand.Tile_StandStatus = TILE_STAND_STATUS.TILE_STAND_STATUS_BOTH;
				TileStand.IsBattleField = true;
			
					if( UnityEngine.Random.Range( 0 , 2) ==0  )
						m_bDistroy = true;
					else
						((AnimalBehaviour)AnimalOnTile.GetComponent("AnimalBehaviour")).m_bDistroy = true;
			
				// stay at the same place
					m_HumanPiece_Behaviour_Prev = m_HumanPiece_Behaviour ;
					m_HumanPiece_Behaviour = HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_FIGHT;
					Position_Destination =BoardBehavior.GetWorldCoordinates(TileStand.Location.X, TileStand.Location.Y, 0f);
			
			
					// to decide who win / lose
			
					return;
			}
		
		
			// or just do normal process
			TileStand.CanPass = true; // release this tile 
			TileStand.Tile_StandStatus = TILE_STAND_STATUS.TILE_STAND_STATUS_NONE;
		
		
			// 2.get all the neighbours
			int nNewX = Piece.X;
			int nNewY = Piece.Y;
			List <Tile>AvailableTiles =  TileStand.Neighbours.ToList();		
			// 2. include self  , calculate all it's attribute , apply to probability
			AvailableTiles.Add(TileStand);	
		
			//3.  decide which tile will be taken
			int nRandomChoose =  UnityEngine.Random.Range(0 , AvailableTiles.Count);
			Debug.Log( "CalculateGameLogic:"+ Piece.X + ","+Piece.Y +"neib # "+AvailableTiles.Count+",choose:"+ nRandomChoose );
		
			nNewX = AvailableTiles[nRandomChoose].X;
			nNewY = AvailableTiles[nRandomChoose].Y;
			//Piece_Destination = new GamePiece( nNewX , nNewY);
		
			// 4. 
			AvailableTiles[nRandomChoose].CanPass = false; // occupy now 
			AvailableTiles[nRandomChoose].Tile_StandStatus =TILE_STAND_STATUS.TILE_STAND_STATUS_HUMAN; /// hey now i stand on this tile 
		
			// 5. decide the action depends on probability	
			m_HumanPiece_Behaviour_Prev = m_HumanPiece_Behaviour ;
		
			if(AvailableTiles[nRandomChoose].TileStatus  == TILE_STATUS.TILE_STATUS_TERRIAN)
			{
				if (AvailableTiles[nRandomChoose].TerrianType == TERRIAN_STATUS.MINE)
				{
					m_HumanPiece_Behaviour = HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_GETRESOURCE;
				}
				else // if no mine => 
				{
					AvailableTiles[nRandomChoose].TileStatus = TILE_STATUS.TILE_STATUS_BUILDING;  // set this tile as 
				// decide the behavior by probability //m_Human
					m_HumanPiece_Behaviour = HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_BUILD; // hey  , now we are make building 
				}
			}
			else
			if(AvailableTiles[nRandomChoose].TileStatus  == TILE_STATUS.TILE_STATUS_BUILDING)
			{
			// the same 
			//AvailableTiles[nRandomChoose].TileStatus = TILE_STATUS.TILE_STATUS_BUILDING;  //  
			// Use It to Generate Point
				m_HumanPiece_Behaviour = HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_BUILD_USE; // hey  , now we are make building 
			}
			else
			if(AvailableTiles[nRandomChoose].TileStatus  == TILE_STATUS.TILE_STATUS_TERRIAN && AvailableTiles[nRandomChoose].TerrianType == TERRIAN_STATUS.MINE)
			{
				m_HumanPiece_Behaviour = HUMAN_BEHAVIOUR.HUMAN_BEHAVIOUR_GETRESOURCE; 
			}
			
			
			/// To process the AI activity			
			Piece = new GamePiece(new Point(nNewX, nNewY));
			//transform.position =  BoardBehavior.GetWorldCoordinates(Piece.X, Piece.Y, 0f);
			Position_Destination =BoardBehavior.GetWorldCoordinates(Piece.X, Piece.Y, 0f);
		
		
		
		// also to resigted this tile , means it's occupy by user now 
		
	}
	
	
	

	void switchDebugMessage( bool bOpen)
	{
		UI_StatusText.enabled = bOpen;
	}
	
	
	
	
}
