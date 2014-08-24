using UnityEngine;
using System.Collections.Generic;
using SpaceSandbox;

// meant to be a link between soft logic and unity representation layers
// may be used to represent a phycical entity in specific dimenstions
public class EntityRepresentation : MonoBehaviour 
{
	public Entity detailToRepresent;

	void Start()
	{

	}

	void Update()
	{
		Gizmos.DrawCube( transform.position, Vector3.one );
	}
}
