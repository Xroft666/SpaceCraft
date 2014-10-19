using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceSandbox;

namespace Voxel2D{
	public class VoxelBuilderPlayer : MonoBehaviour {
		
		VoxelSystem voxel;

		int selectedID = 0;

		//HACK ALL OVER DA PLACE!!
//		public List<VoxelData> deviceList = new List<VoxelData>();

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

			voxel.SetVoxelGrid(VoxelUtility.IntToVoxelDataOre(startShip,voxel));
			Vector2 center = voxel.GetCenter();
			Camera.main.transform.position = transform.TransformPoint(new Vector3(center.x,center.y,-10));
		}

		void FixedUpdate(){
			foreach(VoxelData d in voxel.voxelGrid)
				if( d != null )
					d.OnUpdate();
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

			if(Input.GetKeyDown(KeyCode.N)){
				selectedID++;
			}else if(Input.GetKeyDown(KeyCode.P)){
				selectedID--;
			}

			if(Input.GetKeyDown(KeyCode.W)){
				foreach(VoxelData d in voxel.voxelGrid)
					if( d != null )
						d.OnActivate();

			}else if(Input.GetKeyUp(KeyCode.W)){
				foreach(VoxelData d in voxel.voxelGrid)
					if( d != null )
						d.OnDeactivate();

			} 

		}

		void AddSelectedVoxelType(int x,int y){
			switch(selectedID){
			case 1:
				Ore o1 = new Ore(1,new IntVector2(x,y),0,voxel);
				voxel.AddVoxel(o1);
				break;
			case 2:
				Ore o2 = new Ore(2,new IntVector2(x,y),0,voxel);
				voxel.AddVoxel(o2);
				break;
			case 3:
				Ore o3 = new Ore(2,new IntVector2(x,y),0,voxel);
				voxel.AddVoxel(o3);
				break;
			case 4:

			
				Engine e = new Engine(3,new IntVector2(x,y),0,voxel,1000);


				VoxelData vox = voxel.AddVoxel(e);

				//deviceList.Add(e);
				break;
			}
		}

		void OnGUI(){
			GUI.Label(new Rect(Screen.width/2,0,150,20),"Selected block "+selectedID);
		}

	}
	
}