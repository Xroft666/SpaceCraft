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
            BlackChangeThreshold = Random.Range(0.4f, 0.6f);
            WhileChangeThreshold = Random.Range(0.4f, 0.6f);
            Radius = Random.Range(1, 5);
            Rounds = Random.Range(1, 5);
        }

		public void InitializeSampling()
		{
			BlackChangeThreshold = 0.4f;
			WhileChangeThreshold = 0.4f;
			Radius = 1;
			Rounds = 1;
		}

		public void IncrementParameter( int paramIndex, int samplesCount )
		{
			switch( paramIndex )
			{
			case 0:
				BlackChangeThreshold += (0.6f - 0.4f) / (float) samplesCount;
				break;
			case 1:
				WhileChangeThreshold += (0.6f - 0.4f) / (float) samplesCount;
			
				break;
			case 2:
				Radius += (int)( (5 - 1) / (float) samplesCount);
				break;
			case 3:
				Rounds += (int)((5 - 1) / (float) samplesCount);
				break;
			default:
				break;
			}
		}

		public void SetParameter( int paramIndex, float val )
		{
			switch( paramIndex )
			{
			case 0:
				BlackChangeThreshold = Mathf.Clamp(val, 0.4f, 0.6f);
				break;
			case 1:
				WhileChangeThreshold = Mathf.Clamp(val, 0.4f, 0.6f);
				break;
			case 2:
				Radius = Mathf.Clamp((int)val, 1, 5);
				break;
			case 3:
				Rounds = Mathf.Clamp((int)val, 1, 5);
				break;
			default:
				break;
			}
		}
    }
}

