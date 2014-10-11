using UnityEngine;
using System.Collections;
using System.Threading;

namespace Voxel2D{
	[RequireComponent(typeof(PolygonCollider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class VoxelSystem : MonoBehaviour {
		
		private VoxelData[,] voxelGrid;
		
		public Vector2[] previousVelocity { get; private set;}
		public int voxelCount{ get; private set; }
		public float totalMass{get;private set;}
		
		// Use this for initialization
		void Awake () 
		{
			//voxelGrid = new VoxelData[10, 10];
			previousVelocity = new Vector2[3];
		}
		
		// Update is called once per frame
		void FixedUpdate () 
		{
			UpdateVelocityMemory();
		}
		
		void UpdateVelocityMemory()
		{
			previousVelocity [2] = previousVelocity [1];
			previousVelocity [1] = previousVelocity [0];
			previousVelocity [0] = rigidbody2D.velocity;
		}
		
		
		
		/// <summary>
		/// Gets the center.
		/// </summary>
		/// <returns>The average position of voxels.</returns>
		private Vector2 GetCenter(){	//TODO: take mass of voxel type into the calculation	
			int counter = 0;
			Vector2 sum = Vector2.zero;
			
			for (int x=0; x<voxelGrid.GetLength(0); x++) {
				for (int y=0; y<voxelGrid.GetLength(0); y++) {
					if(!IsVoxelEmpty(x,y)){
						sum += new Vector2(x,y);
						counter++;
					}
				}
			}
			if(counter>0){
				return sum/counter;
			}else{
				Debug.LogWarning("Trying to get center of empty voxel system");
				return Vector2.zero;
			}
			
		}
		
		private void UpdateMass(){
			rigidbody2D.mass = totalMass;
		}
		
		private IEnumerator GenerateCollider(Mesh mesh){	//TODO: generate propper collider
			PolygonColliderGenerator colGen = new PolygonColliderGenerator(voxelGrid.GetLength(0),VoxelUtility.VoxelDataToBool(GetVoxelData()));
			PolygonCollider2D col = GetComponent<PolygonCollider2D>();
			//colGen.UpdateMeshCollider();
			Thread t = new Thread(colGen.UpdateMeshCollider);
			t.Start();
			while(t.IsAlive){
				yield return new WaitForEndOfFrame();
			}

			col.pathCount = colGen.vertexPaths.Count;
			for(int i=0;i<col.pathCount;i++){
				col.SetPath(i,colGen.vertexPaths[i].ToArray());
				yield return new WaitForEndOfFrame();
			}
		}
		
		#region set&get
		public void SetVoxelGrid(VoxelData[,] grid){
			voxelGrid = grid;
			for (int x = 0; x < voxelGrid.GetLength(0); x++) {
				for (int y = 0; y < voxelGrid.GetLength(1); y++) {
					if(voxelGrid[x,y].GetID() != null){
						voxelCount++;
						totalMass += 1; //TODO: add correct mass
					}
				}
			}
			UpdateMass();
		}
		
		public void SetVoxel(int x, int y, int ID)
		{
			voxelGrid [x, y] = new VoxelData (ID);
			voxelCount++;
			totalMass += 1; //TODO: add correct mass
			UpdateMass();
		}
		public VoxelData GetVoxel(int x, int y)
		{
			if(voxelGrid [x, y] != null){
				return voxelGrid [x, y];
			}else{
				return null;
			}
		}

		public int GetVoxelID(int x, int y)
		{
			return voxelGrid [x, y].GetID();
		}
		public VoxelData[,] GetVoxelData()
		{
			return (VoxelData[,])voxelGrid.Clone();
		}

		/// <summary>
		/// Gets the closest voxel.
		/// </summary>
		/// <returns>The closest voxel.</returns>
		/// <param name="localPos">Local position.</param>
		public int[] GetClosestVoxelIndex(Vector2 localPos){
			float minDist = float.PositiveInfinity;
			Vector2 minVoxel = new Vector2(-1,-1);
			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					Vector2 c = localPos+new Vector2(x,y);
					c = new Vector2(Mathf.RoundToInt(c.x),Mathf.RoundToInt(c.y));
					if(GetVoxel((int)c.x,(int)c.y) != null){
						float dist = (c-localPos).sqrMagnitude;
						if(dist < minDist){
							minDist = dist;
							minVoxel = c;
						}
					}
				}
			}
			if(minVoxel.x < 0 || minVoxel.y < 0){
				Debug.LogError("No voxel in range of collision!");
			}
			int[] i = new int[2];
			i[0] = (int)minVoxel.x;
			i[1] = (int)minVoxel.y;
			return i;
		}
		public void SetMesh(Mesh mesh)
		{
			GetComponent<MeshFilter>().sharedMesh = mesh;
			StartCoroutine(GenerateCollider(mesh));
			rigidbody2D.centerOfMass = GetCenter();
			
		}
		
		public void SetGridSize(int size)
		{
			voxelGrid = new VoxelData[size,size];
		}
		public int GetGridSize()
		{
			return voxelGrid.GetLength(0);
		}
		#endregion set&get
		
		public bool IsVoxelEmpty(int x,int y)
		{
			if (voxelGrid [x, y] == null) {
				return false;			
			} else {
				return true;
			}
		}
		
		public void RemoveVoxel(int x,int y){
			totalMass -= 1; //TODO:use correct mass
			UpdateMass();
			voxelGrid [x, y] = null;
			voxelCount--;
		}
		
		
		
	}
}
