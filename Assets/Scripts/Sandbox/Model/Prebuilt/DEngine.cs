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

//	private Rigidbody2D m_rigidbody = null;
	private Rigidbody m_rigidbody = null;

	#region device's functions


	public IEnumerator MoveForward( DeviceQuery qry )//EventArgs args )
	{
		ApplyForce();

		yield break;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		base.OnDeviceInstalled();

		AddAction("MoveForward", MoveForward );
	}

	public override void Initialize()
	{
//		m_rigidbody = m_containerAttachedTo.View.gameObject.GetComponent<Rigidbody2D>();
		m_rigidbody = m_containerAttachedTo.View.gameObject.GetComponent<Rigidbody>();
	}

	public override void Update()
	{
		if( m_isActive )
			ApplyForce();
	}

	public override void Destroy()
	{
	
	}

	#endregion

	private void ApplyForce()
	{
		// Move the object and consume fuel

		Vector3 dir = m_lookDirection.normalized;
		if( m_space == Space.Self )
			dir = m_containerAttachedTo.View.transform.TransformDirection( m_lookDirection ).normalized;

		m_rigidbody.AddForce( dir * speed * Time.deltaTime, ForceMode.Force);//ForceMode2D.Force );
	}
}
