using UnityEngine;
using System.Collections;

namespace WorldGen
{
    [System.Serializable]
    public class PerlinNoiseStats
    {
        public int octNum = 5;
        public float frq = 5;
        public float amp = 5;

        public void Randomize(int mapSize)
        {
            octNum = Random.Range(1, 25);
            frq = Random.Range(0.5f, 25f);
            amp = Random.Range(0.5f, 25f);
        }
    }
}
