using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MaterialSystem;
using SpaceSandbox;

namespace Voxel2D{
	/// <summary>
	/// Holds data about the voxel
	/// </summary>
	public class VoxelData{

		public string deviceName;
	    public readonly DeviceData.DeviceType deviceType;

		public ElementStats stats;

		public int rotation;

		public int[] vertexIndexes;

		public VoxelSystem voxel;

		protected IntVector2 position;

		protected int elementID;

		protected List<PhysicalProperty> propertyList = new List<PhysicalProperty>();

		public VoxelData VDU{get;private set;}
		public VoxelData VDD{get;private set;}
		public VoxelData VDL{get;private set;}
		public VoxelData VDR{get;private set;}
		
		public VoxelData(int elementID, IntVector2 pos, int rotation, VoxelSystem voxel){
			this.elementID = elementID;		
			this.rotation = rotation;
			this.voxel = voxel;
			position = pos;
			stats = new ElementStats(elementID);
			deviceName = this.GetType().Name;
            deviceType = (DeviceData.DeviceType) System.Enum.Parse(typeof(DeviceData.DeviceType), deviceName);

			vertexIndexes = new int[4];
		}
		
		public void SetElementID(int ID){
			this.elementID = ID;
		}
		
		public int GetElementID(){
			return elementID;
		}

		public IntVector2 GetPosition(){
			return position;
		}

		public void SetPosition(IntVector2 pos){
			position = pos;
		}

		/// <summary>
		/// Neighbour of voxel was changed
		/// </summary>
		public virtual void OnNeighbourChange(){
			if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),new Vector2(position.x,position.y+1)) && !voxel.IsVoxelEmpty(position.x,position.y+1)){
				VDU = voxel.GetVoxel(position.x,position.y+1);
			} 
			if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),new Vector2(position.x,position.y-1)) && !voxel.IsVoxelEmpty(position.x,position.y-1)){
				VDD = voxel.GetVoxel(position.x,position.y-1);
			} 
			if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),new Vector2(position.x+1,position.y)) && !voxel.IsVoxelEmpty(position.x+1,position.y)){
				VDR = voxel.GetVoxel(position.x+1,position.y);
			} 
			if(VoxelUtility.IsPointInBounds(voxel.GetGridSize(),new Vector2(position.x-1,position.y)) && !voxel.IsVoxelEmpty(position.x-1,position.y)){
				VDL = voxel.GetVoxel(position.x-1,position.y);
			}
		}


		#region Voxels Interface

		public delegate void DeviceCallback( object[] output );
		public DeviceCallback outputCallback;
		
		public virtual void OnStart(params object[] input){}
		public virtual void OnFixedUpdate(){}
		public virtual void OnUpdate(){
			for(int i=0;i<propertyList.Count;i++){
				propertyList[i].OnUpdate(this);
			}
		}

		/// <summary>
		/// Voxel was deleted
		/// </summary>
		public virtual void OnDelete(){}
		/// <summary>
		/// Voxel changed system
		/// </summary>
		public virtual void OnChangedSystem(){}


		/// <summary>
		/// There was a change in the voxel system
		/// </summary>
		public virtual void OnSystemChange(VoxelSystem voxel){}

		public virtual void OnActivate(params object[] input){}
		public virtual void OnDeactivate(params object[] input){}

		#endregion

	}

	public class VoxelRawData
	{
		public VoxelRawData(int deviceType,
		                    int materialType,
		                    int xPos,
		                    int yPos,
		                    int rotation)
		{
			_deviceType = deviceType;
			_materialType = materialType;
			_xPos = xPos;
			_yPos = yPos;
			_rotation = rotation;
		}

		public int _deviceType;
		public int _materialType;

		public int _xPos;
		public int _yPos;
		public int _rotation;
	
//		protected List<PhysicalProperty> propertyList = new List<PhysicalProperty>();
//		public string deviceName;		
//		public ElementStats stats;
	}
}