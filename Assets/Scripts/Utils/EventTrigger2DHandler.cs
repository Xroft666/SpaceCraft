using UnityEngine;
using System.Collections;


public class EventTrigger2DHandler : MonoBehaviour 
{
	public delegate void TriggerHandler( Collider2D other );
	public delegate void CollisionHandler( Collision2D collision );

	public TriggerHandler onTriggerEnter = null;
	public TriggerHandler onTriggerExit = null;

	public CollisionHandler onCollisionEnter = null;
	public CollisionHandler onCollisionExit = null;

	private void OnTriggerEnter2D( Collider2D other )
	{
		if( onTriggerEnter != null )
			onTriggerEnter.Invoke( other );
	}

	private void OnTriggerExit2D( Collider2D other )
	{
		if( onTriggerExit != null )
			onTriggerExit.Invoke( other );
	}

	private void OnCollisionEnter2D( Collision2D collision )
	{
		if( onCollisionEnter != null )
			onCollisionEnter.Invoke( collision );
	}
	
	private void OnCollisionExit2D( Collision2D collision )
	{
		if( onCollisionExit != null )
			onCollisionExit.Invoke( collision );
	}
}
