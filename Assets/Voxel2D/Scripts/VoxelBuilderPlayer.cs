using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Voxel2D{
	public class VoxelBuilderPlayer : MonoBehaviour {
		
		VoxelSystem voxel;
		
		// Use this for initialization
		void Start () {
			
			voxel = gameObject.AddComponent<VoxelSystem>();
			
			Camera.main.transform.parent = transform;
			Camera.main.orthographicSize = 15;
			
			InitShip();
		}
		
		void InitShip(){
			int[,] startShip = new int[20,20];
			
			for (int x = 10; x < 14; x++) {
				for (int y = 8; y < 16; y++) {
					startShip[x,y] = 2;
					//voxel.AddVoxel(x+5,y+5,2);
				}
			}
			
			voxel.SetVoxelGrid(VoxelUtility.IntToVoxelData(startShip));
		}

		void FixedUpdate(){
			rigidbody2D.angularVelocity = 10;
		}

		// Update is called once per frame
		void Update () {
			CheckInput();
			
			Vector2 center = voxel.GetCenter();
			Camera.main.transform.position = transform.TransformPoint(new Vector3(center.x,center.y,-10));
		}
		
		void CheckInput(){
			Vector2 point = Input.mousePosition;
			Vector3 world = Camera.main.ScreenToWorldPoint(point);
			Vector3 localPos = transform.InverseTransformPoint(world);
			IntVector2 RL = new IntVector2(Mathf.RoundToInt(localPos.x),Mathf.RoundToInt(localPos.y));
			
			if(Input.GetMouseButton(0)){
				if(!voxel.IsVoxelEmpty(RL.x,RL.y)){
					voxel.RemoveVoxel(RL.x,RL.y);
					List<bool[,]> islands = VoxelIslandDetector.findIslands(VoxelUtility.VoxelDataToBool(voxel.GetVoxelData()));
					if(islands.Count == 1){

					}else{
						VoxelData[,] vox = VoxelIslandDetector.SplitIslands(voxel.GetVoxelData(),voxel);
						voxel.SetVoxelGrid(vox);
					}
				}
			}else if(Input.GetMouseButton(1)){
				if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),localPos)){
					if(VoxelUtility.NextToVoxel(voxel.GetVoxelData(),RL)){
						if(voxel.IsVoxelEmpty(RL.x,RL.y)){
							voxel.AddVoxel(RL.x,RL.y,1);
						}
					}
				}
			}
		}
	}
	
}