using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharpNeat.Phenomes;
using Voxel2D;

public class shipBuilderBrain : UnitController {
	
	VoxelSystem voxelSystem;
	IBlackBox box;

	int[] blockCounts;

	enum BlockType
	{
		engine
	}
	
	// Use this for initialization
	void Start () {
		voxelSystem = new VoxelSystem();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public override void Activate(IBlackBox box){
		
	}
	
	public override void Stop(){
		
	}
	
	public override float GetFitness(){
		return blockCounts[0];
	}
	
	void NextStep(){
		
		VoxelData[,] data = voxelSystem.GetVoxelData();
		
		blockCounts = new int[1];
		
		foreach(VoxelData vd in data){
			if(vd.GetType().Name == "Engine"){
				blockCounts[0]++;
			}
		}
		
		ISignalArray inputArr = box.InputSignalArray;

		for(int i=0;i<blockCounts.Length;i++){
			inputArr[i] = blockCounts[i];
		}
		
		box.Activate();
		
		ISignalArray outputArr = box.OutputSignalArray;
		
		int type = Mathf.RoundToInt((float)outputArr[0]);
		int x = Mathf.RoundToInt((float)outputArr[1]);
		int y = Mathf.RoundToInt((float)outputArr[2]);
	}

	
	
	void OnGUI(){
		if(GUI.Button(new Rect(0,0,50,50),"NextShip")){
			
		}
	}
}
