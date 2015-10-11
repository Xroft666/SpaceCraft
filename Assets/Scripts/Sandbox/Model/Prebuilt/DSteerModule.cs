using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DSteerModule : Device 
{
//	private Rigidbody2D m_rigidbody;
	private Rigidbody m_rigidbody;

	// temporary variable. Should be changed to something more physical realistic
	private float torqueSpeed = 25f;

	#region device's functions

	public IEnumerator SteerTowards( DeviceQuery qry )
	{
		ArgsObject pArgs = qry.Invoke() as ArgsObject;

		DeviceTrigger onSteerStart = GetTrigger("OnSteerStart");
		if( onSteerStart != null )
			onSteerStart();

//		Vector2 worldPos = (Vector2) pArgs.position;
		Vector3 worldPos = (Vector3) pArgs.obj;

//		m_rigidbody.angularVelocity = 0f;
//		m_rigidbody.rotation = Mathf.Repeat( m_rigidbody.rotation, 360f );

		m_rigidbody.angularVelocity = Vector3.zero;

		float prevAngle = 1f;
		float currentAngle = 0f;

		while ( Mathf.Abs(currentAngle) < Mathf.Abs(prevAngle) )
		{
			Quaternion targetRotation = Quaternion.LookRotation( ( worldPos - m_rigidbody.position).normalized );

//			prevAngle = Mathf.DeltaAngle( m_rigidbody.rotation , CurrentAngle(worldPos)) ;
			prevAngle = Quaternion.Angle( m_rigidbody.rotation, targetRotation );
						//Vector3.Angle( m_rigidbody.rotation * Vector3.forward, ( worldPos - m_rigidbody.position).normalized);

//			m_rigidbody.rotation =
//						Mathf.MoveTowardsAngle( m_rigidbody.rotation, 
//			            CurrentAngle(worldPos), 
//			            torqueSpeed * Time.deltaTime );

			m_rigidbody.rotation = Quaternion.RotateTowards(m_rigidbody.rotation, targetRotation, torqueSpeed * Time.deltaTime );

//			currentAngle = Mathf.DeltaAngle( m_rigidbody.rotation , CurrentAngle(worldPos)) ;
			currentAngle = Quaternion.Angle( m_rigidbody.rotation, targetRotation );
							//Vector3.Angle( m_rigidbody.rotation * Vector3.forward, ( worldPos - m_rigidbody.position).normalized);
	
			yield return null;
		}

		DeviceTrigger onSteerComplete = GetTrigger("OnSteerComplete");
		if( onSteerComplete != null )
			onSteerComplete();
	}


	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddTrigger("OnSteerStart", null );
		AddTrigger("OnSteerComplete", null );

		AddAction("SteerTowards", SteerTowards );
	}

	public override void OnDeviceUninstalled()
	{
		RemoveTrigger("OnSteerStart" );
		RemoveTrigger("OnSteerComplete" );
		
		RemoveAction("SteerTowards" );
	}

	public override void Initialize()
	{
//		m_rigidbody = m_containerAttachedTo.View.GetComponent<Rigidbody2D>();
		m_rigidbody = m_containerAttachedTo.View.GetComponent<Rigidbody>();
	}

	public override void Update()
	{

	}

	#endregion


//	private float CurrentAngle( Vector3 worldPos )
//	{
//		Vector3 direction = ( (Vector2) worldPos - m_rigidbody.position).normalized;
//		return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
//	}
}
