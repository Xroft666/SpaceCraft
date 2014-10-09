using UnityEngine;
using System.Collections;
using Voxel2D;

public class AstroidGenerator : MonoBehaviour {



	// Use this for initialization
	void Start () {
		testFunction();
	}

	void testFunction()
	{
		GameObject g = new GameObject();


		VoxelSystem v = g.AddComponent<VoxelSystem>();

		int astroidSize = 10;
		v.SetGridSize(astroidSize);

		for (int x=0; x<astroidSize; x++) {
			for (int y=0; y<astroidSize; y++) {
				float place = Random.Range(0f,1f);
				if(place >= 0.5f){
					v.SetVoxel(x,y,0);
				}
			}
		}

		v.SetMesh(VoxelMeshGenerator.VoxelToMesh(v.GetVoxelData()));

		g.rigidbody2D.angularVelocity = 100;

		//g.rigidbody2D.centerOfMass = new Vector2(10,10);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
