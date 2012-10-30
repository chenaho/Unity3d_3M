using UnityEngine;
using System.Collections;

public class drawLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		
		
		Debug.DrawLine( transform.position ,transform.position+ transform.up*50, Color.yellow);
		
//		Debug.Log("transform.up:"+transform.up.ToString());
		
		//Debug.Log("(transform.position+ new Vector3(0f,10.0f,0)).normalized:"+(transform.position+ new Vector3(0f,10.0f,0)).ToString());
		Debug.DrawLine( transform.position , transform.position+ new Vector3(0f,10.0f,0) , Color.blue);
		
		
		
	}
}
