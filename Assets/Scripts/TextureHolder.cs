using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureHolder : MonoBehaviour {

	public List<Texture2D> TileTextures = new List<Texture2D>();
	public int MaxAtlasSize;

	public Texture2D TileTextureAtlas {get;private set;}
	public Rect[] TileAtlastRects {get;private set;}
	public Material TileMaterial {get;private set;}

	public static TextureHolder Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Destroy(gameObject);
		}
		TileTextureAtlas = new Texture2D(MaxAtlasSize,MaxAtlasSize);
		TileAtlastRects = TileTextureAtlas.PackTextures(TileTextures.ToArray(),0,MaxAtlasSize,false);
		TileMaterial = new Material(Shader.Find("Diffuse"));
		TileMaterial.mainTexture = TileTextureAtlas;
	}
}
