using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {
	
	RaycastHit hit ;
	Ray ray;
	public LayerMask groundLayers;
	
	/*
	 * Assigned the Face , then 
	 * */
	
	// Use this for initialization
	void Start () {
	
	}
	
	
	// assign 
	
	void Moving( Transform Destination)
	{
		
		   transform.position =  Vector3.Lerp(transform.position, Destination.position+Destination.up*3f,   Time.deltaTime * 10 );
	
		   transform.rotation =  Quaternion.Lerp(transform.rotation, Destination.rotation,   Time.deltaTime * 10 );

	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
		// correct the angle to walk on the sphere surface	
		int RayDistance = 10;
		Vector3 desireUp = new Vector3(0f,0f,0f);
		if (Physics.Raycast(transform.position ,  -transform.up*4 ,out hit, RayDistance)  ) 
		{
			Debug.DrawRay(transform.position , -transform.up*4, Color.yellow );
			
			desireUp = hit.normal;
			
			desireUp = (transform.up+desireUp).normalized;
			Vector3 newUp = (transform.up+desireUp*30).normalized;
			Debug.DrawRay(transform.position+ new Vector3 (0f,0.5f,0f), newUp, Color.green );
			
			float angle = Vector3.Angle(transform.up,newUp);
			Vector3 axis = Vector3.Cross(transform.up,newUp).normalized;
			Quaternion rota = Quaternion.AngleAxis(angle,axis);
			transform.rotation = rota * transform.rotation;		
			
		}
		
		
		
		
	// When no button is pressed we update the mesh collider
		if (!Input.GetMouseButton (0))
		{
			// Apply collision mesh when we let go of button
			//	ApplyMeshCollider();
			return;
		}		
		
		
		//var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		
		//if (Physics.Raycast(ray,out hit,groundLayers.value))
		if (Physics.Raycast(ray,out hit))
		{
			
			Renderer renderer =hit.collider.renderer;
			if(renderer)
			{
				renderer.material.shader = Shader.Find("Mobile/Diffuse");
				//renderer.material.shader = Shader.Find("FX/Flare");
			}
			
			GameObject faceObj = hit.collider.gameObject;
			if(faceObj)
			{
				Debug.Log("FaceObj="+ faceObj.ToString());
				
				//Vector3 desiredUp;
				//desiredUp = hit.normal;
				//Debug.DrawLine(hit.normal, hit.normal+
				
				//Vector3 Position =   faceObj.transform.position;
				Vector3 Position =   faceObj.transform.position;
				Vector3 Up = transform.position;
				Debug.DrawLine(Position,Up,Color.green,10);
				
				
				
				Debug.DrawLine(faceObj.transform.position,faceObj.transform.position + faceObj.transform.up*10f,Color.red);
				
				//Debug.Log("hit.normal="+hit.normal.ToString());
				
			//	transform.position = Position+Up;
				
//				Vector3 axis=new Vector3.Cross(transform.up,newUp).normalized;			
				Moving(faceObj.transform);
				
				
				
			}
			
			
			
			
		}
		
		
	}
}
