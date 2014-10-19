using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceSandbox;

namespace Voxel2D{
	public class VoxelBuilderPlayer : MonoBehaviour {
		
		VoxelSystem voxel;

		int selectedID = 0;

		//HACK ALL OVER DA PLACE!!
		public List<Device> deviceList = new List<Device>();

		// Use this for initialization
		void Start () {
			
			voxel = gameObject.AddComponent<VoxelSystem>();
			
			Camera.main.transform.parent = transform;
			Camera.main.orthographicSize = 15;
			
			InitShip();
		}
		
		void InitShip(){
			int[,] startShip = new int[20,20];
			
			for (int x = 9; x < 14; x++) {
				for (int y = 8; y < 16; y++) {
					startShip[x,y] = 2;
					//voxel.AddVoxel(x+5,y+5,2);
				}
			}

			voxel.SetVoxelGrid(VoxelUtility.IntToVoxelData(startShip, new Ore()));
			Vector2 center = voxel.GetCenter();
			Camera.main.transform.position = transform.TransformPoint(new Vector3(center.x,center.y,-10));
		}

		void FixedUpdate(){
			//rigidbody2D.angularVelocity = 10;
			foreach(Device d in deviceList){
				d.OnUpdate();
			}
		}

		// Update is called once per frame
		void Update () {
			CheckInput();
			
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

			if(Input.GetKeyDown(KeyCode.KeypadPlus)){
				selectedID++;
			}else if(Input.GetKeyDown(KeyCode.KeypadMinus)){
				selectedID--;
			}

			if(Input.GetKeyDown(KeyCode.W)){
				foreach(Device d in deviceList){
					d.OnActivate();
				}
			}else if(Input.GetKeyUp(KeyCode.W)){
				foreach(Device d in deviceList){
					d.OnDeactivate();
				}
			} 

		}

		void AddSelectedVoxelType(int x,int y){
			switch(selectedID){
			case 1:
				voxel.AddVoxel(x,y,1, new Ore());
				break;
			case 2:
				voxel.AddVoxel(x,y,2, new Ore());
				break;
			case 3:
				voxel.AddVoxel(x,y,3, new Ore());
				break;
			case 4:
			
				Engine e = new Engine();
				e.OnStart(new object[]{
					voxel,
					new Vector2(x,y),
					1000f,
					0f
				});
				VoxelData vox = voxel.AddVoxel(x,y,3,e);

				deviceList.Add(e);
				break;
			}
		}

		void OnGUI(){
			GUI.Label(new Rect(Screen.width/2,0,150,20),"Selected block "+selectedID);
		}

	}
	
}