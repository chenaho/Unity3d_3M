using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour {
	
	#region UIToolkit
	private UITextInstance text1, text2, text3, text4, text5, text6;		
	
	UIText UIText_Era;
	public UIToolkit buttonToolkit;
	public UIToolkit textToolkit;
	#endregion	
	
	#region UIToolkit UI
	UIProgressBar progressBar_Culture ;
	UIProgressBar progressBar_Healtth ; 
	UIProgressBar progressBar_Science;
	public UIProgressBar progressBar_Base;	
	
	
//	string debugString;
	#endregion
	
	
	// Use this for initialization
	void Start () {
		
//		debugString = "";
		// Progress/Health bar
		progressBar_Culture = UIProgressBar.create( buttonToolkit,"UI_Ball.png", 0, 0,false,1 );
		//progressBar_Culture = UIProgressBar.create( "UI_Ball.png", 0, 0 );
		//progressBar.positionFromLeft
		progressBar_Culture.positionFromTopRight( .17f, .40f );
		progressBar_Culture.resizeTextureOnChange = true;
		progressBar_Culture.value = 0.4f;
		
		
		
		progressBar_Healtth = UIProgressBar.create(buttonToolkit, "UI_Ball.png", 0, 0 ,true,1 );
		progressBar_Healtth.positionFromTopRight( .17f, .22f );
		progressBar_Healtth.resizeTextureOnChange = true;
		progressBar_Healtth.value = 1.0f;		
		
		progressBar_Science = UIProgressBar.create(buttonToolkit, "UI_Ball.png", 0, 0,true,1  );
		//progressBar_Science.positionFromBottomLeft( .15f, .02f );
		progressBar_Science.positionFromTopRight( .17f, .10f );
		progressBar_Science.resizeTextureOnChange = true;
		progressBar_Science.value = 1.0f;		
		
		progressBar_Base = UIProgressBar.create(buttonToolkit, "UI_Base.png", 0, 0,true,2  );
		//progressBar_Science.positionFromBottomLeft( .15f, .02f );
		progressBar_Base.positionFromTopRight( .15f, .05f );
		progressBar_Base.resizeTextureOnChange = true;
		progressBar_Base.value = 1f;			
		
		
		StartCoroutine( animateProgressBar( progressBar_Culture ) );		
		
		
		
		UIText_Era = new UIText( textToolkit, "prototype", "prototype.png" );
		var helloText = UIText_Era.addTextInstance( "Aloha", 0, 0 );
        //helloText.positionFromTopLeft( 0.1f, 0.05f );			
		helloText.positionFromTopLeft( 0.1f, 0.05f );			
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
		/*if( progressBar_Base.position.ToString() != null)
			debugString ="data:"+ progressBar_Base.position.ToString();
		else
			debugString =  "no data!!!";*/
		
		
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
