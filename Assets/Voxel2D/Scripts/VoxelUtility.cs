using UnityEngine;
using System.Collections;

namespace Voxel2D{
	public static class VoxelUtility {
		
		public static VoxelData[,] IntToVoxelData(int[,] map)
		{
			//TODO: create a voxel utility class and make this into a method
			int sx = map.GetLength(0);
			int sy = map.GetLength(1);

			VoxelData[,] VD = new VoxelData[sx,sy];
			for (int x = 0; x < sx; x++) {
				for (int y = 0; y < sy; y++) {
					if(map[x,y] != 0){
						VD[x,y] = new VoxelData(1);
					}
				}
			}
			return VD;
		}
	}
}
