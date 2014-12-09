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

		public void InitializeSampling()
		{
			octNum = 1;
			frq = 0.5f;
			amp = 0.5f;
		}

		public void IncrementParameter( int paramIndex, int samplesCount )
		{
			switch( paramIndex )
			{
			case 0:
				octNum += (int)(( 25 - 1 ) / (float) samplesCount);
				
				break;
			case 1:
				frq += (25f - 0.5f ) / (float) samplesCount;
				break;
			case 2:
				amp += (25f - 0.5f) / (float) samplesCount;
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
				octNum = Mathf.Clamp((int)val, 1, 25);
				break;
			case 1:
				frq = Mathf.Clamp(val, 0.5f, 25f);
				break;
			case 2:
				amp = Mathf.Clamp(val, 0.5f, 25f);
				break;
			default:
				break;
			}
		}
    }
}
