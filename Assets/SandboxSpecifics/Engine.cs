using UnityEngine;
using SpaceSandbox;
using Voxel2D;

public class Engine : Device 
{
	// engine force in newtons
	public float pullForce = 1000;

	public bool enabled = false;

	public Vector2 position = Vector2.zero;


	private VoxelSystem voxel;
	private Rigidbody2D body;

	// normalized speed (no speed, half speed, full speed
	private float _currentSpeed;
	
	public float Speed
	{
		get{ return _currentSpeed; }
		set{ _currentSpeed = Mathf.Clamp01(value);}
	}

	public override void OnActivate(params object[] input)
	{
		voxel = input[0] as VoxelSystem;
		Vector2 pos = (Vector2)input[1];
		pullForce = (float)input[2];

		position = pos;
		body = voxel.rigidbody2D;

		/*
		if( input.Length > 0 )
		{
			_currentSpeed = (float) input[0];
		}

		object[] outputData = new object[1] { pullForce * _currentSpeed };

		if( outputCallback != null )
			outputCallback(outputData);
			*/
	}

	public override void OnUpdate()
	{
		if(enabled){
			body.AddForceAtPosition(voxel.transform.TransformDirection(Vector2.up)*pullForce,voxel.transform.TransformPoint(position));
		}
	}
}
