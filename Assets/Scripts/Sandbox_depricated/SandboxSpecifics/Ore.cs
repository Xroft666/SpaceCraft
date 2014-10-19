using UnityEngine;
using System.Collections;
using SpaceSandbox;

public class Ore : Device {

	public Ore(){
		deviceName = "Ore";
	}

	public override void OnStart(params object[] input){
		deviceName = "Ore";
	}
}
