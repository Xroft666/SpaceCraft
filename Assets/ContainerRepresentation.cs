using UnityEngine;
using SpaceSandbox;
using System.Collections.Generic;

// represents a small detail - tile, device, anything graphical
public class ContainerRepresentation : MonoBehaviour 
{
	public List<Container> _contains = new List<Container>();

	void Start () 
	{
		foreach( Container container in _contains )
		{
			foreach( Entity entity in container.cargo )
			{
				GameObject entityDetail = new GameObject(entity.name); 
				EntityRepresentation representation = entityDetail.AddComponent<EntityRepresentation>();
				representation.detailToRepresent = entity;
			}
		}
	}

	void Update () {
	
	}
}
