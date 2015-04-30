using UnityEngine;
using System.Collections.Generic;

using System;

public static class EntitySelection 
{
	public delegate void OnContainerSelectedEvent( ContainerRepresentation container );
	public static OnContainerSelectedEvent onEntityClicked = null;

	public static List<ContainerRepresentation> selectedContainers = new List<ContainerRepresentation>();
	public static ContainerRepresentation selectedContainer = null;

	
	public static void OnEntityClicked( ContainerRepresentation entity )
	{
		if( entity == null )
			selectedContainers.Clear();
		else if( !selectedContainers.Contains( entity ) )
			selectedContainers.Add( entity );

		selectedContainer = entity;

		if( onEntityClicked != null )
			onEntityClicked( selectedContainer );
	}
}
