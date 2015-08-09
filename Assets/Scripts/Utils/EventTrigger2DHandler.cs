using UnityEngine;
using System.Collections;


public class EventTrigger2DHandler : MonoBehaviour 
{
	public delegate void TriggerHandler( Collider2D other );

	public TriggerHandler onTriggerEnter = null;
	public TriggerHandler onTriggerExit = null;

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
}
