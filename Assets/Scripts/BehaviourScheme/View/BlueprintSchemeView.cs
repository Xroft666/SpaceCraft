using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

public class BlueprintSchemeView : MonoBehaviour 
{
	#region Unity references

	[Header("Inspector references")]
	// UI transforms references for all the UI sections
	public Transform m_hardwareList, m_hardwareBP, m_softwareBP, m_softwareAPI;

	#endregion

	#region data references

	private BlueprintScheme m_blueprint;

	private BSFunction m_root;
	private BSFunction m_currentView;

	private NodeView m_selectedNode;

	#endregion

	// Loading up and rendering all he containment
	public void LoadDevicesList( Container container )
	{
		foreach( Entity ent in container.GetCargoList() )
		{
			Device device = ent as Device;
			if( device == null )
				continue;

			// create a device UI representation
		}
	}

	public void UpdateSchemeView()
	{
		// Update all the visuals
	}

	#region Callback methods

	public void OnNodesConnected( NodeView left, NodeView right )
	{

	}

	public void OnNodesDropped( NodeView node )
	{
		// check if it was dropped over another node
	}

	public void OnNodeDragged( NodeView node )
	{
		// update position
	}

	public void OnNodeClicked( NodeView node )
	{
		// highlight and show info (or something)
		m_selectedNode = node;
	}

	#endregion
}
