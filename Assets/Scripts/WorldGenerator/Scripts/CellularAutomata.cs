using UnityEngine;
using System.Collections;

namespace WorldGen{
	public class CellularAutomata{
		
		private int[,] map;
		private System.Random rand;
		
		public int seed;
		
		
		
		public enum NeighborType{Adjecent,Square,Circle};
		public NeighborType neighborType;
		
		private int iterationCounter = 0;
		
		#region classType
		
		
		
		[System.Serializable]
		public class CaveConfig{
			[System.Serializable]
			public class AdjecentRules{
				public int whileChangeThreshold;
				public int blackChangeThreshold;
			}
			
			[System.Serializable]
			public class SquareRules{
				public int radius;
				
				public float whileChangeThreshold;
				public float blackChangeThreshold;
				
				[HideInInspector]
				public int wt;
				[HideInInspector]
				public int bt;
			}
			
			[System.Serializable]
			public class CircleRules{
				public int radius;
				public int whileChangeThreshold;
				public int blackChangeThreshold;
			}
			
			public AdjecentRules adjecentRules = new AdjecentRules();
			public SquareRules squareRules = new SquareRules();
			public CircleRules circleRules = new CircleRules();
		}
		#endregion classType
		
		public CaveConfig caveConfig = new CaveConfig();
		
		// Use this for initialization
		public CellularAutomata (ref int[,] map, int seed = -1) {
			this.map = map;

			if(seed == -1){
				System.Random rand = new System.Random();
				this.seed = rand.Next();
			}else{
				this.seed = seed;
			}

			
			generateNoise(this.seed);
		}
		

		
		
		
		private void generateNoise(float seed)
		{
			rand = new System.Random((int)seed);
			
			for (int x=0; x<map.GetLength(0); x++) {
				for(int y=0;y<map.GetLength(1);y++){
					int r = Mathf.RoundToInt((float)rand.NextDouble());
					map[x,y] = r;
				}
			}
		}
		
		public void ClearMapEdges(int thickness)
		{
			for (int x= 0; x < map.GetLength(0); x++) {
				for (int y = 0; y < map.GetLength(1); y++) {
					if(x < thickness || x > (map.GetLength(0)-thickness) || y < thickness || y > (map.GetLength(1)-thickness)){
						map[x,y] = 0;
					}
				}
			}
		}
		
		public int[,] GetMap(){
			return map;
		}
		
		void GenerateDigital(){
			int[,] mapClone = this.map.Clone () as int[,];
			//int numTilesRadius = Mathf.RoundToInt(Mathf.Pow (squareRules.radius * 2 + 1, 2))-1;
			caveConfig.squareRules.bt = Mathf.RoundToInt (caveConfig.squareRules.blackChangeThreshold * (Mathf.Pow(caveConfig.squareRules.radius * 2 + 1,2)));
			caveConfig.squareRules.wt = Mathf.RoundToInt (caveConfig.squareRules.whileChangeThreshold * (Mathf.Pow(caveConfig.squareRules.radius * 2 + 1,2)));
			
			
			for (int x=0; x<mapClone.GetLength(0); x++) {
				for(int y=0;y<mapClone.GetLength(1);y++){
					
					int blockCount = 0;
					int[] counter = new int[2];
					int current = mapClone[x,y];
					
					if (neighborType == NeighborType.Adjecent) {
						
						if(checkBounds(x,y,1,0)){
							counter[this.map[x+1,y]]++;
						}
						if(checkBounds(x,y,-1,0)){
							counter[this.map[x-1,y]]++;
						}
						if(checkBounds(x,y,0,1)){
							counter[this.map[x,y+1]]++;
						}
						if(checkBounds(x,y,0,-1)){
							counter[this.map[x,y-1]]++;
						}
						
						if(current == 0 && counter[1] >= caveConfig.adjecentRules.blackChangeThreshold){
							mapClone[x,y] = 1;
						}else if(current== 1 && counter[0] >= caveConfig.adjecentRules.whileChangeThreshold){
							mapClone[x,y] = 0;
						}
						
					}else if(neighborType == NeighborType.Square){
						int radius = caveConfig.squareRules.radius;
						for (int nx=-radius; nx<=radius; nx++) {
							for(int ny=-radius;ny<=radius;ny++){
								if(!(nx == 0 && ny == 0) && checkBounds(x,y,nx,ny)){
									counter[this.map[x+nx,y+ny]]++;
								}
							}
						}
						
						
						
						if(current == 0 && counter[1] >= caveConfig.squareRules.bt){
							mapClone[x,y] = 1;
						}else if(current== 1 && counter[0] >= caveConfig.squareRules.wt){
							mapClone[x,y] = 0;
						}
						
					}
					
					
				}	
			}
			
			for (int x = 0; x < map.GetLength(0); x++) {
				for (int y = 0; y < map.GetLength(1); y++) {
					this.map[x,y] = mapClone[x,y];
				}
			}
			
			//this.map = map;
			
		}
		
		bool checkBounds(int x,int y, int nx, int ny){
			if (x + nx >= 0 && x + nx < map.GetLength(0) &&	y + ny >= 0 && y + ny < map.GetLength(1)) {
				return true;
			} else {
				return false;
			}
		}
		
		
		
		public void nextIteration(){
			iterationCounter++;
			GenerateDigital();
		}
		
		
	}
}
