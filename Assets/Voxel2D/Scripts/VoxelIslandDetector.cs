using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class VoxelIslandDetector{


	public static bool CalculateIslandStartingPoints(bool [,] binaryImage, out IslandDetector.Region[] islands, out IslandDetector.Region[] seaRegions) {
		IslandDetector mIslandDetector = new IslandDetector();

		int[,] islandClassificationImage = null;
		islands = null;
		seaRegions = null;
		
		mIslandDetector.DetectIslandsFromBinaryImage(binaryImage, out islandClassificationImage, out islands, out seaRegions);
		return (islands.Length > 0);	
	}

	public static List<bool[,]> findIslands(bool[,] land) {
		List<bool[,]> islands = new List<bool[,]>();
		bool[,] visited  = new bool[land.GetLength(0),land.GetLength(0)];
		for(int x=0;x<land.GetLength(0);x++){
			for(int y=0;y<land.GetLength(1);y++){
				if(land[x,y] && !visited[x,y]) { // new island detected
					bool[,] island = new bool[land.GetLength(0),land.GetLength(1)];
					visitNeighbours(x, y, land, ref visited, ref island);
					islands.Add(island);
				} 
			}
		}
		return islands;
	}
	
	private static void visitNeighbours(int x, int y, bool[,] land,ref bool[,] visited,ref bool[,] island) {
		if(land[x,y] && !visited[x,y]) {
			visited[x,y] = true;
			island[x,y] = true;
			if(x<land.GetLength(0)-1){
				visitNeighbours(x+1, y, land, ref visited,ref island);
			}
			if(x>0){
				visitNeighbours(x-1, y, land, ref visited,ref island);
			}
			if(y<land.GetLength(1)-1){
				visitNeighbours(x, y+1, land, ref visited,ref island);
			}
			if(y>0){
				visitNeighbours(x, y-1, land, ref visited,ref island);
			}
 
		}else if(!visited[x,y]){
			visited[x,y] = true;
		}
	}


}
