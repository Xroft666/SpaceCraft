using UnityEngine;
using System.Collections;


namespace WorldGen{
	public class GenerationProcedures{

		int[,] map;
		//System.Random randomSeed;

		//RandomSeedGen se = new RandomSeedGen();
		//public int mySeed = se.DoIt();
		//int mySeed = Random.Range(0, 900000);

	    private int seed;
	    private AstroidGenerator.AstroidSettings astroid;
	    private AstroidGenerator AS;

		public GenerationProcedures(AstroidGenerator AS, ref int[,] map, int seed, AstroidGenerator.AstroidSettings astroid)
		{
		    this.seed = seed;
            this.map = map;
		    this.astroid = astroid;
		    this.AS = AS;
		}

		

	    public void Generate()
	    {
            map = NoiseGenerator.GenerateNoise(seed, map);
            //map = MapUtility.ClearMapEdges(map, 2);

	        foreach (AstroidGenerator.AstroidSettings.Actions action in astroid.actions)
	        {
	            if (action.method == AstroidGenerator.AstroidSettings.Actions.Method.CellularAutomata)
	            {
	                CellularAutomata CA = new CellularAutomata(ref map);
	                CellularAutomata.CaveConfig.SquareRules rules = CA.caveConfig.squareRules;
	                    //TODO: allow for other method
	                CA.neighborType = CellularAutomata.NeighborType.Square;

	                rules.blackChangeThreshold = AS.cellularAutomataMethods[action.index].BlackChangeThreshold;
	                rules.whileChangeThreshold = AS.cellularAutomataMethods[action.index].WhileChangeThreshold;
	                rules.radius = AS.cellularAutomataMethods[action.index].Radius;
	                int rounds = AS.cellularAutomataMethods[action.index].Rounds;
                    for (int i = 0; i < rounds; i++)
                    {
                        CA.nextIteration();
                    }
	            }
	            else if(action.method == AstroidGenerator.AstroidSettings.Actions.Method.MapEdgeCleaning)
	            {
	                MapUtility.ClearMapEdges(map, AS.mapEdgeCleaning[action.index]);
	            }//TODO:implement others
	        }
	    }
		
		public void GenAstroidType1()
		{

		    map = NoiseGenerator.GenerateNoise(seed, map);
            map = MapUtility.ClearMapEdges(map, 2);

            CellularAutomata CA = new CellularAutomata(ref map);
			CellularAutomata.CaveConfig.SquareRules rules = CA.caveConfig.squareRules;
		    CA.neighborType = CellularAutomata.NeighborType.Square;

			int rounds = 0;

			rules.blackChangeThreshold = 0.55f;
			rules.whileChangeThreshold = 0.55f;
			rules.radius = 2;
			rounds = 10;
			//RunIteration (CA,rounds);

			rules.blackChangeThreshold = 0.6f;
			rules.whileChangeThreshold = 0.6f;
			rules.radius = 1;
			rounds = 2;
			//RunIteration (CA,rounds);

			map = CA.GetMap();

			//map = MapUtility.swapMapIDs(map,0);

			OreVeinGenerator v = new OreVeinGenerator();

			//int[,] tmpMap =v.GenerateOreVeins(map,0.5f,1,3,new int[]{1});
			map = v.GenerateOreVeins(map,0.2f,1,2,new int[]{1});
			map = v.GenerateOreVeins(map,0.1f,1,3,new int[]{1});
		}

		public void PerlinGen(){

			
			//RandomSeedGen generator = new RandomSeedGen();
			//System.Random mySeed = new System.Random(987);
//			PerlinNoise noise = new PerlinNoise((int)mySeed.NextDouble());
			PerlinNoise noise = new PerlinNoise(seed);

	
			int[,] mapClone = this.map.Clone () as int[,];
		
		

			for (int x=0; x<mapClone.GetLength(0); x++) 
			{
				for (int y=0; y<mapClone.GetLength(1); y++)
				{
					int voxel = (int)noise.FractalNoise2D((float)x, (float)y, 6, 10f, 22f);

					if (voxel <= 0)
						mapClone[x,y]=0;
					else
						mapClone[x,y] = 1;

					//mapClone[x,y] = (int)noise.FractalNoise2D((float)x, (float)y, 6, 10f, 22f);
				//	Debug.Log("x= "+x+ "y= " + y);
					//Debug.Log (noise.FractalNoise2D((float)x, (float)y, 6, 10f, 22f));
					Debug.Log (mapClone[x,y]);
				}
			}
			
					

		

			for (int x = 0; x < map.GetLength(0); x++) {
								for (int y = 0; y < map.GetLength(1); y++) {
										this.map [x, y] = mapClone [x, y];
								}
						}
			
			Debug.Log (mapClone.Length);
			map = mapClone;
		}
		/*
		void method2(){
			CellularAutomata CA = new CellularAutomata(voxelSize,voxelSize);
			CellularAutomata.CaveConfig.SquareRules r = CA.caveConfig.squareRules;
			
			CA.neighborType = CellularAutomata.NeighborType.Square;
			
			r.blackChangeThreshold = 0.6f;
			r.whileChangeThreshold = 0.6f;
			r.radius = 1;
			RunIteration (CA,25);
			
		}
		
		void method3(){
			CellularAutomata CA = new CellularAutomata(voxelSize,voxelSize);
			CellularAutomata.CaveConfig.SquareRules r = CA.caveConfig.squareRules;
			
			CA.neighborType = CellularAutomata.NeighborType.Square;
			
			
			r.blackChangeThreshold = 0.7f;
			r.whileChangeThreshold = 0.7f;
			r.radius = 3;
			RunIteration (CA,3);
			
			r.blackChangeThreshold = 0.6f;
			r.whileChangeThreshold = 0.53f;
			r.radius = 4;
			RunIteration (CA,2);
			
			r.blackChangeThreshold = 0.6f;
			r.whileChangeThreshold = 0.6f;
			r.radius = 1;
			RunIteration (CA,10);
		}
		*/

		
	}
}
