using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharpNeat.Phenomes;
using Voxel2D;

public class shipBuilderBrain : UnitController {

	Dictionary<Voxel2D.IntVector2,int> takenPosition = new Dictionary<Voxel2D.IntVector2, int>();

	VoxelSystem voxelSystem;
	IBlackBox box;

	int[] blockCounts;

	int shipSize = 10;

	bool invalid = false;

	enum BlockType
	{
		engine
	}
	
	// Use this for initialization
	void Awake () {

		GameObject g = new GameObject("Ship");
		g.transform.position = new Vector3(Random.Range(-10,10),Random.Range(-10,10),0);
	
		voxelSystem = g.AddComponent<VoxelSystem>();
		voxelSystem.rigidbody2D.isKinematic = true;
		voxelSystem.SetGridSize(shipSize);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public override void Activate(IBlackBox box){
		this.box = box;
		bool running = true;
		//Debug.Log("-------------------------");
		while(running){
			running = NextStep();
		}
		//print("INVALID DNA "+voxelSystem.voxelCount);
		return;
	}
	
	public override void Stop(){
		Destroy(voxelSystem.gameObject);
	}
	
	public override float GetFitness(){
		if(invalid){
			return 0;
		}
		//Debug.Log("fitness "+voxelSystem.voxelCount);
		return blockCounts[0]+blockCounts[1]*2+blockCounts[2]*3+blockCounts[3]*10;
	}
	
	bool NextStep(){
	
	
		
		VoxelData[,] data = voxelSystem.GetVoxelData();
		
		blockCounts = new int[4];
		
		foreach(VoxelData vd in data){
			if(vd != null && vd.GetType().Name == "Wall"){
				blockCounts[0]++;
			}else if(vd != null && vd.GetType().Name == "Cannon"){
				blockCounts[1]++;
			}else if(vd != null && vd.GetType().Name == "Laser"){
				blockCounts[2]++;
			}else if(vd != null && vd.GetType().Name == "Engine"){
				blockCounts[3]++;
			} 
		}
		
		ISignalArray inputArr = box.InputSignalArray;

		for(int i=0;i<blockCounts.Length;i++){
			inputArr[i] = blockCounts[i];
		}
		
		box.Activate();
		
		ISignalArray outputArr = box.OutputSignalArray;
		
		int type = Mathf.Abs(Mathf.RoundToInt((float)outputArr[0]*4));
		int x = Mathf.Abs(Mathf.RoundToInt((float)outputArr[1]*shipSize));
		int y = Mathf.Abs(Mathf.RoundToInt((float)outputArr[2]*shipSize));
		int rot = Mathf.Abs(Mathf.RoundToInt((float)outputArr[3])*90);
		Voxel2D.IntVector2 iv2 = new Voxel2D.IntVector2(x,y);

		if(!takenPosition.ContainsKey(iv2) && voxelSystem.CanAddVoxel(iv2)){
			takenPosition.Add(iv2,type);
			VoxelData vd = null;
			if(type == 0){
				vd = new Wall(1,iv2,rot,voxelSystem);
			}else if(type == 1){
				vd = new Cannon(1,iv2,rot,voxelSystem,10,1);
			}else if(type == 2){
				vd = new Laser(1,iv2,rot,voxelSystem,250);
			}else if(type == 3){
				vd = new Engine(1,iv2,rot,voxelSystem,100);
			}else{
				vd = new Wall(1,iv2,rot,voxelSystem);
			}
			voxelSystem.AddVoxel(vd);
			//print("ADDED VOXEL "+voxelSystem.voxelCount);
			return true;
		}else{
			//Debug.Log("invalid "+voxelSystem.voxelCount);
			//invalid = true;
			return false;
		}
	}

	
	/*
	void OnGUI(){
		if(GUI.Button(new Rect(0,0,50,50),"NextShip")){
			NextStep();
		}
	}*/
}
