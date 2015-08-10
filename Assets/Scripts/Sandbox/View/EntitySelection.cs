using UnityEngine;
using System.Collections.Generic;

using System;

public static class EntitySelection 
{
	public delegate void OnContainerSelectedEvent( ContainerView container );
	public static OnContainerSelectedEvent onEntityClicked = null;

	public static List<ContainerView> selectedContainers = new List<ContainerView>();
	public static ContainerView selectedContainer = null;

	
	public static void OnEntityClicked( ContainerView entity )
	{
		if( entity == null )
			selectedContainers.Clear();
		else if( !selectedContainers.Contains( entity ) )
			selectedContainers.Add( entity );

		selectedContainer = entity;

		if( onEntityClicked != null )
			onEntityClicked( selectedContainer );
	}

	public static void Cleanup()
	{
		selectedContainers.Clear();
	}
}
