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

		private float _minBlackChangeThreshold = 0.4f;
		private float _maxBlackChangeThreshold = 0.6f;
		private float _minWhiteChangeThreshold = 0.4f;
		private float _maxWhiteChangeThreshold = 0.6f;
		private int _minRadius = 1;
		private int _maxRadius = 5;
		private int _minRounds = 1;
		private int _maxRounds = 5;

        public void Ranomize(int mapSize)
        {
			BlackChangeThreshold = Random.Range(_minBlackChangeThreshold, _maxBlackChangeThreshold);
			WhileChangeThreshold = Random.Range(_minWhiteChangeThreshold, _maxWhiteChangeThreshold);
			Radius = Random.Range(_minRadius, _maxRadius);
			Rounds = Random.Range(_minRounds, _maxRounds);
        }

		public void InitializeSampling(int paramIdx)
		{
			switch( paramIdx )
			{
			case 0:

				BlackChangeThreshold = _minBlackChangeThreshold;
				break;
			case 1:
				WhileChangeThreshold = _minWhiteChangeThreshold;
				break;
			case 2:
				Radius = _minRadius;
				break;
			case 3:
				Rounds = _minRounds;
				break;
			default:
				break;
			}
		}

		public void IncrementParameter( int paramIndex, int samplesCount )
		{
			switch( paramIndex )
			{
			case 0:
				BlackChangeThreshold += (_maxBlackChangeThreshold - _minBlackChangeThreshold) / (float) samplesCount;
				break;
			case 1:
				WhileChangeThreshold += (_maxWhiteChangeThreshold - _minWhiteChangeThreshold) / (float) samplesCount;
			
				break;
			case 2:
				Radius += (int)( (_maxRadius - _minRadius) / (float) samplesCount);
				break;
			case 3:
				Rounds += (int)((_maxRadius - _minRadius) / (float) samplesCount);
				break;
			default:
				break;
			}
		}
    }
}

