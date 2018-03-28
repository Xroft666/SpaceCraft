using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;


public class DEngine : Device 
{
	// temporary variable. Should be changed to something more physical realistic
	public float speed = 100f;
	public Vector3 m_lookDirection = Vector3.forward;
	public Space m_space;

	private Rigidbody m_rigidbody = null;

	#region device's functions


	public IEnumerator MoveForward( DeviceQuery qry )
	{
		ApplyForce();

		yield break;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		base.OnDeviceInstalled();

		m_blueprint.AddAction("MoveForward", MoveForward );
	}

	public override void OnDeviceUninstalled()
	{
		base.OnDeviceUninstalled();
		
		m_blueprint.RemoveAction("MoveForward" );
	}


	public override void Initialize()
	{
		m_rigidbody = m_container.View.gameObject.GetComponent<Rigidbody>();
	}
		
	public override void Update()
	{
		if( m_isActive )
			ApplyForce();
	}

	#endregion

	private void ApplyForce()
	{
		// Move the object and consume fuel

		Vector3 dir = m_lookDirection.normalized;
		if( m_space == Space.Self )
			dir = m_container.View.transform.TransformDirection( m_lookDirection ).normalized;

		m_rigidbody.AddForce( dir * speed * Time.deltaTime, ForceMode.Force);
	}

    public override string ToString()
    {
        return "Engine";
    }
}
