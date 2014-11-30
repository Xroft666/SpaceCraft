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
        public class Actions
        {
            public string name;
            public int index;

            public enum Method
            {
                CellularAutomata, PerlinNoise, MapEdgeCleaning
            }

            public Method method;
        }

        public string name;
        public int size;
        public List<Actions> actions = new List<Actions>();
    }

    
    public List<int> AstroidsToGenerate = new List<int>(); 
    
    public List<AstroidSettings> AstroidList = new List<AstroidSettings>();

    public List<CellularAutomataStats>  cellularAutomataMethods = new List<CellularAutomataStats>();

	public List<int> mapEdgeCleaning = new List<int>(); 
	

	int[,] map;

    public List<int> seeds = new List<int>();

    public bool randomSeed;

	public static List<VoxelSystem> generatedAsteroidList = new List<VoxelSystem>();
	
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
			g.transform.position = transform.position;
			g.transform.name = "Astroid "+theSeed;

			g.layer = 9; // obstacle layer

			VoxelSystem v = g.AddComponent<VoxelSystem>();

			VoxelData[,] VD = VoxelUtility.IntToVoxelDataOre(map,v);
			v.SetVoxelGrid(VD);

			g.rigidbody2D.isKinematic = true;
			//HACK: just for tests
			//g.rigidbody2D.angularVelocity = 100;

			generatedAsteroidList.Add(v);
		}

	}



	// Update is called once per frame
	void Update () {
		
	}
}
