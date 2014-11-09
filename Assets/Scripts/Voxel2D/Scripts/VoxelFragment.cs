using UnityEngine;
using System.Collections;
using MaterialSystem;
using SpaceSandbox;

namespace Voxel2D{
	[RequireComponent(typeof(BoxCollider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class VoxelFragment : MonoBehaviour {

		public VoxelData voxel {get;private set;}

		public void Init(VoxelData vox){
			voxel = vox;
			vox.voxel = null;

			//for mesh creation
			VoxelData[,] voxelGrid = new VoxelData[1,1];
			voxelGrid[0,0] = vox;
			vox.OnSystemChange(null);

			MeshFilter mFilter = GetComponent<MeshFilter>();
			Mesh mesh = mFilter.sharedMesh;

			VoxelMeshGenerator.VoxelToMesh(voxelGrid, ref mesh);


//			if( mFilter.sharedMesh != null )
//			DestroyImmediate(mFilter.sharedMesh);

//			mFilter.sharedMesh = null;
//			mFilter.sharedMesh = mesh;

			rigidbody2D.mass = vox.stats.mass;

			gameObject.AddComponent<VoxelTextureHandler>();
			gameObject.AddComponent<PropertyOverlay>();

			rigidbody2D.angularDrag = 0;
		}

		// Use this for initialization
		void Awake () {
			BoxCollider2D bc = GetComponent<BoxCollider2D>();
			bc.center = Vector2.zero;
			bc.size = Vector2.one;
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
