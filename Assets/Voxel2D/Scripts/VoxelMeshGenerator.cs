using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Voxel2D
{
	public static class VoxelMeshGenerator 
	{
		
		//TODO: make this multithreaded
		public static Mesh VoxelToMesh (VoxelData[,] voxels)
		{
			Mesh mesh = new Mesh();
			
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector2> uv = new List<Vector2>();
			
			for (int x=0; x<voxels.GetLength(0); x++) {
				for (int y=0; y<voxels.GetLength(1); y++) {
					if (voxels [x, y] != null) {
						AddQuad(ref vertices,ref triangles, ref uv, x,y,voxels[x,y].GetID());
					}
				}
			}
			
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uv.ToArray();

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			//mesh.Optimize();

			mesh.name = "Generated Mesh";

			return mesh;
		}
		
		private static void AddQuad (ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uv, int x, int y, int ID)
		{
			//index of current length to add reference vertices from
			int startIndex = vertices.Count;
			Rect rect = TextureHolder.Instance.TileAtlastRects[ID-1];

			vertices.Add(new Vector3(x-0.5f,y-0.5f));
			vertices.Add(new Vector3(x-0.5f,y+0.5f));
			vertices.Add(new Vector3(x+0.5f,y+0.5f));
			vertices.Add(new Vector3(x+0.5f,y-0.5f));



			uv.Add(new Vector2(rect.xMin,rect.yMin));
			uv.Add(new Vector2(rect.xMin,rect.yMax));
			uv.Add(new Vector2(rect.xMax,rect.yMax));
			uv.Add(new Vector2(rect.xMax,rect.yMin));


			//tri 1
			triangles.Add(startIndex+0);
			triangles.Add(startIndex+1);
			triangles.Add(startIndex+2);

			//tri 2
			triangles.Add(startIndex+0); 
			triangles.Add(startIndex+2); 
			triangles.Add(startIndex+3);
			
		}
		
		public static IEnumerator GeneratePolygonCollider(VoxelSystem voxel){
			PolygonColliderGenerator colGen = new PolygonColliderGenerator(voxel.GetGridSize(),VoxelUtility.VoxelDataToBool(voxel.GetVoxelData()));
			
			Thread t = new Thread(colGen.UpdateMeshCollider);
			t.Start();
			while(t.IsAlive){
				yield return new WaitForEndOfFrame();
			}
			
			PolygonCollider2D col = voxel.gameObject.GetComponent<PolygonCollider2D>();
			col.pathCount = colGen.vertexPaths.Count;
			for(int i=0;i<colGen.vertexPaths.Count;i++){
				col.SetPath(i,colGen.vertexPaths[i].ToArray());
				yield return new WaitForEndOfFrame();
			}
			voxel.rigidbody2D.WakeUp();
		}
	}
}