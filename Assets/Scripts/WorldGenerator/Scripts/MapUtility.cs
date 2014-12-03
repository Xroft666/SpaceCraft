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

		public static int[,] setMapToID(int[,] original, int fromID, int toID){
			int[,] map = original.Clone() as int[,];
			for (int x = 0; x < map.GetLength(0); x++) {
				for (int y = 0; y < map.GetLength(1); y++) {
					if(map[x,y] == fromID){
						map[x,y] = toID;
					}
				}
			}
			return map;
		}

	    public static int[,] invertMap(int[,] original, int ID1, int ID2)
	    {
            int[,] map = original.Clone() as int[,];
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] == ID1)
                    {
                        map[x, y] = ID2;
                    }
                    else if (map[x, y] == ID2)
                    {
                        map[x, y] = ID1;
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
                    if (x < thickness || x > (map.GetLength(0) - thickness-1) || y < thickness || y > (map.GetLength(1) - thickness-1))
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

		public static Texture2D MapToBinaryTexture(float[,] map)
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
