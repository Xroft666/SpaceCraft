using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OreVeinGenerator{

	private System.Random rand;

	private int seed;

	public OreVeinGenerator(int seed = -1){

		if(seed == -1){
			System.Random r = new System.Random();
			this.seed = r.Next();
		}else{
			this.seed = seed;
		}
		rand = new System.Random((int)this.seed);
	}
	
	public int[,] GenerateOreVeins(int[,] map,float probability, int veinRadius, int oreID, int[] oreInMaterial){ //TODO: include radius
		//int[,] tmpMap = map.Clone() as int[,];

		for (int x = 0; x < map.GetLength(0); x++) {
			for (int y = 0; y < map.GetLength(1); y++) {
				bool canOre = false;

				foreach(int i in oreInMaterial){
					if(map[x,y] == i){
						canOre = true;
					}
				}
				if(canOre){
					float f = (float)rand.NextDouble();
					if(f<probability){
						map[x,y] = oreID;
					}
				}
			}
		}
	
		return map;
	}
	
}
