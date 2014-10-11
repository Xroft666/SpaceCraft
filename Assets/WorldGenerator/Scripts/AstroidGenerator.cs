using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Voxel2D;
using WorldGen;

public class AstroidGenerator : MonoBehaviour {

	//HACK
	public List<Vector2> AstroidList = new List<Vector2>();

	int[,] map;
	
	// Use this for initialization
	void Start () {
		StartCoroutine(Generate());
	}
	
	IEnumerator Generate()
	{
		foreach(Vector2 pos in AstroidList){
		
			int astroidSize = 30;
			map = new int[astroidSize,astroidSize];
		
			GenerationProcedures GP = new GenerationProcedures(ref map);

			Thread thread;
			thread = new Thread(GP.GenAstroid);
	    	thread.Start();
			while(thread.IsAlive){
				yield return new WaitForEndOfFrame();
			}


			GameObject g = new GameObject();
			g.transform.position = pos;
			g.transform.name = "Astroid "+Random.seed;
			VoxelData[,] VD = VoxelUtility.IntToVoxelData(map);

			VoxelSystem v = g.AddComponent<VoxelSystem>();
			g.AddComponent<AstroidImpacter>();
			v.SetVoxelGrid(VD);
			v.SetMesh(VoxelMeshGenerator.VoxelToMesh(v.GetVoxelData()));

			//HACK: just for tests
			//g.rigidbody2D.angularVelocity = 100;
		}

	}



	// Update is called once per frame
	void Update () {
		
	}
}
