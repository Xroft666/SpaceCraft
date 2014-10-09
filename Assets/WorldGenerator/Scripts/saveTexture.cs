using UnityEngine;
using System.Collections;
using System.IO;

public class saveTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void saveToFile(string name, Texture2D tex){
		byte[] data = tex.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/savedTextures/"+name+".png", data);
	}


}


