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
            map = NoiseGenerator.GenerateNoise(seed, map,0.5f);
            //map = MapUtility.ClearMapEdges(map, 2);

	        foreach (AstroidGenerator.AstroidSettings.Action action in astroid.actions)
	        {
	           GenerateAction(ref map, action);
	        }
	    }

        public void GenerateAction(ref int[,] map, AstroidGenerator.AstroidSettings.Action action)
	    {
            if (action.method == AstroidGenerator.AstroidSettings.Action.Method.CellularAutomata)
            {
                CellularAutomata CA = new CellularAutomata(ref map);
                CellularAutomata.CaveConfig.SquareRules rules = CA.caveConfig.squareRules;
                CellularAutomataStats stats = action.cellularAutomataStats;

                //TODO: allow for other method
                CA.neighborType = CellularAutomata.NeighborType.Square;

                rules.blackChangeThreshold = stats.BlackChangeThreshold;
                rules.whileChangeThreshold = stats.WhileChangeThreshold;
                rules.radius =stats.Radius;
                int rounds = stats.Rounds;
                for (int i = 0; i < rounds; i++)
                {
                    CA.nextIteration();
                }
            }
            else if (action.method == AstroidGenerator.AstroidSettings.Action.Method.MapEdgeCleaning)
            {
                MapUtility.ClearMapEdges(map, action.mapEdgeCleaning);
            }
            else if (action.method == AstroidGenerator.AstroidSettings.Action.Method.Noise)
            {
                map = NoiseGenerator.GenerateNoise(seed, map, action.noiseThreshold);
            }
            else if (action.method == AstroidGenerator.AstroidSettings.Action.Method.PerlinNoise)
            {
                PerlinGen(ref map, action);
            }
            else if (action.method == AstroidGenerator.AstroidSettings.Action.Method.Invert)
            {
                map = MapUtility.invertMap(map, action.invertStats.x, action.invertStats.y);
            }
            
            //TODO:implement others
	    }

	    public void PerlinGen(ref int[,] map, AstroidGenerator.AstroidSettings.Action action){

			PerlinNoise noise = new PerlinNoise(seed);
	        PerlinNoiseStats PS = action.perlinNoiseStats;
	
			int[,] mapClone = this.map.Clone () as int[,];
		
		

			for (int x=0; x<mapClone.GetLength(0); x++) 
			{
				for (int y=0; y<mapClone.GetLength(1); y++)
				{
					int voxel = (int)noise.FractalNoise2D((float)x, (float)y, PS.octNum, PS.frq, PS.amp);

					if (voxel <= 0)
						mapClone[x,y]=0;
					else
						mapClone[x,y] = 1;
				}
			}
			
			for (int x = 0; x < map.GetLength(0); x++) {
				for (int y = 0; y < map.GetLength(1); y++) {
					this.map [x, y] = mapClone [x, y];
				}
			}

			map = mapClone;
		}
		
	}
}
