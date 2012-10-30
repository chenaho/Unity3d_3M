using UnityEngine;
using System.Collections;
using Model;


public class BuildingBehavior : MonoBehaviour {

	public GamePiece Piece; // Location
	
	
	// GUIText ShowStatus	
	// UI
	public GUIText  UI_StatusText;
	Vector3 TextCoords;	
	
	// TYPE: Culture, Science , Health
	
	
	// Use this for initialization
	void Start () {
	
		
		/// Init the TextUI
		GameObject go  = new GameObject("Text_Building_"+ this.name);
		UI_StatusText = (GUIText)go.AddComponent( typeof(GUIText) );
		UI_StatusText.transform.parent = transform;

		Font BroadwayFont = (Font)Resources.Load("Fonts/Arial");
		UI_StatusText.font = BroadwayFont;
		UI_StatusText.text = "Building_Level1";
		UI_StatusText.fontSize=16;
		UI_StatusText.material.color = Color.blue;
		// GText.material.color = Color.Green;		
		UI_StatusText.transform.position=new Vector3(0,0,0);
		TextCoords = new Vector3(0,0,0);		
		
		
		
	}
	
	
	
	// Update is called once per frame
	void Update () {

		/// text for showing the status
		TextCoords = Camera.main.WorldToScreenPoint(transform.position);
		UI_StatusText.transform.position = new Vector3( (TextCoords.x -30) / Screen.width , ( TextCoords.y -10)/ Screen.height,0);		
	}

	
	void switchDebugMessage( bool bOpen)
	{
		UI_StatusText.enabled = bOpen;
	}	
	
	
	
}
