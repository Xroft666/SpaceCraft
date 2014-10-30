using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voxel2D;

public class ShipController : VoxelData {

	private bool delete = false;

	private List<Engine> engineList = new List<Engine>();

	private IEnumerator<Vector3> followPath = null;
	float interpolateValue = 0f;
	Vector3 prevPos;

	float vesselSpeed = 3f;

	public ShipController(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel):base(elementID,pos,rotation, voxel){
		stats.sizeModifier = 1;
		foreach(VoxelData sc in voxel.GetVoxelData()){
			if(sc is ShipController){
				
				if(sc != this){
					Debug.LogWarning("Ship controller allready exists in this system, deleting");
					delete = true;
				}
			}
		}

		voxel.VoxelSystemUpdated += updated;
	}

	private void updated(VoxelSystem vox){
		if(voxel != null){
			findEngines();
		}
	}

	private void findEngines(){
		engineList.Clear();
		foreach(VoxelData e in voxel.GetVoxelData())
		{
			if(e is Engine)
			{
				engineList.Add((Engine)e);
			}
		}
	}

	public override void OnDelete()
	{
		if(voxel != null)
		{
			voxel.VoxelSystemUpdated -= updated;
		}
	}

	public override void OnUpdate()
	{
		if(delete)
		{
			voxel.RemoveVoxel(position.x,position.y);
		}

		List<Engine> leftEngines = new List<Engine>();
		List<Engine> rightEngines = new List<Engine>();
		List<Engine> forwardEngines = new List<Engine>();
		List<Engine> backwardEngines = new List<Engine>();

		CollectThrusters(ref leftEngines, ref rightEngines, ref forwardEngines, ref backwardEngines);
		InputThrusters(ref leftEngines, ref rightEngines, ref forwardEngines, ref backwardEngines);

//		RTSControl();
	}

	public void CollectThrusters(ref List<Engine> left, ref List<Engine> right, ref List<Engine> forward, ref List<Engine> backward)
	{
		foreach(VoxelData e in voxel.GetVoxelData())
		{
			Voxel2D.IntVector2 pos = GetPosition();
			if(e is Engine)
			{
//				Voxel2D.IntVector2 engPos = e.GetPosition();
//				
//				all.Add((Engine)e);
//				
//				if( engPos.x < pos.x )
//				{
//					left.Add((Engine)e);
//					continue;
//				}
//				if( engPos.x > pos.x )
//				{
//					right.Add((Engine)e);
//					continue;
//				}

				switch(e.rotation)
				{
				case 0:
					forward.Add((Engine)e);
					break;
				case 90:
					left.Add((Engine)e);
					break;
				case 270:
					right.Add((Engine)e);
					break;
				case 180:
					backward.Add((Engine)e);
					break;
				}
			}
		}
	}

	public void InputThrusters(ref List<Engine> left, ref List<Engine> right, ref List<Engine> forward, ref List<Engine> backward )
	{
		if(Input.GetKeyDown(KeyCode.W))
			foreach(Engine e in forward)
				e.OnActivate();
		
		if(Input.GetKeyUp(KeyCode.W))
			foreach(Engine e in forward)
				e.OnDeactivate();

		if(Input.GetKeyDown(KeyCode.S))
			foreach(Engine e in backward)
				e.OnActivate();
		
		if(Input.GetKeyUp(KeyCode.S))
			foreach(Engine e in backward)
				e.OnDeactivate();
		
		if(Input.GetKeyDown(KeyCode.A))
			foreach(Engine e in right)
				e.OnActivate();
		
		if(Input.GetKeyUp(KeyCode.A))
			foreach(Engine e in right)
				e.OnDeactivate();
		
		if(Input.GetKeyDown(KeyCode.D))
			foreach(Engine e in left)
				e.OnActivate();
		
		if(Input.GetKeyUp(KeyCode.D))
			foreach(Engine e in left)
				e.OnDeactivate();
	}

	public void RTSControl()
	{
		if( Input.GetMouseButtonDown(0) )
		{
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = -Camera.main.transform.position.z;

			Vector3 worldClickPos = Camera.main.ScreenToWorldPoint( mousePos );

			Vector3 shipPos = voxel.transform.TransformPoint(voxel.GetCenter());

			float curVelocity = voxel.rigidbody2D.velocity.magnitude;

			Vector3 middleWayPoint = voxel.transform.up * curVelocity + shipPos;

			followPath = Interpolate.NewCatmullRom( new Vector3[]{shipPos, middleWayPoint, worldClickPos}, 10, false ).GetEnumerator();

			followPath.MoveNext();
			prevPos = shipPos;//voxel.transform.position;
//			Vector3 currentPos = followPath.Current;
//
//			while( followPath.MoveNext() )
//			{
//				Debug.DrawLine( currentPos, followPath.Current , Color.red, 1f );
//				currentPos = followPath.Current;
//			}
		}

		if( followPath != null )
		{
//			Vector3 centPos = voxel.GetCenter();
//			interpolateValue = 1f;

			voxel.transform.position = Vector3.Lerp(prevPos, followPath.Current, interpolateValue);
			interpolateValue += Time.deltaTime * vesselSpeed;

			if( interpolateValue >= 1f ) 
			{
				prevPos = followPath.Current;
				if( !followPath.MoveNext() )
					followPath = null;

				interpolateValue = 0f;
			}
		}
	}
}
