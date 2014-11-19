using UnityEngine;
using System.Collections;
using Random = System.Random;

namespace WorldGen
{
public static class NoiseGenerator{

    public static int[,] GenerateNoise(float seed, int[,] map)
    {
        Random rand = new System.Random((int)seed);

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                int r = Mathf.RoundToInt((float)rand.NextDouble());
                map[x, y] = r;
            }
        }
        return map;
    }
}
}