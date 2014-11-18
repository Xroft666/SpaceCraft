using UnityEngine;
using System.Collections;

public class RandomSeedGen : MonoBehaviour {

	// Use this for initialization


	public int DoIt(){
		
		return Random.Range(0, 90000);
	}

}
