using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Model;
using System.Linq;
using PathFind;

public class AnimalBehaviour : MonoBehaviour {
	
	
	public GamePiece Piece;
	Vector3 Position_Destination;
	
	GameObject CoreGame;
	BoardBehavior BoardClass ;
	
	// UI
	GUIText  UI_StatusText;
	Vector3 TextCoords;
	
	bool  m_bDistroy = false;		
	
	
	public enum ANIMAL_BEHAVIOUR 
	{
	 ANIMAL_BEHAVIOUR_IDLE, // Do Nothing
	 ANIMAL_BEHAVIOUR_FIGHT, // Fight with human
	 ANIMAL_BEHAVIOUR_ESCAPE, // escape when
	 ANIMAL_BEHAVIOUR_MOVE_CASUAL // 
	}; // In_building-> 	
	
	public ANIMAL_BEHAVIOUR m_AnimalPiece_Behaviour_Prev;
	public ANIMAL_BEHAVIOUR m_AnimalPiece_Behaviour;	
	
	public enum ANIMAL_STATUS
	{
		ANIMAL_STATUS_INIT,
		//HUMAN_STATUS_LOGIC,
		ANIMAL_STATUS_MOVING,
		ANIMAL_STATUS_BEHAVIOUR,
		ANIMAL_STATUS_FINISH
	};	
	
	
	int m_nCurrentFrame;
	public ANIMAL_STATUS m_AnimalStatus;
	
	
	float m_ProbFight;
	float m_ProbEscape;
	

	
	
	// Use this for initialization
	void Start () {

		Piece.m_StrongValue=2;
		
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
		UI_StatusText.material.color = Color.green;		
		UI_StatusText.transform.position=new Vector3(0,0,0);
		TextCoords = new Vector3(0,0,0);		
		
		/// Status control init 
		m_AnimalPiece_Behaviour =m_AnimalPiece_Behaviour_Prev = ANIMAL_BEHAVIOUR.ANIMAL_BEHAVIOUR_IDLE;
		m_AnimalStatus = ANIMAL_STATUS.ANIMAL_STATUS_INIT; 
		m_nCurrentFrame = 0 ;
		
		m_bDistroy = false;		
	}
	
	// Update is called once per frame
	void Update () {
	
			TextCoords = Camera.main.WorldToScreenPoint(transform.position);
			UI_StatusText.transform.position = new Vector3(TextCoords.x / Screen.width ,TextCoords.y / Screen.height,0);		
		
	}
	
	
	// find the neighbhord and check if i need to fight with 
	void UpdateGameLogic(int nCurrentSec)	
	{
		
		
		
		if( m_AnimalStatus == ANIMAL_STATUS.ANIMAL_STATUS_INIT)
		{
			CalculateGameLogic();
			m_AnimalStatus = ANIMAL_STATUS.ANIMAL_STATUS_MOVING;
			m_nCurrentFrame+=1;
			UI_StatusText.text = "Moving-"+(nCurrentSec)%5;			
		}
		
		if( m_AnimalStatus == ANIMAL_STATUS.ANIMAL_STATUS_MOVING)
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
				m_AnimalStatus = ANIMAL_STATUS.ANIMAL_STATUS_BEHAVIOUR;
				UI_StatusText.text = "Behaviour-"+ (nCurrentSec)%5;
			}
			
			m_nCurrentFrame+=1;			
			
		}
		
		if( m_AnimalStatus == ANIMAL_STATUS.ANIMAL_STATUS_BEHAVIOUR)
		{
			if(  (nCurrentSec)%5  <=3)
			{
				if( m_AnimalPiece_Behaviour == ANIMAL_BEHAVIOUR.ANIMAL_BEHAVIOUR_MOVE_CASUAL)
				{
					
					UI_StatusText.text = "Behaviour-Move Casual-"+ (nCurrentSec)%5;	
				}
				
				
			}
			else
			{
				
				
				m_AnimalStatus = ANIMAL_STATUS.ANIMAL_STATUS_FINISH;
				UI_StatusText.text = "Finish-"+(nCurrentSec)%5;
				
			}
			
			m_nCurrentFrame+=1;
		}
		
		if( m_AnimalStatus == ANIMAL_STATUS.ANIMAL_STATUS_FINISH)
		{
			m_nCurrentFrame+=1;
			
			if(  (nCurrentSec)%5  ==4)
			{
				UI_StatusText.text = "Finish-"+ (nCurrentSec)%5;
				
					if(m_bDistroy == true)
					{
						// DistroySelf - show the animation
					}
			}
			else
			{

						m_AnimalStatus = ANIMAL_STATUS.ANIMAL_STATUS_INIT;
						m_nCurrentFrame=0;
						UI_StatusText.text = "Init"+(nCurrentSec)%5;
			}			
			
		}		
		
		
		
	}
	
	
	
	
	
	void CalculateGameLogic()
	{
		
		// 1. get the tile i stand now.
			Tile TileStand = BoardClass._game.AllTiles.Single(o => o.X == Piece.Location.X && o.Y == Piece.Location.Y); // get the tile 
			TileStand.CanPass = true; // release this tile 
			TileStand.Tile_StandStatus = TILE_STAND_STATUS.TILE_STAND_STATUS_NONE;

			// 2.get all the neighbours
			int nNewX = Piece.X;
			int nNewY = Piece.Y;

			//List <Tile>AvailableTiles =  TileStand.Neighbours.ToList();		
			List <Tile>AvailableTiles =  TileStand.NeighboursWithHuman.ToList();		
		//
			// 2. include self  , calculate all it's attribute , apply to probability
			AvailableTiles.Add(TileStand);	
		
			
			int nRandomChoose =  UnityEngine.Random.Range(0 , AvailableTiles.Count);
			Debug.Log( "CalculateGameLogic:"+ Piece.X + ","+Piece.Y +"neib # "+AvailableTiles.Count+",choose:"+ nRandomChoose );		

			nNewX = AvailableTiles[nRandomChoose].X;
			nNewY = AvailableTiles[nRandomChoose].Y;

			//3.  search the human , go to find or escape 
			/// 3-1 find the nearest one 
			/// 3-2 decide to attack or escape 				
			int nCount = BoardClass._gamePieces_Humans.Count;
			GameObject nearestHuman;

			var sp = Piece;
			GamePiece dp;
			int nShortestIndex=0;
			int nShortestDistance=1000;
		
			for( int i = 0 ; i <nCount ; i++ )
			{
				// get the nearest one , find the position
					nearestHuman= BoardClass._gamePieces_Humans[i];
					dp = ((HumanBehaviour)nearestHuman.GetComponent("HumanBehaviour")).Piece;
					
					int tmepDistance = Math.Max(  Math.Abs( dp.Location.X - sp.Location.X ), Math.Abs( dp.Location.Y - sp.Location.Y ) );
					if(tmepDistance < nShortestDistance)
					{
					 		nShortestDistance = tmepDistance;
							nShortestIndex = i;
					}
			}
				// ok , we get the nearest human , now go for it !
				Debug.Log("shortest distance["+nShortestIndex+"]="+ nShortestDistance);		
				nearestHuman= BoardClass._gamePieces_Humans[nShortestIndex];
				dp = ((HumanBehaviour)nearestHuman.GetComponent("HumanBehaviour")).Piece;
				
		
			/// find the nearest one to Selected human from all Available Tiles			
			nShortestIndex=0;
			nShortestDistance=1000;		
			for(int i = 0 ; i < AvailableTiles.Count ; i++)
			{
					int tmepDistance = Math.Max(  Math.Abs( dp.Location.X - AvailableTiles[i].Location.X  ), Math.Abs( dp.Location.Y - AvailableTiles[i].Location.Y  ) );
					if(tmepDistance < nShortestDistance)
					{
					 		nShortestDistance = tmepDistance;
							nShortestIndex = i;
					}
			}
				nRandomChoose = nShortestIndex;
		
			nNewX = AvailableTiles[nRandomChoose].Location.X;
			nNewY = AvailableTiles[nRandomChoose].Location.Y;
		
			// 4. 		
			AvailableTiles[nRandomChoose].CanPass = false; // occupy now 
			//AvailableTiles[nRandomChoose].Tile_StandStatus =TILE_STAND_STATUS.TILE_STAND_STATUS_HUMAN; /// hey now i stand on this tile 
			AvailableTiles[nRandomChoose].Tile_StandStatus =TILE_STAND_STATUS.TILE_STAND_STATUS_ANIMAL; /// hey now i stand on this tile 
		
			// 5. decide the action depends on probability
			m_AnimalPiece_Behaviour_Prev = m_AnimalPiece_Behaviour;
			
			m_AnimalPiece_Behaviour = ANIMAL_BEHAVIOUR.ANIMAL_BEHAVIOUR_MOVE_CASUAL;
		
		/*
		 *  process the detail behavior
		 * */
		
		
		/// To process the AI activity			
		Piece = new GamePiece(new Point(nNewX, nNewY));
		//transform.position =  BoardBehavior.GetWorldCoordinates(Piece.X, Piece.Y, 0f);
		Position_Destination =BoardBehavior.GetWorldCoordinates(Piece.X, Piece.Y, 0f);		
		
	}
	
	
	void switchDebugMessage( bool bOpen)
	{
		UI_StatusText.enabled = bOpen;
	}
	
}
