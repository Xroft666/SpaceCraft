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

		public ElementStats stats;

		public int rotation;

		public VoxelSystem voxel;

		protected IntVector2 position;

		int elementID;
		
		public VoxelData(int elementID, IntVector2 pos, int rotation, VoxelSystem voxel){
			this.elementID = elementID;		
			this.rotation = rotation;
			this.voxel = voxel;
			position = pos;
			stats = new ElementStats(elementID);
			deviceName = this.GetType().Name;
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



		#region Voxels Interface

		public delegate void DeviceCallback( object[] output );
		public DeviceCallback outputCallback;
		
		public virtual void OnStart(params object[] input){}
		public virtual void OnFixedUpdate(){}
		public virtual void OnUpdate(){}
		public virtual void OnDelete(){}

		public virtual void OnNeighbourChange(){}
		public virtual void OnSystemChange(){}

		public virtual void OnActivate(params object[] input){}
		public virtual void OnDeactivate(params object[] input){}

		#endregion

	}
}