using UnityEngine;
using System.Collections;
using Voxel2D;
using WorldGen;

public class AstroidGenerator : MonoBehaviour {
	
	
	
	// Use this for initialization
	void Start () {
		testFunction();
	}
	
	void testFunction()
	{
		GameObject g = new GameObject();
		
		
		VoxelSystem v = g.AddComponent<VoxelSystem>();
		
		int astroidSize = 30;
		
		int[,] map = GenerationProcedures.method1(astroidSize);
		
		VoxelData[,] VD = new VoxelData[astroidSize,astroidSize];
		
		for (int x = 0; x < astroidSize; x++) {
			for (int y = 0; y < astroidSize; y++) {
				if(map[x,y] != 0){
					VD[x,y] = new VoxelData(1);
				}
			}
		}
		
		v.SetVoxelGrid(VD);
		
		
		
		v.SetMesh(VoxelMeshGenerator.VoxelToMesh(v.GetVoxelData()));
		
		g.rigidbody2D.angularVelocity = 100;
		
		//g.rigidbody2D.centerOfMass = new Vector2(10,10);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
