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
	static public float Evaluate(float seed, ref int[,] map, float radius)
	{
		float density = EvaluateDensity( seed, ref map );
		float roundness = EvaluateRoundness( seed, ref map, radius );

		return (density + roundness) ;/// 2f;
	}

	// tells how dense an asteroid is. If it contains a lot of holes,
	// then the density drops
	static private float EvaluateDensity(float seed, ref int[,] map)
	{
		// searching for the density center

		int xAver = 0;
		int yAver = 0;
		int voxelsCount = 0;

		int size = map.GetLength(0);

		for( int i = size - 1; i >= 0 ; i-- )
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

		// calculating density ( white voxels to black voxels relation )
		// currently supports only quad maps

		// horizontal line through the center

		voxelsCount = 0;
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

		for( int i = 0; i <= size; i++ )
		{
			// if voxel is solid on the edge of the map, we count it as an object
			if( i == map.GetLength(0) )
			{
				if( map[i - 1, i - 1] == 1 )
					objectsCount++;
				break;
			}
			
			// if there is a hole after a solid voxel, we increment objects count
			if( i > 0 )
				if( map[i - 1, i - 1] != map[i, i] && map[i, i] == 0 )
                    objectsCount++;
            
            // general voxels count
            if( map[i, i] == 1 )
                voxelsCount ++;
		}

		// second diagonal line

		for( int i = 0; i <= size; i++ )
		{
			// if voxel is solid on the edge of the map, we count it as an object
			if( i == size )
			{
				if( map[i - 1, size - i] == 1 )
					objectsCount++;
				break;
			}
			
			// if there is a hole after a solid voxel, we increment objects count
			if( i > 0 )
				if( map[i - 1, size - i] != map[i, size - i - 1] && map[i, size - i - 1] == 0 )
					objectsCount++;
            
            // general voxels count
			if( map[i, size - i - 1] == 1 )
                voxelsCount ++;
        }
        
        // general density is solid voxels to empty voxels relation
		float lineDensity = voxelsCount / (float) map.GetLength(0);

		// density to objects count relation would be the average density per object
		return lineDensity / (float) objectsCount;
    }
    
    // check how the asteroid fits into a specified circle radius
	static private float EvaluateRoundness(float seed, ref int[,] map, float radius)
	{
		for( int i = map.GetLength(0) - 1; i >= 0 ; i-- )
		{
			for( int j = 0; j < map.GetLength(1); j++ )
			{
		
			}
        }

        return 0f;
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
