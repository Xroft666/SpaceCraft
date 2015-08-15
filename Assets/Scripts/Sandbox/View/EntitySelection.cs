using UnityEngine;
using System.Collections.Generic;

using System;

public static class EntitySelection 
{
	public delegate void OnContainerSelectedEvent( ContainerView container );
	public static OnContainerSelectedEvent onEntityClicked = null;
	public static Action onCleanSpaceClicked = null;

	public static List<ContainerView> selectedContainers = new List<ContainerView>();
	public static ContainerView selectedContainer = null;

	
	public static void OnEntityClicked( ContainerView entity )
	{
		selectedContainer = entity;

		if( entity == null )
		{
			Cleanup();

			if( onCleanSpaceClicked != null )
				onCleanSpaceClicked();
		}
		else 
		{
			if( onEntityClicked != null )
				onEntityClicked( selectedContainer );
		}
	}

	public static void Cleanup()
	{
		selectedContainers.Clear();
	}

	private static void AddEntity( ContainerView entity )
	{
		if( !selectedContainers.Contains( entity ) )
			selectedContainers.Add( entity );
	}
}
