using UnityEngine;
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
		#region events
		public delegate void VoxelSystemDestroyedAction(Voxel2D.VoxelSystem voxelSystem);
		public static event VoxelSystemDestroyedAction VoxelSystemDestroyed;
		
		public delegate void VoxelSystemUpdatedAction(Voxel2D.VoxelSystem voxelSystem);
		public event VoxelSystemUpdatedAction VoxelSystemUpdated;
		
		public delegate void VoxelAddedAction(VoxelData VD);
		public event VoxelAddedAction VoxelAdded;
		
		public delegate void VoxelRemovedAction(VoxelData VD);
		public event VoxelRemovedAction VoxelRemoved;
		
		#endregion events
		
		public VoxelData[,] voxelGrid;
		
		public Vector2[] previousVelocity { get; private set;}
		public float[] previousAngularVelocity { get; private set;}
		
		public int voxelCount{ get; private set; }
		public float totalMass{get;private set;}
		
		public bool isDestroying{get;private set;}
		
		
		
		private IslandDetector.Region[] mIslands = null;
		private IslandDetector.Region[] mSeaRegions = null;
		
		private bool wasDataChanged = false;

		private Vector2 cachedCenter;
		
		
		// Use this for initialization
		void Awake () 
		{
			voxelGrid = new VoxelData[0, 0];
			previousVelocity = new Vector2[3];
			previousAngularVelocity = new float[3];
			
			GetComponent<Rigidbody2D>().angularDrag = 0;
			
			gameObject.AddComponent<VoxelImpacter>();
			gameObject.AddComponent<VoxelTextureHandler>();
			gameObject.AddComponent<PropertyOverlay>();

			MeshFilter mFilter = GetComponent<MeshFilter>();
			Mesh mesh = new Mesh();
			mesh.MarkDynamic();
			mFilter.sharedMesh = mesh;

			RefreshCenter();
		}
		
		void Update(){
			if(wasDataChanged){
				VoxelSystemWasUpdated(true);
				wasDataChanged = false;
			}
			
			foreach(VoxelData v in voxelGrid){
				if(v != null){
					v.OnUpdate();
				}
			}
		}
		
		// Update is called once per physics tick
		void FixedUpdate () 
		{
			UpdateVelocityMemory();
			foreach(VoxelData v in voxelGrid){
				if(v != null){
					v.OnFixedUpdate();
				}
			}
		}
		
		void UpdateVelocityMemory()
		{
			previousVelocity [2] = previousVelocity [1];
			previousVelocity [1] = previousVelocity [0];
			previousVelocity [0] = GetComponent<Rigidbody2D>().velocity;
			
			previousAngularVelocity [2] = previousAngularVelocity [1];
			previousAngularVelocity [1] = previousAngularVelocity [0];
			previousAngularVelocity [0] = GetComponent<Rigidbody2D>().angularVelocity;
		}
		
		
		
		/// <summary>
		/// Gets the center.
		/// </summary>
		/// <returns>The average position of voxels.</returns>
		public Vector2 GetCenter(){	

			return cachedCenter;
		}

		private void RefreshCenter(){

			float counter = 0;
			Vector2 sum = Vector2.zero;
			
			for (int x=0; x<voxelGrid.GetLength(0); x++) {
				for (int y=0; y<voxelGrid.GetLength(0); y++) {
					if(!IsVoxelEmpty(x,y) && VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(x,y))){
						float mass = GetVoxel(x,y).stats.mass;
						sum += new Vector2(x*mass,y*mass);
						counter += mass;
					}
				}
			}
			if(counter>0){
				cachedCenter = sum/counter;
			}else{
				//Debug.LogWarning("Trying to get center of empty voxel system");
				cachedCenter = Vector2.zero;
			}
		}
		
		private void VoxelSystemWasUpdated(bool removed){

			if(VoxelSystemUpdated != null){
				VoxelSystemUpdated(this);
			}

			if(voxelCount == 0){
				Debug.LogWarning("The newly created voxel system is empty, deleting");
				DestroyVoxelSystem();
			}else if(voxelCount == -1){ //BUG hacky disabled fragments
				Debug.Log("Turning voxel system into voxel fragment");
				IntVector2 pos = GetClosestVoxelIndex(new IntVector2(0,0),GetGridSize());
				Vector3 tPos = transform.TransformPoint(new Vector3(pos.x,pos.y,0));
				VoxelData vox = GetVoxel(pos.x,pos.y);
				VoxelUtility.CreateFragment(vox,tPos,this);
				DestroyVoxelSystem();
			}
			else{
				RefreshCenter();
				UpdateMass();

				if(voxelCount >0){

					MeshFilter mFilter = GetComponent<MeshFilter>();
					Mesh mesh = mFilter.sharedMesh;

					VoxelMeshGenerator.VoxelToMesh(GetVoxelData(), ref mesh);
					SetMesh(ref mesh);
				}
			}
		}

	    public void ForceUpdate()
	    {
	        VoxelSystemWasUpdated(true);
            RefreshCenter();
	    }

		void OnDestroy()
		{
			MeshFilter mFilter = GetComponent<MeshFilter>();
			Mesh mesh = mFilter.sharedMesh;
			Destroy(mesh);
		}
		
		private void DestroyVoxelSystem(){
			if(VoxelSystemDestroyed != null){
				VoxelSystemDestroyed(this);
			}
			isDestroying = true;

			MeshFilter mFilter = GetComponent<MeshFilter>();
			Destroy(mFilter.sharedMesh);

//			mFilter.sharedMesh = null;

			Destroy(gameObject);
		}
		
		private void UpdateMass(){
			GetComponent<Rigidbody2D>().mass = totalMass;
		}
		
		private void FillVoxelGrid(VoxelData[,] grid){
			voxelGrid = grid;
			for (int x = 0; x < voxelGrid.GetLength(0); x++) {
				for (int y = 0; y < voxelGrid.GetLength(1); y++) {
					if(!IsVoxelEmpty(x,y) && VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(x,y))){
						voxelCount++;
						totalMass += GetVoxel(x,y).stats.mass; 
						GetVoxel(x,y).OnNeighbourChange();
					}
				}
			}



			wasDataChanged = true;
		}
		
		#region set&get
		public void SetVoxelGrid(VoxelData[,] grid){	//TODO: messy code, clean!
		    SetEmpty();

            VoxelData[,] v = VoxelIslandDetector.SplitAndReturnFirstIslands(grid,this);
			if(v != null){
				FillVoxelGrid(v);
			}else{
				Debug.LogWarning("Requested generation of empty voxel system, deleting");
				DestroyVoxelSystem();
				
			}
			
		}

	    public void SetEmpty()
	    {
            for (int x = 0; x < voxelGrid.GetLength(0); x++)
            {
                for (int y = 0; y < voxelGrid.GetLength(1); y++)
                {
                    if (voxelGrid[x, y] != null)
                    {
                        voxelGrid[x, y].OnDelete();
                        voxelGrid[x, y] = null;
                    }
                }
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
				return voxelGrid [x, y].GetElementID();
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
			if(VoxelUtility.IsPointInBounds(voxelGrid, localPos) && !IsVoxelEmpty(localPos.x,localPos.y)){
				yield return localPos;
			}
			for(int radius = 1; radius < radiusToCheck; radius++){
				for (int i = 0; i <= radius; i++) {
					if(i!=0 && i!= radius){
						IntVector2 cx0 = localPos+new IntVector2(-i,radius);
						if(VoxelUtility.IsPointInBounds(voxelGrid,cx0) && !IsVoxelEmpty(cx0.x,cx0.y)){ 
							yield return cx0;
						}
						
						IntVector2 cx1 = localPos+new IntVector2(radius,i);
						if(VoxelUtility.IsPointInBounds(voxelGrid,cx1) && !IsVoxelEmpty(cx1.x,cx1.y)){ 
							yield return cx1;
						}
						
						IntVector2 cy0 = localPos+new IntVector2(i,-radius);
						if(VoxelUtility.IsPointInBounds(voxelGrid,cy0) && !IsVoxelEmpty(cy0.x,cy0.y)){ 
							yield return cy0;
						}
						
						IntVector2 cy1 = localPos+new IntVector2(-radius,-i);
						if(VoxelUtility.IsPointInBounds(voxelGrid,cy1) && !IsVoxelEmpty(cy1.x,cy1.y)){ 
							yield return cy1;
						}
					}		
					{
						IntVector2 cx0 = localPos+new IntVector2(i,radius);
						if(VoxelUtility.IsPointInBounds(voxelGrid,cx0) && !IsVoxelEmpty(cx0.x,cx0.y)){ 
							yield return cx0;
						}
						
						IntVector2 cx1 = localPos+new IntVector2(radius,-i);
						if(VoxelUtility.IsPointInBounds(voxelGrid,cx1) && !IsVoxelEmpty(cx1.x,cx1.y)){ 
							yield return cx1;
						}
						
						IntVector2 cy0 = localPos+new IntVector2(-i,-radius);
						if(VoxelUtility.IsPointInBounds(voxelGrid,cy0) && !IsVoxelEmpty(cy0.x,cy0.y)){ 
							yield return cy0;
						}
						
						IntVector2 cy1 = localPos+new IntVector2(-radius,i);
						if(VoxelUtility.IsPointInBounds(voxelGrid,cy1) && !IsVoxelEmpty(cy1.x,cy1.y)){ 
							yield return cy1;
						}
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
		
		public void SetMesh(ref Mesh mesh)
		{
			StartCoroutine(VoxelMeshGenerator.GeneratePolygonCollider(this));
			GetComponent<Rigidbody2D>().centerOfMass = GetCenter();
		}
		
		public void SetGridSize(int size)
		{
			voxelGrid = new VoxelData[size,size];
		}
		public int GetGridSize()
		{
			if(voxelGrid != null){
				return voxelGrid.GetLength(0);
			}else{ 
				return 0;
			}
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

		public bool CanAddVoxel(IntVector2 pos){
			if(VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(pos.x,pos.y)) && IsVoxelEmpty(pos.x,pos.y))
			{
				return true;
			}
			return false;
		}

		public VoxelData AddVoxel( VoxelData voxel )
		{
		    if (voxel != null)
		    {
		        IntVector2 pos = voxel.GetPosition();

		        if (VoxelUtility.IsPointInBounds(GetVoxelData(), new Vector2(pos.x, pos.y)) && IsVoxelEmpty(pos.x, pos.y))
		        {
		            voxelGrid[pos.x, pos.y] = voxel;
		            voxelCount++;
		            totalMass += voxel.stats.mass; //TODO: add correct mass
		            wasDataChanged = true;
		            if (VoxelAdded != null)
		            {
		                VoxelAdded(voxel);
		            }
		            NeighbourUpdate(voxel);
		            return voxelGrid[pos.x, pos.y];
		        }
		        else
		        {
		            Debug.LogError("Voxel allready contains data, delete voxel before adding");
		            return null;
		        }
		    }
		    else
		    {
		        //Debug.LogWarning("Voxel System Doesnt Exist");
		        return null;
		    }
		}

		public void RemoveVoxel(int x,int y){
			if(IsVoxelEmpty(x,y) || !VoxelUtility.IsPointInBounds(GetVoxelData(),new Vector2(x,y))){
				//Debug.LogError("Voxel doesnt exist");
			}else{
				totalMass -= GetVoxel(x,y).stats.mass; //TODO:use correct mass
				if(VoxelRemoved!=null){
					VoxelRemoved(voxelGrid [x, y]);
				}
				NeighbourUpdate(voxelGrid [x, y]);
				voxelGrid [x, y].OnDelete();
				voxelGrid [x, y] = null;
				voxelCount--;
				wasDataChanged = true;
				
				RemoveVoxelIsland(VoxelIslandDetector.SplitAndReturnOtherIslands(GetVoxelData(),this));
			}
		}
		
		public void RemoveVoxelIsland(List<bool[,]> islandList){
			if(islandList != null && islandList.Count>0){
				foreach(bool[,] island in islandList){
					for (int x = 0; x < island.GetLength(0); x++) {
						for (int y = 0; y < island.GetLength(1); y++) {
							if(island[x,y] == true){
								voxelGrid[x,y].OnChangedSystem();
								voxelGrid[x,y] = null;
								voxelCount--;
							}
						}
					}
				}
			}
		}
		
		private void NeighbourUpdate(VoxelData vox){
			IntVector2 pos = vox.GetPosition();
			if(VoxelUtility.IsPointInBounds(GetGridSize(),new Vector2(pos.x,pos.y+1)) && !IsVoxelEmpty(pos.x,pos.y+1)){
				GetVoxel(pos.x,pos.y+1).OnNeighbourChange();
			} 
			if(VoxelUtility.IsPointInBounds(GetGridSize(),new Vector2(pos.x,pos.y-1)) && !IsVoxelEmpty(pos.x,pos.y-1)){
				GetVoxel(pos.x,pos.y-1).OnNeighbourChange();
			} 
			if(VoxelUtility.IsPointInBounds(GetGridSize(),new Vector2(pos.x+1,pos.y)) && !IsVoxelEmpty(pos.x+1,pos.y)){
				GetVoxel(pos.x+1,pos.y).OnNeighbourChange();
			} 
			if(VoxelUtility.IsPointInBounds(GetGridSize(),new Vector2(pos.x-1,pos.y)) && !IsVoxelEmpty(pos.x-1,pos.y)){
				GetVoxel(pos.x-1,pos.y).OnNeighbourChange();
			} 
		}
		
		
	}
}
