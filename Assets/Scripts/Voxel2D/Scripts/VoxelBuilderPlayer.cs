using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceSandbox;


namespace Voxel2D{
	public class VoxelBuilderPlayer : MonoBehaviour {
		
		VoxelSystem voxel;
		
		enum device
		{
			engine,
			ore,
			wall,
			floor,
			shipcontroller
		}
		
		int selectedElementID = 0;
		int selectedRotation = 0;
		device selectedDevice = device.floor;
		
		// Use this for initialization
		void Start () {
			
			voxel = gameObject.AddComponent<VoxelSystem>();
			
			Camera.main.transform.parent = transform;
			Camera.main.orthographicSize = 15;
			
			InitShip();
		}
		
		void InitShip(){
			VoxelData[,] startShip = new VoxelData[20,20];
			
			for (int x = 9; x < 14; x++) {
				for (int y = 8; y < 16; y++) {
					startShip[x,y] = new Wall(2, new IntVector2(x,y),0,voxel);
					//voxel.AddVoxel(x+5,y+5,2);
				}
			}
			voxel.SetVoxelGrid(startShip);
			
			voxel.AddVoxel(new ShipController(2, new IntVector2(11,7),0,voxel));
			voxel.AddVoxel(new Engine(2, new IntVector2(9,7),0,voxel,1000));
			voxel.AddVoxel( new Engine(2, new IntVector2(13,7),0,voxel,1000));
			voxel.AddVoxel(new Engine(2, new IntVector2(12,7),0,voxel,1000));
			voxel.AddVoxel( new Engine(2, new IntVector2(10,7),0,voxel,1000));
			
			Vector2 center = voxel.GetCenter();
			Camera.main.transform.position = transform.TransformPoint(new Vector3(center.x,center.y,-10));
		}
		
		
		// Update is called once per frame
		void Update () {
			CheckInput();
			selectedElementID = Mathf.Clamp(selectedElementID,1,MaterialSystem.ElementList.Instance.elements.Count-1);
			//Mathf.Clamp(selectedDevice,0,System.Enum.GetValues(typeof(device)).Length);
			if(selectedDevice.GetHashCode() <0){
				selectedDevice++;
			}else if(selectedDevice.GetHashCode() >System.Enum.GetValues(typeof(device)).Length-1){
				selectedDevice--;
			}
			selectedRotation = Mathf.Clamp(selectedRotation,0,270);
			
		}
		
		void CheckInput(){
			Vector2 point = Input.mousePosition;
			Vector3 world = Camera.main.ScreenToWorldPoint(point);
			Vector3 localPos = transform.InverseTransformPoint(world);
			IntVector2 RL = new IntVector2(Mathf.RoundToInt(localPos.x),Mathf.RoundToInt(localPos.y));
			
			if(Input.GetMouseButtonDown(0)){
				if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),localPos)){
					if(!voxel.IsVoxelEmpty(RL.x,RL.y)){
						voxel.RemoveVoxel(RL.x,RL.y);
						List<bool[,]> islands = VoxelIslandDetector.findIslands(VoxelUtility.VoxelDataToBool(voxel.GetVoxelData()));
						if(islands.Count == 1){
							
						}else{
							VoxelData[,] vox = VoxelIslandDetector.SplitIslands(voxel.GetVoxelData(),voxel);
							voxel.SetVoxelGrid(vox);
						}
					}
				}
			}else if(Input.GetMouseButtonDown(1)){
				if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),localPos)){
					if(VoxelUtility.IsPosNextToVoxel(voxel.GetVoxelData(),RL)){
						if(voxel.IsVoxelEmpty(RL.x,RL.y)){
							AddSelectedVoxelType(RL.x,RL.y);
						}
					}
				}
			}
			
			if(Input.GetKeyDown(KeyCode.U)){
				selectedElementID++;
			}else if(Input.GetKeyDown(KeyCode.J)){
				selectedElementID--;
			}
			
			if(Input.GetKeyDown(KeyCode.O)){
				selectedRotation += 90;
			}else if(Input.GetKeyDown(KeyCode.L)){
				selectedRotation -= 90;
			}
			
			if(Input.GetKeyDown(KeyCode.I)){
				selectedDevice++;
			}else if(Input.GetKeyDown(KeyCode.K)){
				selectedDevice--;
			}
			
			
			
		}
		
		void AddSelectedVoxelType(int x,int y){
			VoxelData VD = null;
			
			switch(selectedDevice){
			case device.engine:
				VD = new Engine(selectedElementID,new IntVector2(x,y),selectedRotation,voxel,1000);
				break;
			case device.floor:
				VD = new Floor(selectedElementID,new IntVector2(x,y),selectedRotation,voxel);
				break;
			case device.ore:
				VD = new Ore(selectedElementID,new IntVector2(x,y),selectedRotation,voxel);
				break;
			case device.shipcontroller:
				VD = new ShipController(selectedElementID,new IntVector2(x,y),selectedRotation,voxel);
				break;
			case device.wall:
				VD = new Wall(selectedElementID,new IntVector2(x,y),selectedRotation,voxel);
				break;
			}
			voxel.AddVoxel(VD);
			
		}
		
		void OnGUI(){
			GUI.Label(new Rect(Screen.width/2-150,0,300,20),"element: "+selectedElementID+" device: "+selectedDevice.ToString()+" rotation: "+selectedRotation);
		}
		
	}
	
}