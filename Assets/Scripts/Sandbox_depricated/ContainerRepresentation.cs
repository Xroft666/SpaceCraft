using UnityEngine;

using SpaceSandbox;
using System.Collections.Generic;


[RequireComponent(typeof( BoxCollider2D ))]
[RequireComponent(typeof( EventTriggerInitializer ))]
public class ContainerRepresentation : MonoBehaviour 
{

	public Container _contain;

	private void Awake()
	{

	}

//	public List<Container> _contains = new List<Container>();
//
//	void Start () 
//	{
//		foreach( Container container in _contains )
//		{
//			foreach( Entity entity in container.cargo )
//			{
//				GameObject entityDetail = new GameObject(entity.name); 
//				EntityRepresentation representation = entityDetail.AddComponent<EntityRepresentation>();
//				representation.detailToRepresent = entity;
//			}
//		}
//	}
//
//	void Update () {
//	
//	}
	

}
