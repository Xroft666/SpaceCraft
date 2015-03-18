using System;
using SpaceSandbox;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MaterialSystem;
using Material = UnityEngine.Material;

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

	    Array x = Enum.GetValues(typeof (DeviceData.DeviceType));
	    int l = x.Length;
	    Texture2D[] DeviceTextures = new Texture2D[l];
		for(int i=0;i<devices.Count;i++)
		{
		    DeviceData.DeviceType d = devices[i].Device;
            int index = (int)d;
		    DeviceTextures[index] = devices[i].deviceTexture;
		}
		
		DeviceTextureAtlas = new Texture2D(MaxAtlasSize,MaxAtlasSize);
		DeviceTextureAtlas.filterMode = FilterMode.Point;
		DeviceTextureAtlas.wrapMode = TextureWrapMode.Clamp;

		DeviceAtlasRects = DeviceTextureAtlas.PackTextures(DeviceTextures,0,MaxAtlasSize,false);
		DeviceMaterial = new Material(Shader.Find("Sprites/Default"));
		DeviceMaterial.mainTexture = DeviceTextureAtlas;

	}
    /*
    public int GetDeviceIndex(string deviceName){
		
        for (int i = 0; i < devices.Count; i++) {
            if(devices[i].deviceName == deviceName){
                return i;
            }
        }
        
        return -1;
    }
    
    public string GetDeviceName(int index)
    {
        return devices[index].deviceName;
    }*/
	
	[System.Serializable]
	public class DeviceDescriptor
	{
	    public DeviceData.DeviceType Device;
        //public string deviceName;
		public Texture2D deviceTexture;
	}
}
