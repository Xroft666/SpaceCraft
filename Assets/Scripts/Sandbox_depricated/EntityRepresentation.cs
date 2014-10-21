using UnityEngine;
using System.Collections.Generic;
using SpaceSandbox;

// meant to be a link between soft logic and unity representation layers
// may be used to represent a phycical entity in specific dimenstions
public class EntityRepresentation : MonoBehaviour 
{
//	public enum EntityType
//	{
//		ET_RESOURCE,
//		ET_ENGINE
//	}
//
//	public EntityType entityType;
//	public Entity detailToRepresent;
//
//	public float pullForce = 100;
//
//	void Awake()
//	{
//		switch( entityType )
//		{
//		case EntityType.ET_ENGINE:
//			
//			detailToRepresent = new Engine();
//			(detailToRepresent as Engine).pullForce = pullForce;
//			break;
//		}
//	}
//
//	void Start()
//	{
//		Device device = detailToRepresent as Device;
//		if( device != null )
//		{
//			device.outputCallback += ActivateEntity;
//			device.OnStart();
//
//			Engine engine = device as Engine;
//			if( engine != null )
//			{
////				Rigidbody2D rigid = gameObject.AddComponent<Rigidbody2D>();
////				rigid.gravityScale = 0f;
//				device.OnActivate(1f);
//			}
//		}
//	}
//
//	void Update()
//	{
//		Device device = detailToRepresent as Device;
//		if( device != null )
//			device.OnUpdate();
//	}
//
//	public void ActivateEntity( object[] deviceOutput )
//	{
//		switch( entityType )
//		{
//		case EntityType.ET_ENGINE:
//
//			float pullForce = (float) deviceOutput[0];
//
//			GetComponentInParent<Rigidbody2D>().AddForceAtPosition( transform.up * pullForce, transform.position, ForceMode2D.Force);
//			break;
//		}
//	}
}
