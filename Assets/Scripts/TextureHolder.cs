using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MaterialSystem;

public class TextureHolder : MonoBehaviour {
	
	//public List<Texture2D> TileTextures = new List<Texture2D>();
	[SerializeField]
	private int MaxAtlasSize;
	
	public List<DeviceDescriptor> devices = new List<DeviceDescriptor>();
	
	public Texture2D DeviceTextureAtlas {get;private set;}
	public Rect[] DeviceAtlasRects {get;private set;}
	public Material DeviceMaterial {get;private set;}

	//HACK, move somewhere else
	public GameObject bulletPrefab;

	public static TextureHolder Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Destroy(gameObject);
		}
		
		List<Texture2D> TileTextures = new List<Texture2D>();
		for(int i=0;i<ElementList.Instance.elements.Count;i++){
			Texture2D t = new Texture2D(1,1);
			t.SetPixel(1,1, ElementList.Instance.elements[i].color);
			TileTextures.Add(t);
			//TileTextures.Add(ElementList.Instance.elements[i].texture);
		}


		List<Texture2D> DeviceTextures = new List<Texture2D>();
		for(int i=0;i<devices.Count;i++){
			DeviceTextures.Add(devices[i].deviceTexture);
		}
		
		DeviceTextureAtlas = new Texture2D(MaxAtlasSize,MaxAtlasSize);
		DeviceTextureAtlas.filterMode = FilterMode.Point;
		DeviceTextureAtlas.wrapMode = TextureWrapMode.Clamp;

		DeviceAtlasRects = DeviceTextureAtlas.PackTextures(DeviceTextures.ToArray(),0,MaxAtlasSize,false);
		DeviceMaterial = new Material(Shader.Find("Sprites/Default"));
		DeviceMaterial.mainTexture = DeviceTextureAtlas;

	}
	
	public int GetDeviceIndex(string deviceName){
		
		for (int i = 0; i < devices.Count; i++) {
			if(devices[i].deviceName == deviceName){
				return i;
			}
		}
		return -1;
	}
	
	[System.Serializable]
	public class DeviceDescriptor{
		public string deviceName;
		public Texture2D deviceTexture;
	}
}
