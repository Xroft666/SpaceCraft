using UnityEngine;
using System.Collections;

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

		private void GenerateCollider(Mesh mesh){	//TODO: generate propper collider
			Vector2[] colliderPoints;
			int pathCounter = 0;

			GetComponent<PolygonCollider2D>().pathCount = Mathf.RoundToInt(mesh.vertices.Length/4);

			for(int i=0;i<mesh.vertices.Length;i+=4){
				colliderPoints = new Vector2[4];
				for(int c=0;c<4;c++){
					colliderPoints[c] = mesh.vertices[i+c];
				}
				//colliderPoints[i] = new Vector2(mesh.vertices[i].x,mesh.vertices[i].y);
				GetComponent<PolygonCollider2D>().SetPath(pathCounter,colliderPoints);
				pathCounter++;
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
			return voxelGrid [x, y];
		}
		public int GetVoxelID(int x, int y)
		{
			return voxelGrid [x, y].GetID();
		}
		public VoxelData[,] GetVoxelData()
		{
			return (VoxelData[,])voxelGrid.Clone();
		}

		public void SetMesh(Mesh mesh)
		{
			GetComponent<MeshFilter>().sharedMesh = mesh;
			GenerateCollider(mesh);
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
