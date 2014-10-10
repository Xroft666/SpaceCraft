using UnityEngine;
using System.Collections;

public class AstroidImpacter : MonoBehaviour {

	Voxel2D.VoxelSystem voxel;

	// Use this for initialization
	void Awake () {
		voxel = GetComponent<Voxel2D.VoxelSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnCollisionEnter2D(Collision2D col){
		Vector2 pos = col.contacts[0].point;

		pos = transform.InverseTransformPoint(pos);
		pos.x = Mathf.Round(pos.x);
		pos.y = Mathf.Round(pos.y);


		int[] vox = voxel.GetClosestVoxelIndex(pos);
		voxel.RemoveVoxel(vox[0],vox[1]);
		
		voxel.SetMesh(Voxel2D.VoxelMeshGenerator.VoxelToMesh(voxel.GetVoxelData()));
		
	}
}
