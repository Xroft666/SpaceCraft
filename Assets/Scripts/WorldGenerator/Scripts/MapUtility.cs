using UnityEngine;
using System.Collections;

namespace WorldGen{
	public static class MapUtility{
		
		public static int[,] MergeMaps(int[,] original, int[,] overlay){
			int[,] map = original.Clone() as int[,];
			for (int x = 0; x < map.GetLength(0); x++) {
				for (int y = 0; y < map.GetLength(1); y++) {
					if(overlay[x,y] != 0){
						map[x,y] = overlay[x,y];
					}
				}
			}
		
			return map;
		}

		public static int[,] setMapToID(int[,] original, int ID){
			int[,] map = original.Clone() as int[,];
			for (int x = 0; x < map.GetLength(0); x++) {
				for (int y = 0; y < map.GetLength(1); y++) {
					if(map[x,y] != 0){
						map[x,y] = ID;
					}
				}
			}
			return map;
		}

        public static int[,] ClearMapEdges(int[,] map, int thickness)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (x < thickness || x > (map.GetLength(0) - thickness) || y < thickness || y > (map.GetLength(1) - thickness))
                    {
                        map[x, y] = 0;
                    }
                }
            }
            return map;
        }

	    public static Texture2D MapToBinaryTexture(int[,] map)
	    {
	        Texture2D t = new Texture2D(map.GetLength(0),map.GetLength(1));
	        for (int x = 0; x < map.GetLength(0); x++)
	        {
	            for (int y = 0; y < map.GetLength(1); y++)
	            {
	                t.SetPixel(x,y,new Color(map[x,y],map[x,y],map[x,y]));
	            }
	        }
	        return t;
	    }
	}
}
