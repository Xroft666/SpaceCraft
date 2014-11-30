//using System;
//using System.Collections;
//using Voxel2D;


// this class evaluates how good a randomized asteroid is
// it checks the density and roundness of an asteroid
public class AsteroidEvaluator 
{
	// Fintess function that returns a value from 0 to 1
	// 0 - does not fit
	// 1 - fits
	// general formula: fitness = (density + roundness) / 2
	static public float Evaluate(float seed, ref int[,] map)
	{
		float weight = 0.5f;

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

		return UnityEngine.Mathf.Lerp(density, roundness, weight);
	}

	// tells how dense an asteroid is. If it contains a lot of holes,
	// then the density drops
	static private float EvaluateDensity(int xAver, int yAver, float seed, ref int[,] map)
	{
		int size = map.GetLength(0);
	
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

		// vertical line through the center

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

		// first diagonal line

//		for( int i = 0; i <= size; i++ )
//		{
//			// if voxel is solid on the edge of the map, we count it as an object
//			if( i == map.GetLength(0) )
//			{
//				if( map[i - 1, i - 1] == 1 )
//					objectsCount++;
//				break;
//			}
//			
//			// if there is a hole after a solid voxel, we increment objects count
//			if( i > 0 )
//				if( map[i - 1, i - 1] != map[i, i] && map[i, i] == 0 )
//                    objectsCount++;
//            
//            // general voxels count
//            if( map[i, i] == 1 )
//                voxelsCount ++;
//		}
//
//		// second diagonal line
//
//		for( int i = 0; i <= size; i++ )
//		{
//			// if voxel is solid on the edge of the map, we count it as an object
//			if( i == size )
//			{
//				if( map[i - 1, size - i] == 1 )
//					objectsCount++;
//				break;
//			}
//			
//			// if there is a hole after a solid voxel, we increment objects count
//			if( i > 0 )
//				if( map[i - 1, size - i] != map[i, size - i - 1] && map[i, size - i - 1] == 0 )
//					objectsCount++;
//            
//            // general voxels count
//			if( map[i, size - i - 1] == 1 )
//                voxelsCount ++;
//        }
        
        // general density is solid voxels to empty voxels relation
		float lineDensity = voxelsCount / (float) map.GetLength(0);

		// density to objects count relation would be the average density per object
		return lineDensity / (float) objectsCount;
    }
    
    // check how the asteroid fits into a specified circle radius
	// we raycast from 8 angles and store the position of the first solid voxel
	// then we compare the distances to the center with each other and say how round it is
	static private float EvaluateRoundness(int xAver, int yAver, float seed, ref int[,] map)
	{
		int size = map.GetLength(0);

		float leftToRightRad = size, rightToLeftRad = size, topToBotRad = size, botToTopRad = size;

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
		//...


		// we take the minimum and the maximum radiuses.
		// the roundess is told by min / max relation

		float minRadius = UnityEngine.Mathf.Min(leftToRightRad, rightToLeftRad, topToBotRad, botToTopRad);
		float maxRadius = UnityEngine.Mathf.Max(leftToRightRad, rightToLeftRad, topToBotRad, botToTopRad);

		return minRadius / maxRadius;
    }
    
    static private void DebugMap(ref int[,] map)
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
