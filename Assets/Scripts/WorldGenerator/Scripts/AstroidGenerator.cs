using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Voxel2D;
using WorldGen;

[System.Serializable]
public class AstroidGenerator : MonoBehaviour {

[System.Serializable]
    public class AstroidSettings
    {
        [System.Serializable]
        public class Action
        {
            public string name;
            public int index;

            [System.Serializable]
            public enum Method
            {
                CellularAutomata, PerlinNoise, MapEdgeCleaning, Noise, Invert
            }

            [System.Serializable]
            public class WindowEditor
            {
                public Vector2 windowPos;
                public List<int> inList = new List<int>();
                public List<int> outList = new List<int>();
            }

            public WindowEditor windowEditor = new WindowEditor();
            public Method method;
            public CellularAutomataStats cellularAutomataStats = new CellularAutomataStats();
            public PerlinNoiseStats perlinNoiseStats = new PerlinNoiseStats();
            public Voxel2D.IntVector2 invertStats;
            public int mapEdgeCleaning;
            public float noiseThreshold;

            public void Randomize(int mapSize)
            {
                cellularAutomataStats.Ranomize(mapSize);
                perlinNoiseStats.Randomize(mapSize);
                mapEdgeCleaning = Random.Range(1, mapSize/2);
                noiseThreshold = Random.Range(0f, 1f);
            }
        }

        public string name;

        public int size = 10;
        public List<Action> actions = new List<Action>();

    
    }

    
    public List<int> AstroidsToGenerate = new List<int>(); 
    
    public List<AstroidSettings> AstroidList = new List<AstroidSettings>();

	int[,] map;

    public List<int> seeds = new List<int>();

    public bool randomSeed;
    public bool ToVoxel = true;
	
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

		    if (ToVoxel)
		    {
		        MapToVoxel(i, theSeed);
		    }
		}

        

	}

    void MapToVoxel(int i,int theSeed)
    {
        GameObject g = new GameObject();
        g.transform.position = Vector3.up * i * 30;
        g.transform.name = "Astroid " + theSeed;
        VoxelSystem v = g.AddComponent<VoxelSystem>();

        VoxelData[,] VD = VoxelUtility.IntToVoxelDataOre(map, v);
        v.SetVoxelGrid(VD);
    }

}
