﻿using UnityEngine;
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
			VoxelData[,] voxelGrid = new VoxelData[1,1];
			voxelGrid[0,0] = vox;
			Mesh mesh = VoxelMeshGenerator.VoxelToMesh(voxelGrid);
			GetComponent<MeshFilter>().sharedMesh = mesh;

			rigidbody2D.mass = ElementList.Instance.elements[voxel.GetElementID()].mass;

			gameObject.AddComponent<VoxelTextureHandler>();

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
