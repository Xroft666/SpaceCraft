using UnityEngine;
using System.Collections;


namespace WorldGen{
	public class CellularAutomata{
		
		private int[,] map;
		
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
		public CellularAutomata (int mapWidth, int mapHeight) {
			map = new int[mapWidth, mapHeight];
			seed = Random.Range(int.MaxValue,int.MinValue);

			//caveConfig = new CaveConfig();

			generateNoise(this.seed);
		}
		
		public CellularAutomata (int mapWidth, int mapHeight, int seed) {
			map = new int[mapWidth, mapHeight];
			this.seed = seed;

			//caveConfig = new CaveConfig();

			generateNoise(this.seed);
		}
		
		
		
		private void generateNoise(float seed)
		{
			Random.seed = (int)seed;
			
			Debug.Log ("generating noise");
			
			for (int x=0; x<map.GetLength(0); x++) {
				for(int y=0;y<map.GetLength(1);y++){
					int r = Random.Range(0,2);
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
			int[,] map = this.map.Clone () as int[,];
			//int numTilesRadius = Mathf.RoundToInt(Mathf.Pow (squareRules.radius * 2 + 1, 2))-1;
			caveConfig.squareRules.bt = Mathf.RoundToInt (caveConfig.squareRules.blackChangeThreshold * (Mathf.Pow(caveConfig.squareRules.radius * 2 + 1,2)));
			caveConfig.squareRules.wt = Mathf.RoundToInt (caveConfig.squareRules.whileChangeThreshold * (Mathf.Pow(caveConfig.squareRules.radius * 2 + 1,2)));
			
			
			for (int x=0; x<map.GetLength(0); x++) {
				for(int y=0;y<map.GetLength(1);y++){
					
					int blockCount = 0;
					float counter2 = 0;
					int[] counter = new int[2];
					int current = map[x,y];
					
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
							map[x,y] = 1;
						}else if(current== 1 && counter[0] >= caveConfig.adjecentRules.whileChangeThreshold){
							map[x,y] = 0;
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
						
						counter2 = counter2/blockCount;
						
						
						if(current == 0 && counter[1] >= caveConfig.squareRules.bt){
							map[x,y] = 1;
						}else if(current== 1 && counter[0] >= caveConfig.squareRules.wt){
							map[x,y] = 0;
						}
						
					}
					
					
				}	
			}
			
			this.map = map;
			
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
			Debug.Log(caveConfig.squareRules.whileChangeThreshold);
			GenerateDigital();
		}
		
		
	}
}
