using UnityEngine;
using System.Collections;

public static class VoxelIslandDetector{


	public static bool CalculateIslandStartingPoints(bool [,] binaryImage, out IslandDetector.Region[] islands, out IslandDetector.Region[] seaRegions) {
		IslandDetector mIslandDetector = new IslandDetector();

		int[,] islandClassificationImage = null;
		islands = null;
		seaRegions = null;
		
		mIslandDetector.DetectIslandsFromBinaryImage(binaryImage, out islandClassificationImage, out islands, out seaRegions);
		return (islands.Length > 0);	
	}

}
