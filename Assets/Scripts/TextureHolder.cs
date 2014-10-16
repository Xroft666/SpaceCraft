using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MaterialSystem;

public class TextureHolder : MonoBehaviour {

	//public List<Texture2D> TileTextures = new List<Texture2D>();
	[SerializeField]
	private int MaxAtlasSize;

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

		List<Texture2D> TileTextures = new List<Texture2D>();
		for(int i=0;i<ElementList.Instance.elements.Count;i++){
			TileTextures.Add(ElementList.Instance.elements[i].texture);
		}


		TileTextureAtlas = new Texture2D(MaxAtlasSize,MaxAtlasSize);
		TileAtlastRects = TileTextureAtlas.PackTextures(TileTextures.ToArray(),0,MaxAtlasSize,false);
		TileMaterial = new Material(Shader.Find("Diffuse"));
		TileMaterial.mainTexture = TileTextureAtlas;
	}
}
