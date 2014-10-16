using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceSandbox;

public class DeviceList : MonoBehaviour {

	//public List<EntityRepresentation> test = new List<EntityRepresentation>();


	public static DeviceList Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
