﻿//using System;
//using System.Collections;
//using Voxel2D;


// this class evaluates how good a randomized asteroid is
// it checks the density and roundness of an asteroid
public class AsteroidEvaluator 
{
	private static float[,] evaluationData = new float[10,10];

	// Fintess function that returns a value from 0 to 1
	// 0 - does not fit
	// 1 - fits
	// general formula: fitness = (density + roundness) / 2
	static public void Evaluate(float seed, ref int[,] map)
	{
		float weight = 0.0f;

		// searching for the density center
		
		int xAver = 0;
		int yAver = 0;



		int voxelsCount = 0;
		for( int i = map.GetLength(0) - 1; i >= 0 ; i-- )
		{
			for( int j = 0; j < map.GetLength(1); j++ )
			{
				if( map[j,i] == 1 )
				{
					xAver +=j;
					yAver +=i;
					voxelsCount ++;
				}
			}
		}
		
		xAver /= voxelsCount;
		yAver /= voxelsCount;


		float density = EvaluateDensity( xAver, yAver, seed, ref map );
		float roundness = EvaluateRoundness( xAver, yAver, seed, ref map );

		evaluationData[(int) (density * evaluationData.GetLength(0)), (int) (roundness * evaluationData.GetLength(1))] += 1f;

		DebugMap( ref map);
		UnityEngine.Debug.Log("density: " + density + ", roundness: " + roundness);
	}

	// tells how dense an asteroid is. If it contains a lot of holes,
	// then the density drops
	static private float EvaluateDensity(int xAver, int yAver, float seed, ref int[,] map)
	{
		int size = map.GetLength(0);
		int transX;
		int transY;
	
		// calculating density ( white voxels to black voxels relation )
		// currently supports only quad maps

		// horizontal line through the center

		int voxelsCount = 0;
		int objectsCount = 0;


		for( int i = 0; i <= size; i++ )
		{
			// if voxel is solid on the edge of the map, we count it as an object
			if( i == map.GetLength(0) )
			{
				if( map[i - 1, yAver] == 1 )
					objectsCount++;
				break;
			}

			// if there is a hole after a solid voxel, we increment objects count
			if( i > 0 )
				if( map[i - 1, yAver] != map[i, yAver] && map[i, yAver] == 0 )
					objectsCount++;

			// general voxels count
			if( map[i, yAver] == 1 )
				voxelsCount ++;
		}

//		// vertical line through the center
//
		for( int i = 0; i <= size; i++ )
		{
			// if voxel is solid on the edge of the map, we count it as an object
			if( i == map.GetLength(1) )
			{
				if( map[xAver, i - 1] == 1 )
					objectsCount++;
				break;
			}
			
			// if there is a hole after a solid voxel, we increment objects count
			if( i > 0 )
				if( map[xAver, i - 1] != map[xAver, i] && map[xAver, i] == 0 )
                    objectsCount++;
            
            // general voxels count
            if( map[xAver, i] == 1 )
                voxelsCount ++;
		}

		// first diagonal line (from bottom left to upper right)


		//defines the diagonal starting point (x, 0) or (0, y)
		if (xAver < yAver) {
			transX = 0;
			transY = yAver - xAver;
				}
		else if (xAver > yAver) {
			transX = xAver - yAver;
			transY = 0;
				} 
		else {
			transX = 0;
			transY = 0;
		}

		for (int i = 0; i <= (size - transX - transY ); i++)
		{

			if( i == size - transX - transY)
			{
				if( map[transX + i - 1, transY + i - 1] == 1 )
					objectsCount++;
				break;
			}


			if (i > 0) {
					if (map [transX + i - 1, transY + i - 1] != map [transX + i, transY + i] && map [transX + i, transY + i] == 0)
							objectsCount++;

					if (map [transX + i, transY + i] == 1)
							voxelsCount++;
			}
		}


		//second diagonal line (from upper left to bottom right)

		if (xAver + yAver <= size) {

				for (int i = 0; i < yAver + xAver; i++) {

				if( i == yAver + xAver )
						{
					if( map[i - 1, xAver + yAver - i + 1] == 1 )
							objectsCount++;
							break;
						}


						if (i > 0) {

								if (map [i - 1, xAver + yAver - i + 1] != map [i, xAver + yAver - i] && map [i, xAver + yAver - i] == 0) //not sure about "-i" after !=
										objectsCount++;

								if (map [i, xAver + yAver - i] == 1)
										voxelsCount++;
						}
				}

		} 
		else 
		{
			for (int j = 0; j <= (2*size - (xAver + yAver)); j++)
			{
				if( j == 2*size - (xAver + yAver))
				{
					if( map [yAver - (size - xAver) + j - 1, size - j] == 1 )
						objectsCount++;
					break;
				}

				if (j > 0)
				{
				
					if (map [yAver - (size - xAver) + j - 1, size - j] != map [yAver - (size - xAver) + j, size - j - 1] && map [yAver - (size - xAver) + j, size - j - 1]  == 0)
						objectsCount++;
					if(map [yAver - (size - xAver) + j, size - j - 1] == 1)
						voxelsCount++;
				}
			}

		}

        
        // general density is solid voxels to empty voxels relation
		float lineDensity = voxelsCount / (float)size;

		// density to objects count relation would be the average density per object
		return lineDensity / (float) objectsCount;
    }
    
    // check how the asteroid fits into a specified circle radius
	// we raycast from 8 angles and store the position of the first solid voxel
	// then we compare the distances to the center with each other and say how round it is
	static private float EvaluateRoundness(int xAver, int yAver, float seed, ref int[,] map)
	{
		int size = map.GetLength(0);
		int transX;
		int transY;

		float leftToRightRad = size, rightToLeftRad = size, topToBotRad = size, botToTopRad = size;
		float diagDownLeftToRightRad = size, diagUpRightToLeftRad = size;
		float diagDownRightToLeftRad = size, diagUpLeftToRightRad = size;

		// horizontal scan lines
		for( int i = 0; i < size; i++ )
		{
			if( map[i, yAver] == 1 )
			{
				leftToRightRad = UnityEngine.Mathf.Abs( i - xAver );
				break;
			}
		}

		for( int i = size - 1; i >= 0; i-- )
		{
			if( map[i, yAver] == 1 )
			{
				rightToLeftRad = UnityEngine.Mathf.Abs( i - xAver );
				break;
			}
		}

		// vertical scan lines

		for( int i = 0; i < size; i++ )
		{
			if( map[xAver, i] == 1 )
			{
				topToBotRad = UnityEngine.Mathf.Abs( i - yAver );
				break;
			}
		}

		for( int i = size - 1; i >= 0; i-- )
		{
			if( map[xAver, i] == 1 )
			{
				botToTopRad = UnityEngine.Mathf.Abs( i - yAver );
				break;
			}
		}

		// diagonal scan lines go here


		// first diagonal line (from bottom left to upper right)

		UnityEngine.Vector2 center = new UnityEngine.Vector2(xAver, yAver);
		UnityEngine.Vector2 asteroidEdge;
		
		//defines the diagonal starting point (x, 0) or (0, y)
		if (xAver < yAver) {
			transX = 0;
			transY = yAver - xAver;
		}
		else if (xAver > yAver) {
			transX = xAver - yAver;
			transY = 0;
		} 
		else {
			transX = 0;
			transY = 0;
		}
		
		for (int i = 0; i <= (size - transX - transY ); i++)
		{
			if(map[transX + i, transY + i] == 1)
			{
				asteroidEdge = new UnityEngine.Vector2 (transX + i, transY + i);
				diagDownLeftToRightRad = new UnityEngine.Vector2( center.x - asteroidEdge.x, center.y - asteroidEdge.y).magnitude;
				break;
			}
		}

		for (int i = (size - transX - transY - 1); i >= 0; i--)
		{
			if(map[transX + i,transY + i] == 1)
			{
				asteroidEdge = new UnityEngine.Vector2 (transX + i, transY + i);
				diagUpRightToLeftRad = new UnityEngine.Vector2 ( center.x - asteroidEdge.x, center.y - asteroidEdge.y).magnitude;
				break;
			}
		}




		//second diagonal line (from upper left to bottom right)
		
		if (xAver + yAver <= size) {

			int offset;
			for (int i = 0; i < yAver + xAver; i++) {

				if(map[i, xAver + yAver - i] == 1)
				{
					asteroidEdge = new UnityEngine.Vector2 (i, xAver + yAver - i);
					diagUpLeftToRightRad = new UnityEngine.Vector2 (center.x - asteroidEdge.x, center.y - asteroidEdge.y).magnitude;
					break;
				}
			}

			for (int i = yAver + xAver - 1; i > 0; i--){

				if (map[i,size - i -1] == 1)
				{
					asteroidEdge = new UnityEngine.Vector2 (transX + transY - i ,i);
					diagDownRightToLeftRad = new UnityEngine.Vector2 (center.x - asteroidEdge.x, center.y - asteroidEdge.y).magnitude;
					break;
				}
			}
			
		} 
		else 
		{
			for (int j = 0; j <= (2*size - (xAver + yAver)); j++)
			{
				if (map[size - (2*size - (xAver + yAver)) + j, size -1 - j] == 1)
				{
					asteroidEdge = new UnityEngine.Vector2 (size - (2*size - (xAver + yAver)) + j, size -1 - j);
					diagUpLeftToRightRad = new UnityEngine.Vector2 (center.x - asteroidEdge.x, center.y - asteroidEdge.y).magnitude;
					break;
				}

			}

			for(int j = (2*size - (xAver + yAver)) - 1; j >= 0; j-- )
			{
				if(map[size -1 - j, size - (2*size - (xAver + yAver)) + j]  == 1)
				{
					asteroidEdge = new UnityEngine.Vector2 (size -1 - j, size - (2*size - (xAver + yAver)) + j);
					diagDownRightToLeftRad = new UnityEngine.Vector2 (center.x - asteroidEdge.x, center.y - asteroidEdge.y).magnitude;
					break;
				}
			}
			
		}



		// we take the minimum and the maximum radiuses.
		// the roundess is told by min / max relation

		float minRadius = UnityEngine.Mathf.Min(leftToRightRad, rightToLeftRad, topToBotRad, botToTopRad, diagUpLeftToRightRad, diagUpRightToLeftRad, diagDownRightToLeftRad, diagDownLeftToRightRad);
		float maxRadius = UnityEngine.Mathf.Max(leftToRightRad, rightToLeftRad, topToBotRad, botToTopRad, diagUpLeftToRightRad, diagUpRightToLeftRad, diagDownRightToLeftRad, diagDownLeftToRightRad);

		return minRadius / maxRadius;
    }

	static public void CollectData( ref int[,] map )
	{
		Evaluate( 0, ref map );
	}

	static public void ClearData()
	{
		for( int i = 0; i < evaluationData.GetLength(0); i++ )
			for( int j = 0; j < evaluationData.GetLength(1); j++ )
				evaluationData[i,j] = 0f;
	}

	static public float[,] GetNormalizedData()
	{

		float[,] normalizedData = new float[evaluationData.GetLength(0),evaluationData.GetLength(1)];

		float maxValue = float.MinValue;

		// seeking the maximum value to normalize to
		for( int i = 0; i < evaluationData.GetLength(0); i++ )
			for( int j = 0; j < evaluationData.GetLength(1); j++ )
				if( evaluationData[i,j] > maxValue )
					maxValue = evaluationData[i,j];

		for( int i = 0; i < normalizedData.GetLength(0); i++ )
			for( int j = 0; j < normalizedData.GetLength(1); j++ )
				normalizedData[i,j] = evaluationData[i,j] / maxValue;

		return normalizedData;
	}
    
    static public void DebugMap(ref int[,] map)
	{
		for( int i = map.GetLength(0) - 1; i >= 0 ; i-- )
		{
			string line = "";
			for( int j = 0; j < map.GetLength(1); j++ )
			{
				line += " " + map[j,i];
            }
            
            UnityEngine.Debug.Log(line);
        }
	}
}
