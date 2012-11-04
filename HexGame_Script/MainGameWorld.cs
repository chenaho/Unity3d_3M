using UnityEngine;
using System;
using System.Collections;
//using System.Collections
using System.Collections.Generic;
public class MainGameWorld : MonoBehaviour {


	 
	enum CULTURE_LIMIT{
		CULTURE_LIMIT_01=10,
		CULTURE_LIMIT_02=30,
		CULTURE_LIMIT_03=75,
		CULTURE_LIMIT_04=150,
		CULTURE_LIMIT_05=400,
	};
	
	
	// Game-Turn Manager 
	// GamePoint Manager

	#region Game Time Managment
	/// Time Manager item
	string sTime_Global;
	int nTime_Global;
	string sTime_Unit;
	int nTime_Unit ;
	int GUITimer;
	
	int m_TurnCount=0;
	
	int m_CurrentTurnCount=0;
	#endregion
	
	
	#region DebugGUI Setting
	bool g_bDebugUI= false;
	
	float UI_Start_Left= 20;
	float UI_Start_Top= 70;
	float UI_Space= 15;
	
	public bool bDebugShowMsg_Creature = true;
	public bool bDebugShowMsg_Building = true;
	
	#endregion
	
	
	#region UIToolkit UI
	private UITextInstance text1, text2, text3, text4, text5, text6;		
	
	UIText UIText_Era;
	public UIToolkit buttonToolkit;
	public UIToolkit textToolkit;
	UITextInstance TextUI_ERA;
		
	UIProgressBar progressBar_Culture ;
	UIProgressBar progressBar_Healtth ; 
	UIProgressBar progressBar_Science;
	public UIProgressBar progressBar_Base;	
	
//	string debugString;
	#endregion
	
	
	
	// List - Building 
	// List - House
	

	
	
	
	public enum GAME_STATUS{ 
		GAME_STATUS_INIT , // Select the Item
		GAME_STATUS_IN_GAME , 
		GAME_STATUS_FINISH  /// Show the result , restart or backToMain
	}
	
	
	/// <summary>
	/// ERA Define 
	/// </summary>	
	#region ERA Setting
	public enum ERA_TYPE { ERA_TYPE_01=1,ERA_TYPE_02,ERA_TYPE_03,ERA_TYPE_04,ERA_TYPE_05,ERA_TYPE_06,ERA_TYPE_COUNT }; 
	public Dictionary<ERA_TYPE,string> m_ERA_NAMEDic ;
	
	public struct Struct_ERA
	{
		 public ERA_TYPE eERA;
		 public int Point_Culture;
		 public int Point_HumanRobust_DownLimit; // culture data
		 public int Point_HumanRobust_UPLimit;
	}
	
	public ERA_TYPE m_currentERA;
	public Struct_ERA[] sERA_Define;		
	
	public int m_Current_HumanRobust__DownLimit=0;
	public int m_Current_HumanRobust__UPLimit=0;
	#endregion
	
	
	#region Game_Point
	int Point_Culture=0;
	int Point_Science=0;
	int Point_Mine=0;
	int Point_Health=0;
	
	int Point_Kama_Positive=0;
	int Point_Kama_Negative=0;
	#endregion
	
	BoardBehavior CoreBoard ;
	
	//public enum TERRIAN_TYPE { ERA_TYPE_01,ERA_TYPE_02,ERA_TYPE_03,ERA_TYPE_04,ERA_TYPE_05,ERA_TYPE_06,ERA_TYPE_COUNT }; 
	public enum TERRIAN_STATUS { TERRIAN_NULL,TERRIAN_BUILDING }; /// spare / with building
	

	public void AddPoint_Mine()
	{
		Point_Mine =Point_Mine +  (int)m_currentERA;	
	
		Debug.Log("AddPoint_Mine: +"+(int)m_currentERA);
	}
	
	public void AddPoint_Culture()
	{
		Point_Culture = Point_Culture +   (int)m_currentERA * 3;	
		
		Debug.Log("AddPoint_Culture: +"+(int)m_currentERA* 3  );
	}
	
	
	protected void Awake () {
		
		// init ERA Table
		m_currentERA = ERA_TYPE.ERA_TYPE_01;
		m_ERA_NAMEDic= new Dictionary<ERA_TYPE,string>();
		m_ERA_NAMEDic.Add(ERA_TYPE.ERA_TYPE_01,"1.Ancient-Age");
		m_ERA_NAMEDic.Add(ERA_TYPE.ERA_TYPE_02,"2.Classic-Age");
		m_ERA_NAMEDic.Add(ERA_TYPE.ERA_TYPE_03,"3.Mid-Age");
		m_ERA_NAMEDic.Add(ERA_TYPE.ERA_TYPE_04,"4.Renaissance-Age");
		m_ERA_NAMEDic.Add(ERA_TYPE.ERA_TYPE_05,"5.Industrial-Revolution-Age");
		m_ERA_NAMEDic.Add(ERA_TYPE.ERA_TYPE_06,"6.Modern-Age");
		
		
		
		/// init World Game Rule
		/*
		sERA_Define = new Struct_ERA[ (int)ERA_TYPE.ERA_TYPE_COUNT];
		sERA_Define[0].eERA = ERA_TYPE.ERA_TYPE_01; //
		sERA_Define[0].Point_Culture = 0;
		sERA_Define[0].Point_HumanRobust_DownLimit=0;
		sERA_Define[0].Point_HumanRobust_UPLimit=1;

		sERA_Define[1].eERA = ERA_TYPE.ERA_TYPE_02; //
		sERA_Define[1].Point_Culture = 10;
		sERA_Define[1].Point_HumanRobust_DownLimit=2;
		sERA_Define[1].Point_HumanRobust_UPLimit=3;
		
		sERA_Define[2].eERA = ERA_TYPE.ERA_TYPE_03; //
		sERA_Define[2].Point_Culture = 30;
		sERA_Define[2].Point_HumanRobust_DownLimit=4;
		sERA_Define[2].Point_HumanRobust_UPLimit=5;		
				
		sERA_Define[3].eERA = ERA_TYPE.ERA_TYPE_04; //
		sERA_Define[3].Point_Culture = 75;
		sERA_Define[3].Point_HumanRobust_DownLimit=6;
		sERA_Define[3].Point_HumanRobust_UPLimit=7;				

		sERA_Define[4].eERA = ERA_TYPE.ERA_TYPE_05;//
		sERA_Define[4].Point_Culture = 150;
		sERA_Define[4].Point_HumanRobust_DownLimit=8;
		sERA_Define[4].Point_HumanRobust_UPLimit=9;				*/
		
		nTime_Global=(int)Time.time;	
	}
	
	
	// Use this for initialization
	protected void Start () {
	
		 CoreBoard = (BoardBehavior) GetComponent("BoardBehavior");
		
		
		
		progressBar_Base = UIProgressBar.create(buttonToolkit, "UI_Base.png", 0, 0,true,2  );
		//progressBar_Science.positionFromBottomLeft( .15f, .02f );
		progressBar_Base.positionFromTopLeft( 20f/Screen.height, 960f/Screen.width );
		progressBar_Base.resizeTextureOnChange = true;
		progressBar_Base.value = 1f;				
		
//		debugString = "";
		// Progress/Health bar
		progressBar_Culture = UIProgressBar.create( buttonToolkit,"UI_Ball.png", 0, 0,false,1 );
		//progressBar_Culture = UIProgressBar.create( "UI_Ball.png", 0, 0 );
		//progressBar.positionFromLeft
		//progressBar_Culture.positionFromTopRight( .17f, .40f );
		progressBar_Culture.positionFromTopLeft(  30f/Screen.height,   685f/Screen.width);
		
		progressBar_Culture.resizeTextureOnChange = true;
		progressBar_Culture.value = 0.0f;
		
		
		
		progressBar_Healtth = UIProgressBar.create(buttonToolkit, "UI_Ball.png", 0, 0 ,true,1 );
		progressBar_Healtth.positionFromTopLeft(30f/Screen.height, 830f/Screen.width);
		progressBar_Healtth.resizeTextureOnChange = true;
		progressBar_Healtth.value = 1.0f;		
		
		progressBar_Science = UIProgressBar.create(buttonToolkit, "UI_Ball.png", 0, 0,true,1  );
		//progressBar_Science.positionFromBottomLeft( .15f, .02f );
		progressBar_Science.positionFromTopLeft(30f/Screen.height, 916f/Screen.width );
		progressBar_Science.resizeTextureOnChange = true;
		progressBar_Science.value = 1.0f;		
		
	
		
		
		//StartCoroutine( animateProgressBar( progressBar_Culture ) );		
		
		
		
		UIText_Era = new UIText( textToolkit, "prototype", "prototype.png" );
		TextUI_ERA = UIText_Era.addTextInstance( m_ERA_NAMEDic[  m_currentERA ], 0, 0 );
        //helloText.positionFromTopLeft( 0.1f, 0.05f );			
		TextUI_ERA.positionFromTopLeft( 0.1f, 0.05f );					

	}
	
	
	
	
	
	
	// Update is called once per frame
	protected void FixedUpdate () {
	
		/// global Game rule-time limit 
		UpdateGameTimer(); 
		
		/// update every object's action , by using broadcast function
		
		UpdateERA();
		CheckKAMA();
		
		
		if(UpdateCulturePoint() == true)
		{
		}
		
		if( CoreBoard._gamePieces_Humans.Count !=0 )
			CoreBoard.Actor_Humans.BroadcastMessage("UpdateGameLogic", GUITimer );	
		
		
		if( CoreBoard._gamePieces_Animals.Count !=0 )
			CoreBoard.Actor_Animals.BroadcastMessage("UpdateGameLogic", GUITimer );	
		

		/*if( progressBar_Base.position.ToString() != null)
			debugString ="data:"+ progressBar_Base.position.ToString();
		else
			debugString =  "no data!!!";*/		
		
	}
	
	
	void UpdateERA()
	{
		//m_currentERA
		if (m_currentERA== ERA_TYPE.ERA_TYPE_01 &&   Point_Culture >= (int)CULTURE_LIMIT.CULTURE_LIMIT_01 )
			SetERAData( ERA_TYPE.ERA_TYPE_02 , 2,3);
		else
		if (m_currentERA== ERA_TYPE.ERA_TYPE_02 &&   Point_Culture >= (int)CULTURE_LIMIT.CULTURE_LIMIT_02 )
			SetERAData( ERA_TYPE.ERA_TYPE_03 , 4, 5);
		// 03-> 04
		else
		if (m_currentERA== ERA_TYPE.ERA_TYPE_03 &&   Point_Culture >= (int)CULTURE_LIMIT.CULTURE_LIMIT_03 )
			SetERAData( ERA_TYPE.ERA_TYPE_04 , 6, 7);
		else
		if (m_currentERA== ERA_TYPE.ERA_TYPE_04 &&   Point_Culture >= (int)CULTURE_LIMIT.CULTURE_LIMIT_04 )
			SetERAData( ERA_TYPE.ERA_TYPE_05 , 8, 9);
		else
		if (m_currentERA== ERA_TYPE.ERA_TYPE_05 &&   Point_Culture >= (int)CULTURE_LIMIT.CULTURE_LIMIT_05 )
			SetERAData( ERA_TYPE.ERA_TYPE_06 , 10,11);
		
			progressBar_Culture.value = (float)Point_Culture / (int)CULTURE_LIMIT.CULTURE_LIMIT_05;		
	}
	
	void SetERAData( ERA_TYPE newEra , int nRobust_DownLimit, int nRobust_UpLimit)
	{
			m_currentERA = newEra;
			m_Current_HumanRobust__DownLimit = nRobust_DownLimit;
			m_Current_HumanRobust__UPLimit = nRobust_UpLimit;					
		
			TextUI_ERA.text = m_ERA_NAMEDic[  m_currentERA ];
		
			CoreBoard.SendMessage("createHumanPiece" , HumanBehaviour.HUMAN_TYPE.HUMAN_TYPE_NORMAL);
	}

	
	
	
	// TODO: KAMA rule
	void CheckKAMA()
	{
		//Positive Kama > 0.75 : human StrongValue+2 , Global Value up 10%  
		//Negative Kama >0.75 : Animal StrongValue +2 
		//Positive Kama >= 1.00 : human must Win, 4 global value Level Up to next generation
		//Negative Kama >= 1.00 : human must lose, human lazy rate up 25% , Epic Animal
		
		//Postive Kama >1.00 && Negative Kama >= 1.00 : human must win , Epic Animal
		
		//Point_Kama_Positive=0;
		//Point_Kama_Negative=0;
	}
	bool UpdateCulturePoint() // 
	{
		if( m_CurrentTurnCount !=m_TurnCount)
		{
			// add culture point 
			if( m_currentERA == ERA_TYPE.ERA_TYPE_01)
			{ 
				// update  in each new turn
			}
			
			
			Point_Culture += m_currentERA;
			m_CurrentTurnCount = m_TurnCount;
			return  true;			
		}
		
		return false;
	}
	
	
	

	protected void OnGUI () {
		
		//GUI.Label (new Rect(20,70,100,20),   GUITimer.ToString() );
		
		if( GUI.Button( new Rect(10,5,100,20),"open/close") )
		{
			g_bDebugUI = !g_bDebugUI;
		}

		
		if(!g_bDebugUI) return;
		// debug window
		GUI.Box(new Rect(UI_Start_Left-15,UI_Start_Top-15,300,500),"DebugWin");
		
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*-1 ,180,20),   GUITimer.ToString("Sec: #."));
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*0 ,180,20),   m_TurnCount.ToString("TURN: #."));
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*1 ,180,20),   "ERA:"+m_ERA_NAMEDic[  m_currentERA ]);
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*2 ,180,20), "P-Kama:"+Point_Kama_Positive);
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*3 ,180,20), "N-Kama:"+Point_Kama_Negative);
		
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*4 ,180,20), "Point-Culture:"+Point_Culture);
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*5 ,180,20), "Human strength:"+m_Current_HumanRobust__DownLimit);
		
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*7 ,180,20), "Point-Science:"+Point_Science);
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*8 ,180,20), "Point_Mine:"+Point_Mine);
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*9 ,180,20), "Point_Health:"+Point_Health);
		
		if( GUI.Button( new Rect(UI_Start_Left,UI_Start_Top + UI_Space*11,180,20),"add Human Unit") )
		{
			//createHumanPiece
			CoreBoard.createHumanPiece(HumanBehaviour.HUMAN_TYPE.HUMAN_TYPE_NORMAL);
			
			//GameObject destroyObj = CoreBoard._gamePieces_Humans.Find( o => o.name =="Human_Adam");
			//Destroy( destroyObj);
			//AddAllEnemies
		}

		if( GUI.Button( new Rect(UI_Start_Left,UI_Start_Top + UI_Space*12,180,20),"Add new Animal") )
		{
			CoreBoard.createAnimalPiece();
		}		
		
		if( GUI.Button( new Rect(UI_Start_Left,UI_Start_Top + UI_Space*13,180,20),"Add:P-Kama") )
		{}				
		if( GUI.Button( new Rect(UI_Start_Left,UI_Start_Top + UI_Space*14,180,20),"Add:N-Kama") )
		{}						
		
		if( GUI.Button( new Rect(UI_Start_Left,UI_Start_Top + UI_Space*16,180,20),"debug: on/off human text") )
		{
				bDebugShowMsg_Creature = !bDebugShowMsg_Creature;
				GameObject.Find("Actor_Humans").BroadcastMessage( "switchDebugMessage", bDebugShowMsg_Creature);
		}								

		if( GUI.Button( new Rect(UI_Start_Left,UI_Start_Top + UI_Space*17,180,20),"debug: on/off Animal text") )
		{
				bDebugShowMsg_Creature = !bDebugShowMsg_Creature;
				GameObject.Find("Actor_Animals").BroadcastMessage( "switchDebugMessage", bDebugShowMsg_Creature);
		}								
		
		
		if( GUI.Button( new Rect(UI_Start_Left,UI_Start_Top + UI_Space*18,180,20),"debug: on/off Building text") )
		{
			bDebugShowMsg_Building = !bDebugShowMsg_Building;			
			GameObject.Find("Environments_Building").BroadcastMessage( "switchDebugMessage" ,bDebugShowMsg_Building);
		}
		
		
		//progressBar_Healtth
		// btn : stop / start the game 
		GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*20 ,180,20), "UI-string:");
		//GUI.Label (new Rect(UI_Start_Left,UI_Start_Top + UI_Space*21 ,180,20), "UI-string:"+debugString);
		
		
		// btn : 
		
		
		
	}
	
	
	
	/// <summary>
	/// Updates the game timer , and for each-term update
	/// </summary>
	private void UpdateGameTimer()
	{
		 GUITimer = (int)(Time.time - nTime_Global);
		 m_TurnCount = GUITimer/5;
	}
	
	
	

	private IEnumerator animateProgressBar( UIProgressBar progressBar )
	{
		float value = 0.0f;
		while( true )
		{
			// Make sure the progress doesnt exceed 1
			if( value > 1.0f )
				// Swap the progressBars resizeTextureOnChange property
			//	progressBar.resizeTextureOnChange = !progressBar.resizeTextureOnChange;
				value = 0.0f;
			else
				value += 0.01f;
			
			progressBar.value = value;			
			yield return 0;
		}
	}			
		

	
	
}
