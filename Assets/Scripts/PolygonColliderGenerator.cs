using UnityEngine;
using System.Collections.Generic;

//-------------------------------------------------------------------------
/// <summary>
/// A component to generate a MeshCollider from an image with alpha channel
/// at runtime.
/// 
/// NOTE: This is experimental code - don't expect it to be perfect or
/// anything close yet.
/// 
/// TODO: Parallelization / coroutine.
/// </summary>

[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonColliderGenerator{
	
	protected int mUpdateCounter = 0;
	
	public bool mUseBinaryImageInsteadOfTexture = true; //< When set to true, the mUsedTexture is ignored and the mBinaryImage attribute is used directly.
	public bool mOutputColliderInNormalizedSpace = true;
	public bool [,] mBinaryImage = null; //< If you want to set the collider-image directly, set mUseBinaryImageInsteadOfTexture=true and fill this attribute accordingly.
	public int mMaxNumberOfIslands = 10;
	public int mMinPixelCountToIncludeIsland = 0;
	public float mColliderThickness = 100.0f;
	public int gridSize = 0;
	
	public float mVertexReductionDistanceTolerance = 0.0f;
	public int mMaxPointCountPerIsland = 2000;
	
	protected PolygonOutlineFromImageFrontend mOutlineAlgorithm = new PolygonOutlineFromImageFrontend();
	protected IslandDetector mIslandDetector = new IslandDetector();
	
	protected IslandDetector.Region[] mIslands = null;
    protected IslandDetector.Region[] mSeaRegions = null;
	public List<List<Vector2> > vertexPaths = new List<List<Vector2> >();



	public PolygonColliderGenerator(int gridSize, bool[,] mBinaryImage){
		this.gridSize = gridSize;
		this.mBinaryImage = mBinaryImage;

	}
	
	//-------------------------------------------------------------------------
	/// <summary>
	/// Updates the mesh collider. Call this method from your code accordingly.
	/// </summary>
	/// <returns>
	/// The alpha mesh collider to texture.
	/// </returns>
	public void UpdateMeshCollider() {
		
		
		bool anyIslandsFound = CalculateIslandStartingPoints(mBinaryImage, out mIslands, out mSeaRegions);
        if (!anyIslandsFound) {
			Debug.LogError("Error: No opaque pixel (and thus no island region) has been found in the texture image - is your mAlphaOpaqueThreshold parameter too high?. Stopping collider generation.");
            return;
        }
		
		mOutlineAlgorithm.VertexReductionDistanceTolerance = mVertexReductionDistanceTolerance;
		mOutlineAlgorithm.MaxPointCount = mMaxPointCountPerIsland;
		mOutlineAlgorithm.Convex = false;
		mOutlineAlgorithm.XOffsetNormalized = -0.5f;
		mOutlineAlgorithm.YOffsetNormalized = -0.5f;
		mOutlineAlgorithm.Thickness = mColliderThickness;
		
		bool anyIslandVerticesAdded = CalculateOutlineForColliderIslands(out vertexPaths, mIslands, mBinaryImage);
		if (!anyIslandVerticesAdded) {
			Debug.LogError("Error: No island vertices added in CalculateUnreducedOutlineForColliderIslands - is your mMinPixelCountToIncludeIsland parameter too low (currently set to " + mMinPixelCountToIncludeIsland + ")?. Stopping collider generation.");
            return;
        }

		InvertAndScalePoints(gridSize);

	}

	void InvertAndScalePoints(int gridSize){
		for (int i = 0; i < vertexPaths.Count; i++) {
			for (int j = 0; j < vertexPaths[i].Count; j++) {
				Vector2 t = vertexPaths[i][j];
				t.x = vertexPaths[i][j].y*gridSize+(gridSize/2-0.5f);
				t.y = vertexPaths[i][j].x*gridSize+(gridSize/2-0.5f);
				vertexPaths[i][j] = t;
			}
		}
	}

	//-------------------------------------------------------------------------
    /// <returns>True if at least one island is found, false otherwise.</returns>
	bool CalculateIslandStartingPoints(bool [,] binaryImage, out IslandDetector.Region[] islands, out IslandDetector.Region[] seaRegions) {
		
		int[,] islandClassificationImage = null;
		islands = null;
		seaRegions = null;
		
		mIslandDetector.DetectIslandsFromBinaryImage(binaryImage, out islandClassificationImage, out islands, out seaRegions);

        return (islands.Length > 0);
	}
	
	//-------------------------------------------------------------------------
	bool CalculateOutlineForColliderIslands(out List<List<Vector2> > outlineVerticesAtIsland, IslandDetector.Region[] islands, bool [,] binaryImage) {
		
		outlineVerticesAtIsland = new List<List<Vector2> >();
		
		for (int islandIndex = 0; islandIndex < islands.Length; ++islandIndex) {
			
			IslandDetector.Region island = islands[islandIndex];
			
			if (islandIndex >= mMaxNumberOfIslands || island.mPointCount < mMinPixelCountToIncludeIsland) {
				break; // islands are sorted by size already, only smaller islands follow.
			}
			else {
				List<Vector2> unreducedOutlineVertices;
	            mOutlineAlgorithm.UnreducedOutlineFromBinaryImage(out unreducedOutlineVertices, binaryImage, island.mPointAtBorder, true, mOutputColliderInNormalizedSpace, true);
				
				List<Vector2> reducedVertices = mOutlineAlgorithm.ReduceOutline(unreducedOutlineVertices, true);
				outlineVerticesAtIsland.Add(reducedVertices);
			}
        }
		return outlineVerticesAtIsland.Count > 0;
	}
	
	//-------------------------------------------------------------------------

}
