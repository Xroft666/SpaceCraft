using UnityEngine;
using System.Collections;
using Voxel2D;

public class Laser :  VoxelData{

	private float watt;
	private float range = 100;

	private LineRenderer lr;

	private bool enabled = false;

	public Laser(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel, float watt):base(elementID,pos,rotation, voxel){
		this.watt = watt;

		SetupLineRenderer();
	}

	private void SetupLineRenderer(){
		GameObject g = new GameObject("Laser line");
		g.transform.parent = voxel.transform;
		g.transform.localPosition = new Vector3(position.x,position.y,0);

		lr = g.AddComponent<LineRenderer>();

		lr.SetColors(Color.red,Color.red);

		//lr.useWorldSpace = false;
	}

	public override void OnDelete(){
		Object.Destroy(lr.gameObject);
	}

	public override void OnUpdate(){
		if(Input.GetKey(KeyCode.Space)){
			enabled = true;
		}else{
			enabled = false;
		}

		if(enabled){
			lr.SetPosition(0,voxel.transform.TransformPoint(new Vector3(position.x,position.y,0)));
			lr.gameObject.SetActive(true);
			Vector3 v = Quaternion.Euler(0,0,rotation)*new Vector3(0,1,0);
			
			Vector3 direction = voxel.transform.TransformDirection(v);
			Vector3 globalPos = voxel.transform.TransformPoint(new Vector3(position.x,position.y,0));

			RaycastHit2D hit = Physics2D.Raycast(globalPos+direction.normalized*1f, direction,range);
			if(hit.collider!=null){
				lr.SetPosition(1,new Vector3(hit.point.x,hit.point.y,0)+direction.normalized*0.5f);
			}else{
				lr.SetPosition(1,globalPos+direction.normalized*range);
			}
		}else{
			lr.gameObject.SetActive(false);
		}
	}

}
