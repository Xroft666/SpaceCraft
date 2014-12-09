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

		private int _minOctNum = 1;
		private int _maxOctNum = 25;
		private float _minFrq = 0.5f;
		private float _maxFrq = 25f;
		private float _minAmp = 0.5f;
		private float _maxAmp = 25f;

        public void Randomize(int mapSize)
        {
			octNum = Random.Range(_minOctNum, _maxOctNum);
			frq = Random.Range(_minFrq, _maxFrq);
			amp = Random.Range(_minAmp, _maxAmp);
		}

		public void InitializeSampling(int paramIdx)
		{
			switch(paramIdx)
			{
			case 0:	
				octNum = _minOctNum;
				break;
			case 1:
				frq = _minFrq;
				break;
			case 2:
				amp = _minAmp;
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
				octNum += (int)(( _maxOctNum - _minOctNum ) / (float) samplesCount);
				
				break;
			case 1:
				frq += (_maxFrq - _minFrq ) / (float) samplesCount;
				break;
			case 2:
				amp += (_maxAmp - _minAmp) / (float) samplesCount;
				break;

			default:
				break;
			}
		}
    }
}
