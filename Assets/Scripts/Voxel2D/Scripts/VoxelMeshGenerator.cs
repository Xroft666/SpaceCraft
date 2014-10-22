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
			List<Color> color = new List<Color>();
			
			for (int x=0; x<voxels.GetLength(0); x++) {
				for (int y=0; y<voxels.GetLength(1); y++) {
					if (voxels [x, y] != null) {
						AddQuad(ref vertices,ref triangles, ref uv,ref color, x,y,voxels[x,y]);
					}
				}
			}
			
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uv.ToArray();
			mesh.colors = color.ToArray();
			
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			//mesh.Optimize();
			
			mesh.name = "Generated Mesh";
			
			
			//			MeshHelper.Subdivide(mesh);
			//			MeshHelper.Subdivide(mesh);
			
			
			return mesh;
		}
		
		private static void AddQuad (ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uv,ref List<Color> color, int x, int y, VoxelData vox)
		{
			// index of current length to add reference vertices from
			int startIndex = vertices.Count;
			int deviceID = TextureHolder.Instance.GetDeviceIndex(vox.deviceName);
			Rect deviceRect = TextureHolder.Instance.DeviceAtlasRects[deviceID];
			Color elementColor = MaterialSystem.ElementList.Instance.elements[vox.GetElementID()].color;

			if(vox.rotation == 0) {
				vertices.Add(new Vector3(x-0.5f,y-0.5f));
				vertices.Add(new Vector3(x-0.5f,y+0.5f));
				vertices.Add(new Vector3(x+0.5f,y+0.5f));
				vertices.Add(new Vector3(x+0.5f,y-0.5f));
			}else if(vox.rotation == 90) {
				vertices.Add(new Vector3(x-0.5f,y+0.5f));
				vertices.Add(new Vector3(x+0.5f,y+0.5f));
				vertices.Add(new Vector3(x+0.5f,y-0.5f));
				vertices.Add(new Vector3(x-0.5f,y-0.5f));
			}else if(vox.rotation == 180) {
				vertices.Add(new Vector3(x+0.5f,y+0.5f));
				vertices.Add(new Vector3(x+0.5f,y-0.5f));
				vertices.Add(new Vector3(x-0.5f,y-0.5f));
				vertices.Add(new Vector3(x-0.5f,y+0.5f));
			}else if(vox.rotation == 270) {
				vertices.Add(new Vector3(x+0.5f,y-0.5f));
				vertices.Add(new Vector3(x-0.5f,y-0.5f));
				vertices.Add(new Vector3(x-0.5f,y+0.5f));
				vertices.Add(new Vector3(x+0.5f,y+0.5f));
			}else{
				Debug.LogError("voxelData has invalid rotation");
			}
			
			
			vox.vertexIndexes[0] = uv.Count;
			uv.Add(new Vector2(deviceRect.xMin,deviceRect.yMin));
			vox.vertexIndexes[1] = uv.Count;
			uv.Add(new Vector2(deviceRect.xMin,deviceRect.yMax));
			vox.vertexIndexes[2] = uv.Count;
			uv.Add(new Vector2(deviceRect.xMax,deviceRect.yMax));
			vox.vertexIndexes[3] = uv.Count;
			uv.Add(new Vector2(deviceRect.xMax,deviceRect.yMin));



			for (int i = 0; i < 4; i++) {
				color.Add(Color.black);
			}
			
			
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