using UnityEngine;
using System.Collections;

namespace Voxel2D{
	public static class VoxelUtility {


		/// <summary>
		/// Converts and INT array to and voxel data array
		/// </summary>
		/// <returns>The voxel data array.</returns>
		/// <param name="map">Int map.</param>
		public static VoxelData[,] IntToVoxelData(int[,] map, SpaceSandbox.Device device)
		{
			int sx = map.GetLength(0);
			int sy = map.GetLength(1);

			VoxelData[,] VD = new VoxelData[sx,sy];
			for (int x = 0; x < sx; x++) {
				for (int y = 0; y < sy; y++) {
					if(map[x,y] != 0){
						VD[x,y] = new VoxelData(map[x,y], new IntVector2(x,y), device);
						//Debug.Log(VD[x,y].GetID());
					}
				}
			}
			return VD;
		}

		public static bool[,] VoxelDataToBool(VoxelData[,] map)
		{
			int sx = map.GetLength(0);
			int sy = map.GetLength(1);
			
			bool[,] binaryMap = new bool[sx,sy];
			for (int x = 0; x < sx; x++) {
				for (int y = 0; y < sy; y++) {
					if(map[x,y] != null){
						binaryMap[x,y] = true;
					}
				}
			}
			return binaryMap;
		}
	
	
		public static bool IsPointInBounds(VoxelData[,] map, Vector2 point){
			int[] i = new int[]{Mathf.RoundToInt(point.x),Mathf.RoundToInt(point.y)};
			Vector2 m = new Vector2(map.GetLength(0), map.GetLength(1));
			if(i[0]>0 && i[0]<m.x && i[1]>0 && i[1]<m.y){
				return true;
			}else{
				return false;
			}
		}

		public static bool IsPointInBounds(int size, Vector2 point){
			int[] i = new int[]{Mathf.RoundToInt(point.x),Mathf.RoundToInt(point.y)};
			if(i[0]>0 && i[0]<size && i[1]>0 && i[1]<size){
				return true;
			}else{
				return false;
			}
		}

		public static GameObject CreateFragment(VoxelData vox, Vector3 position, VoxelSystem voxel){
			GameObject frag = new GameObject(voxel.gameObject.name+" Fragment");
			frag.transform.position = position;
			frag.transform.rotation = voxel.transform.rotation;
			frag.tag = "VoxelFragment";
			
			GameObject parent = GameObject.Find("FragmentHolder");
			if(parent == null){
				parent = new GameObject("FragmentHolder");
			}
			frag.transform.parent = parent.transform;
			
			VoxelFragment f = frag.AddComponent<VoxelFragment>();
			f.Init(vox.GetElementID(),vox.device);

			frag.rigidbody2D.velocity = voxel.rigidbody2D.velocity+new Vector2(Random.Range(-50,50),Random.Range(-50,50));
			frag.rigidbody2D.angularVelocity = voxel.rigidbody2D.angularDrag + Random.Range(-100,100);
			return frag;
		}

		public static bool IsPosNextToVoxel(VoxelData[,] map, IntVector2 pos){

			if(IsPointInBounds(map.GetLength(0),new Vector2(pos.x+1,pos.y))){
				if(map[pos.x+1,pos.y] != null){
					return true;
				}
			}
			if(IsPointInBounds(map.GetLength(0),new Vector2(pos.x-1,pos.y))){
				if(map[pos.x-1,pos.y] != null){
					return true;
				}
			}
			if(IsPointInBounds(map.GetLength(0),new Vector2(pos.x,pos.y+1))){
				if(map[pos.x,pos.y+1] != null){
					return true;
				}
			}
			if(IsPointInBounds(map.GetLength(0),new Vector2(pos.x,pos.y-1))){
				if(map[pos.x,pos.y-1] != null){
					return true;
				}
			}
			return false;

		}

	}
}
