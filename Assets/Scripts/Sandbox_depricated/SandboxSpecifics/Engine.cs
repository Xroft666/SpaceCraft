using UnityEngine;
using SpaceSandbox;

public class Engine : Device 
{
	// engine force in newtons
	public float pullForce = 100;

	// normalized speed (no speed, half speed, full speed
	private float _currentSpeed;
	
	public float Speed
	{
		get{ return _currentSpeed; }
		set{ _currentSpeed = Mathf.Clamp01(value);}
	}

	public override void OnActivate(params object[] input)
	{
		if( input.Length > 0 )
		{
			_currentSpeed = (float) input[0];
		}

		object[] outputData = new object[1] { pullForce * _currentSpeed };

		if( outputCallback != null )
			outputCallback(outputData);
	}

	public override void OnUpdate()
	{

	}
}
