using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

public class BlueprintSchemeView : MonoBehaviour 
{
	#region data references

	private Device m_device;
	private NodeView m_selectedNode;

	#endregion

	public void InitializeView( Device device )
	{
		m_device = device;
	}

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

		// Instantiate Node Views only for the current VIEW
	}

	#region Callback methods

	public void OnNodesConnected( NodeView left, NodeView right )
	{

	}

	public void OnNodesDropped( NodeView left, NodeView right )
	{

	}

	public void OnNodeDragged( NodeView node )
	{
		// update visuals
	}

	public void OnNodeClicked( NodeView node )
	{
		// highlight and show info (or something)
		m_selectedNode = node;
	}

	// If a Function was double click, inception the View inside
	public void OnNodeDoubleClicked( NodeView node )
	{
		m_selectedNode = node;

		// Check if the node represents a composite Device
		// load up its Blueprint scheme
		// update UI
	}

	#endregion

	#region Elements section butttons callback

	public void CreateFunction()
	{
		BSNode newNode = null;// = m_blueprint.CreateFunction(
		AddNodeToCurrentFunction( newNode );
	}
	
	public void CreateAction()
	{
		BSNode newNode = null;// = m_blueprint.CreateAction(
		AddNodeToCurrentFunction( newNode );
	}
	
	public void CreateEntry()
	{
		BSNode newNode = null;// = m_blueprint.CreateEntry(
		AddNodeToCurrentFunction( newNode );
	}
	
	public void CreateExit()
	{
		BSNode newNode = null;// = m_blueprint.CreateExit(
		AddNodeToCurrentFunction( newNode );
	}
	
	public void CreateSelect()
	{
		BSNode newNode = null;// = m_blueprint.CreateSelect(
		AddNodeToCurrentFunction( newNode );
	}
	
	public void CreateEvaluate()
	{
		BSNode newNode = null;// = m_blueprint.CreateEvaluate(
		AddNodeToCurrentFunction( newNode );
	}

	#endregion

	public void AddNodeToCurrentFunction( BSNode node )
	{

	}
}
