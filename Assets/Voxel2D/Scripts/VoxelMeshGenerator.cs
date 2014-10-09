using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Voxel2D
{
	public static class VoxelMeshGenerator 
	{
		
		
		public static Mesh VoxelToMesh (VoxelData[,] voxels)
		{
			Mesh mesh = new Mesh();
			
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector2> uv = new List<Vector2>();
			
			for (int x=0; x<voxels.GetLength(0); x++) {
				for (int y=0; y<voxels.GetLength(1); y++) {
					if (voxels [x, y] != null) {
						AddQuad(ref vertices,ref triangles, ref uv, x,y);
					}
				}
			}
			
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uv.ToArray();

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.Optimize();

			mesh.name = "Generated Mesh";

			return mesh;
		}
		
		private static void AddQuad (ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uv, int x, int y)
		{
			//index of current length to add reference vertices from
			int startIndex = vertices.Count;
			
			vertices.Add(new Vector3(x-0.5f,y-0.5f));
			vertices.Add(new Vector3(x-0.5f,y+0.5f));
			vertices.Add(new Vector3(x+0.5f,y+0.5f));
			vertices.Add(new Vector3(x+0.5f,y-0.5f));
			
			uv.Add(new Vector2(0,0));
			uv.Add(new Vector2(0,1));
			uv.Add(new Vector2(1,1));
			uv.Add(new Vector2(1,0));

			//tri 1
			triangles.Add(startIndex+0);
			triangles.Add(startIndex+1);
			triangles.Add(startIndex+2);

			//tri 2
			triangles.Add(startIndex+0); 
			triangles.Add(startIndex+2); 
			triangles.Add(startIndex+3);
			
		}
		
		
	}
}