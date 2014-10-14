using UnityEngine;
using System.Collections;

namespace Voxel2D{
	public static class VoxelUtility {


		/// <summary>
		/// Converts and INT array to and voxel data array
		/// </summary>
		/// <returns>The voxel data array.</returns>
		/// <param name="map">Int map.</param>
		public static VoxelData[,] IntToVoxelData(int[,] map)
		{
			int sx = map.GetLength(0);
			int sy = map.GetLength(1);

			VoxelData[,] VD = new VoxelData[sx,sy];
			for (int x = 0; x < sx; x++) {
				for (int y = 0; y < sy; y++) {
					if(map[x,y] != 0){
						VD[x,y] = new VoxelData(map[x,y]);
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
			int[] i = new int[]{(int)point.x,(int)point.y};
			Vector2 m = new Vector2(map.GetLength(0), map.GetLength(1));
			if(i[0]>0 && i[0]<m.x && i[1]>0 && i[1]<m.y){
				return true;
			}else{
				return false;
			}
		}
	
	}
}
