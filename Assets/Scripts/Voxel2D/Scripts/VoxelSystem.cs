﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MaterialSystem;

namespace Voxel2D{
	[RequireComponent(typeof(PolygonCollider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class VoxelSystem : MonoBehaviour {
		public delegate void VoxelSystemDestroyedAction(Voxel2D.VoxelSystem voxelSystem);
		public static event VoxelSystemDestroyedAction VoxelSystemDestroyed;

		public delegate void VoxelUpdatedAction(Voxel2D.VoxelSystem voxelSystem);
		public static event VoxelUpdatedAction VoxelUpdated;

		private VoxelData[,] voxelGrid;
		
		public Vector2[] previousVelocity { get; private set;}
		public float[] previousAngularVelocity { get; private set;}
		
		public int voxelCount{ get; private set; }
		public float totalMass{get;private set;}
		
		public bool isDestroying{get;private set;}
		
		
		
		private IslandDetector.Region[] mIslands = null;
		private IslandDetector.Region[] mSeaRegions = null;
		
		private bool wasDataChanged = false;
		
		// Use this for initialization
		void Awake () 
		{
			//voxelGrid = new VoxelData[10, 10];
			previousVelocity = new Vector2[3];
			previousAngularVelocity = new float[3];
			
			rigidbody2D.angularDrag = 0;
			
			gameObject.AddComponent<VoxelImpacter>();
			gameObject.AddComponent<VoxelTextureHandler>();
		}
		
		void Update(){
			if(wasDataChanged){
				VoxelSystemUpdated(true);
				wasDataChanged = false;
			}
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
			
			previousAngularVelocity [2] = previousAngularVelocity [1];
			previousAngularVelocity [1] = previousAngularVelocity [0];
			previousAngularVelocity [0] = rigidbody2D.angularVelocity;
		}
		
		
		
		/// <summary>
		/// Gets the center.
		/// </summary>
		/// <returns>The average position of voxels.</returns>
		public Vector2 GetCenter(){	//TODO: take mass of voxel type into the calculation	
			float counter = 0;
			Vector2 sum = Vector2.zero;
			
			for (int x=0; x<voxelGrid.GetLength(0); x++) {
				for (int y=0; y<voxelGrid.GetLength(0); y++) {
					if(!IsVoxelEmpty(x,y) && VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(x,y))){
						float mass = MaterialSystem.ElementList.Instance.elements[GetVoxelID(x,y)].mass;
						sum += new Vector2(x*mass,y*mass);
						counter += mass;
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
		
		private void VoxelSystemUpdated(bool removed){
			if(voxelCount == 0){
				Debug.LogWarning("The newly created voxel system is empty, deleting");
				DestroyVoxelSystem();
			}else if(voxelCount == 1){
				Debug.Log("Turning voxel system into voxel fragment");
				IntVector2 pos = GetClosestVoxelIndex(new IntVector2(0,0),GetGridSize());
				Vector3 tPos = transform.TransformPoint(new Vector3(pos.x,pos.y,0));
				int voxelID = GetVoxel(pos.x,pos.y).GetID();
				VoxelUtility.CreateFragment(voxelID,tPos,this);
				DestroyVoxelSystem();
			}
			else{
				UpdateMass();
				SetMesh(VoxelMeshGenerator.VoxelToMesh(GetVoxelData()));
			}
		}
		
		private void DestroyVoxelSystem(){
			if(VoxelSystemDestroyed != null){
				VoxelSystemDestroyed(this);
			}
			isDestroying = true;
			Destroy(gameObject);
		}
		
		private void UpdateMass(){
			rigidbody2D.mass = totalMass;
		}
		
		private void FillVoxelGrid(VoxelData[,] grid){
			voxelGrid = grid;
			for (int x = 0; x < voxelGrid.GetLength(0); x++) {
				for (int y = 0; y < voxelGrid.GetLength(1); y++) {
					if(!IsVoxelEmpty(x,y) && VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(x,y))){
						voxelCount++;
						totalMass += MaterialSystem.ElementList.Instance.elements[GetVoxelID(x,y)].mass; //TODO: add correct mass
					}
				}
			}
			wasDataChanged = true;
		}
		
		#region set&get
		public void SetVoxelGrid(VoxelData[,] grid){	//TODO: messy code, clean!
			VoxelData[,] v = VoxelIslandDetector.SplitIslands(grid,this);
			if(v != null){
				FillVoxelGrid(v);
			}else{
				Debug.LogWarning("Requested generation of empty voxel system, deleting");
				DestroyVoxelSystem();
				
			}
			
		}
		
		public VoxelData GetVoxel(int x, int y)
		{
			if(!IsVoxelEmpty(x,y)){
				return voxelGrid [x, y];
			}else{
				return null;
			}
		}
		
		public int GetVoxelID(int x, int y)
		{
			if(!IsVoxelEmpty(x,y) && VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(x,y))){
				return voxelGrid [x, y].GetID();
			}else{
				return -1;
			}
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
		public IntVector2 GetClosestVoxelIndex(IntVector2 localPos, int radiusToCheck){
			float minDist = float.PositiveInfinity;
			IntVector2 minVoxel = new IntVector2(-1,-1);
			
			
			for(int radius = 0; radius < radiusToCheck; radius++){
				List<IntVector2> pos = new List<IntVector2>();
				for (int i = -radius; i <= radius; i++) {
					
					IntVector2 cx0 = localPos+new IntVector2(i,-radius);
					if(VoxelUtility.IsPointInBounds(voxelGrid,cx0) && !IsVoxelEmpty(cx0.x,cx0.y)){ 
						pos.Add(cx0);
					}
					
					IntVector2 cx1 = localPos+new IntVector2(i,radius);
					if(VoxelUtility.IsPointInBounds(voxelGrid,cx1) && !IsVoxelEmpty(cx1.x,cx1.y)){ 
						pos.Add(cx1);
					}
					
					IntVector2 cy0 = localPos+new IntVector2(-radius,i);
					if(VoxelUtility.IsPointInBounds(voxelGrid,cy0) && !IsVoxelEmpty(cy0.x,cy0.y)){ 
						pos.Add(cy0);
					}
					
					IntVector2 cy1 = localPos+new IntVector2(radius,i);
					if(VoxelUtility.IsPointInBounds(voxelGrid,cy1) && !IsVoxelEmpty(cy1.x,cy1.y)){ 
						pos.Add(cy1);
					}
					
				}
				for(int j=0;j<pos.Count;j++){
					float dist = ((Vector2)(pos[j]-localPos)).sqrMagnitude;
					if(dist < minDist){
						minDist = dist;
						minVoxel = pos[j];
					}
				}
				
				if(minVoxel.x != -1){
					break;
				}
			}
			
			if(minVoxel.x < 0 || minVoxel.y < 0){
				//Debug.LogError("No voxel in range of collision!");
				return minVoxel;
			}else{
				return minVoxel;
			}
		}

		public IEnumerable<IntVector2> GetClosestVoxelIndexIE(IntVector2 localPos, int radiusToCheck){

			for(int radius = 0; radius < radiusToCheck; radius++){
				for (int i = -radius; i <= radius; i++) {
					
					IntVector2 cx0 = localPos+new IntVector2(i,-radius);
					if(VoxelUtility.IsPointInBounds(voxelGrid,cx0) && !IsVoxelEmpty(cx0.x,cx0.y)){ 
						yield return cx0;
					}
					
					IntVector2 cx1 = localPos+new IntVector2(i,radius);
					if(VoxelUtility.IsPointInBounds(voxelGrid,cx1) && !IsVoxelEmpty(cx1.x,cx1.y)){ 
						yield return cx1;
					}
					
					IntVector2 cy0 = localPos+new IntVector2(-radius,i);
					if(VoxelUtility.IsPointInBounds(voxelGrid,cy0) && !IsVoxelEmpty(cy0.x,cy0.y)){ 
						yield return cy0;
					}
					
					IntVector2 cy1 = localPos+new IntVector2(radius,i);
					if(VoxelUtility.IsPointInBounds(voxelGrid,cy1) && !IsVoxelEmpty(cy1.x,cy1.y)){ 
						yield return cy1;
					}
					
				}

			}
		
		}
		
		public VoxelData GetClosestVoxel(IntVector2 localPos,int radiusToCheck){
			IntVector2 v = GetClosestVoxelIndex(localPos,radiusToCheck);
			if(v.x>=0){
				return GetVoxel(v.x,v.y);
			}else{
				return null;
			}
		}
		
		public void SetMesh(Mesh mesh)
		{
			GetComponent<MeshFilter>().sharedMesh = mesh;
			StartCoroutine(VoxelMeshGenerator.GeneratePolygonCollider(this));
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
			if (VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(x,y)) && voxelGrid [x, y] == null) {
				return true;			
			} else {
				return false;
			}
		}
		
		public VoxelData AddVoxel(int x, int y, int ID)
		{
			if(VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(x,y)) && IsVoxelEmpty(x,y)){
				voxelGrid [x, y] = new VoxelData (ID,new IntVector2(x,y));
				voxelCount++;
				totalMass += MaterialSystem.ElementList.Instance.elements[GetVoxelID(x,y)].mass; //TODO: add correct mass
				wasDataChanged = true;
				return voxelGrid[x,y];
			}else{
				Debug.LogError("Voxel allready contains data, delete voxel before adding");
				return null;
			}
		}
		
		public void RemoveVoxel(int x,int y){
			if(IsVoxelEmpty(x,y) || !VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(x,y))){
				//Debug.LogError("Voxel doesnt exist");
			}else{
				totalMass -= MaterialSystem.ElementList.Instance.elements[GetVoxelID(x,y)].mass; //TODO:use correct mass
				voxelGrid [x, y] = null;
				voxelCount--;
				wasDataChanged = true;
			}
		}
		
		
	}
}