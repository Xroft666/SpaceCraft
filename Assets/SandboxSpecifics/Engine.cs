using UnityEngine;
using SpaceSandbox;

public class Engine : Device 
{
	// engine force in newtons
	public float pullForce;

	// normalized speed (no speed, half speed, full speed
	private float _currentSpeed;

	public float Speed
	{
		get{ return _currentSpeed; }
		set{ _currentSpeed = Mathf.Clamp01(value);}
	}
}
