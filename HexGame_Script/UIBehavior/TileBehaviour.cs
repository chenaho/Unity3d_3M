using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Model;

public class TileBehaviour : MonoBehaviour
{
    public Tile Tile;
	
	
	
	private string StrText;
    
	
	
	// TODO : 
    public void SetMaterial()
    {
//		Debug.Log("SetMaterial");
        //var mat = new Material(Shader.Find(" Glossy"));
        //mat.color = Tile.CanPass ? Color.green : mat.color = Color.black;
        //this.renderer.material = mat;
		
		StrText= "ground_grass_D";	
		
		if(Tile.TerrianType == TERRIAN_STATUS.FOREST || Tile.TerrianType == TERRIAN_STATUS.FOREST_GROUND)
			StrText= "forest_ground_D";
		
		if(Tile.TerrianType == TERRIAN_STATUS.WATER)
			StrText= "water";

		if(Tile.TerrianType == TERRIAN_STATUS.MINE)
			StrText= "mine_001_D";		
		
		if(Tile.TerrianType == TERRIAN_STATUS.MOUNTAIN)
			StrText= "Mountain_ground_D";				

		if(Tile.TerrianType == TERRIAN_STATUS.GROUND_FLOODPLAN)
			StrText= "ground_floodplan_D";				
		
		if(Tile.TerrianType == TERRIAN_STATUS.GROUND_FLOODPLAN)
			StrText= "ground_grass_D";				

		

		
		
		
		//this.renderer.material.shader = Shader.Find("Mobile/Diffuse");
		this.renderer.material.shader = Shader.Find("Diffuse");
		//this.renderer.material.mainTexture =  Tile.CanPass ? Resources.Load(StrText, typeof(Texture2D))  as Texture2D : Resources.Load("Mountain_ground_D", typeof(Texture2D))  as Texture2D;		
		this.renderer.material.mainTexture =   Resources.Load(StrText, typeof(Texture2D))  as Texture2D ;		
		this.renderer.material.mainTextureScale = new Vector2 (1,1);
		
		
		
		
		
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseUp()
    {
        Debug.Log(string.Format("Tile {0}", Tile.ToString()));
        Messenger<TileBehaviour>.Broadcast("Tile selected", this);
    }    
}
