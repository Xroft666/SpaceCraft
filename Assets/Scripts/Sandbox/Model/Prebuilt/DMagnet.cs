using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DMagnet : Device 
{
	// Exportable variable
	public float m_magnetPower = 10f;


	public List<Rigidbody2D> m_attractTargets = new List<Rigidbody2D>();
	public List<Rigidbody2D> m_repusleTargets = new List<Rigidbody2D>();

	private Rigidbody2D myRigid;


	#region device's functions

	private IEnumerator Attract( EventArgs args )
	{
		ContainerArgs cArgs = args as ContainerArgs;
		m_attractTargets.Add( cArgs.container.View.GetComponent<Rigidbody2D>() );

		yield break;
	}

	private IEnumerator Repulse( EventArgs args )
	{
		ContainerArgs cArgs = args as ContainerArgs;
		m_repusleTargets.Add( cArgs.container.View.GetComponent<Rigidbody2D>() );

		yield break;
	}

	private IEnumerator RemoveTarget( EventArgs args )
	{
		ContainerArgs cArgs = args as ContainerArgs;
		Rigidbody2D view = cArgs.container.View.GetComponent<Rigidbody2D>();

		if( m_attractTargets.Contains(view) )
			m_attractTargets.Remove( view );

		if( m_repusleTargets.Contains(view) )
			m_repusleTargets.Remove( view );

		yield break;
	}
	

	#endregion

	#region Predecates 



	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddAction("Attract", Attract );
		AddAction("Repulse", Repulse);
		AddAction("RemoveTarget", Repulse);
	}

	public override void Initialize()
	{
		myRigid = m_containerAttachedTo.View.GetComponent<Rigidbody2D>();
	}

	public override void Update()
	{
		foreach( Rigidbody2D obj in m_attractTargets )
			obj.AddForce( (myRigid.position - obj.position).normalized * m_magnetPower );
		
		foreach( Rigidbody2D obj in m_repusleTargets )
			obj.AddForce( (obj.position - myRigid.position).normalized * m_magnetPower );
	}

	#endregion


}
