using UnityEngine;
using System.Collections;

public class EventTriggerHandler : MonoBehaviour {

	public delegate void TriggerHandler( Collider other );
	public delegate void CollisionHandler( Collision collision );
	
	public TriggerHandler onTriggerEnter = null;
	public TriggerHandler onTriggerExit = null;
	
	public CollisionHandler onCollisionEnter = null;
	public CollisionHandler onCollisionExit = null;

	private void OnTriggerEnter( Collider other )
	{
		if( onTriggerEnter != null )
			onTriggerEnter.Invoke( other );
	}
	
	private void OnTriggerExit( Collider other )
	{
		if( onTriggerExit != null )
			onTriggerExit.Invoke( other );
	}
	
	private void OnCollisionEnter( Collision collision )
	{
		if( onCollisionEnter != null )
			onCollisionEnter.Invoke( collision );
	}
	
	private void OnCollisionExit( Collision collision )
	{
		if( onCollisionExit != null )
			onCollisionExit.Invoke( collision );
	}
}
