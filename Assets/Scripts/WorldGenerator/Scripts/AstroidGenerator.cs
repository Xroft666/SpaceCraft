using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Voxel2D;
using WorldGen;

public class AstroidGenerator : MonoBehaviour {

[System.Serializable]
    public class AstroidSettings
    {
        [System.Serializable]
        public class Action
        {
            public string name;
            public int index;

            public enum Method
            {
                CellularAutomata, PerlinNoise, MapEdgeCleaning, Noise
            }

            public Method method;
            public CellularAutomataStats cellularAutomataStats = new CellularAutomataStats();
            public int mapEdgeCleaning;
            public float noiseThreshold;
        }

        public string name;
        public int size = 10;
        public List<Action> actions = new List<Action>();
    }

    
    public List<int> AstroidsToGenerate = new List<int>(); 
    
    public List<AstroidSettings> AstroidList = new List<AstroidSettings>();

    public List<CellularAutomataStats>  cellularAutomataMethods = new List<CellularAutomataStats>();

	public List<int> mapEdgeCleaning = new List<int>(); 
	

	int[,] map;

    public List<int> seeds = new List<int>();

    public bool randomSeed;
	
	// Use this for initialization
	void Start () {

		StartCoroutine(Generate());
	}
	
	IEnumerator Generate()
	{
		for(int i=0;i<AstroidsToGenerate.Count;i++)
		{

		    int astroidSize = AstroidList[AstroidsToGenerate[i]].size;
			map = new int[astroidSize,astroidSize];

		    int theSeed = 0;
		    if (randomSeed)
		    {
		        theSeed = Random.Range(int.MinValue, int.MaxValue);
		    }
		    else
		    {
		        theSeed = seeds[i];
		    }

            GenerationProcedures GP = new GenerationProcedures(this, ref map, theSeed, AstroidList[AstroidsToGenerate[i]]);


		    Thread thread = new Thread(GP.Generate);
	    	thread.Start();
			while(thread.IsAlive){
				yield return new WaitForEndOfFrame();
			}


			GameObject g = new GameObject();
		    g.transform.position = Vector3.up*i*30;
			g.transform.name = "Astroid "+theSeed;
			VoxelSystem v = g.AddComponent<VoxelSystem>();

			VoxelData[,] VD = VoxelUtility.IntToVoxelDataOre(map,v);
			v.SetVoxelGrid(VD);

		}

	}

}
