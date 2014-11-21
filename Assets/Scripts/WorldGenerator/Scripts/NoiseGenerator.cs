using UnityEngine;
using System.Collections;
using Random = System.Random;

namespace WorldGen
{
public static class NoiseGenerator{

    public static int[,] GenerateNoise(float seed, int[,] map, float whiteChance)
    {
        Random rand = new System.Random((int)seed);

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                float r = (float)rand.NextDouble();
                int i;
                if (r > whiteChance)
                {
                    i = 1;
                }
                else
                {
                    i = 0;
                }

                    
                map[x, y] = i;
            }
        }
        return map;
    }
}
}