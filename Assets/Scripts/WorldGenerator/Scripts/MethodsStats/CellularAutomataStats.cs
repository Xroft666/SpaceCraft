using UnityEngine;
using System.Collections;

namespace WorldGen
{
    [System.Serializable]
    public class CellularAutomataStats
    {
       
        public enum NeighborType { Adjecent, Square, Circle };
        public CellularAutomata.NeighborType neighborType;

        public string name; 

        public float BlackChangeThreshold = 0.55f;
		public	float WhileChangeThreshold = 0.55f;
		public int Radius = 2;
		public int Rounds = 10;

        public void Ranomize(int mapSize)
        {
            BlackChangeThreshold = Random.Range(0f, 1f);
            WhileChangeThreshold = Random.Range(0f, 1f);
            Radius = Random.Range(1, mapSize);
            Rounds = Random.Range(1, 15);
        }
    }
}

