using UnityEngine;
using System.Collections;
using System.Threading;
using Voxel2D;
using WorldGen;

public class AstroidGenerator : MonoBehaviour {
	
	int[,] map;
	
	// Use this for initialization
	void Start () {
		StartCoroutine(Generate());
	}
	
	IEnumerator Generate()
	{

		
		int astroidSize = 50;
		map = new int[astroidSize,astroidSize];
	
		GenerationProcedures GP = new GenerationProcedures(ref map);

		Thread thread;
		thread = new Thread(GP.GenAstroid);
    	thread.Start();
		while(thread.IsAlive){
			yield return new WaitForEndOfFrame();
		}


		GameObject g = new GameObject();
		VoxelData[,] VD = VoxelUtility.IntToVoxelData(map);

		VoxelSystem v = g.AddComponent<VoxelSystem>();
		g.AddComponent<AstroidImpacter>();
		v.SetVoxelGrid(VD);
		v.SetMesh(VoxelMeshGenerator.VoxelToMesh(v.GetVoxelData()));

		//HACK: just for tests
		//g.rigidbody2D.angularVelocity = 100;

	}



	// Update is called once per frame
	void Update () {
		
	}
}
