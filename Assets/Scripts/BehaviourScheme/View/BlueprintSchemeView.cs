using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

public class BlueprintSchemeView : MonoBehaviour 
{
	#region Unity references

	[Header("Inspector references")]
	// UI transforms references for all the UI sections
	public Transform m_hardwareList;
	public Transform m_hardwareBP; 
	public Transform m_softwareBP; 
	public Transform m_softwareAPI;

	#endregion

	#region data references

	private BlueprintScheme m_blueprint;

	private BSFunction m_root;
	private BSFunction m_currentView;

	private NodeView m_selectedNode;

	#endregion

	public void InitializeView( BlueprintScheme scheme )
	{
		m_blueprint = scheme;
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

		BSFunction asFunction = node.Node as BSFunction;
		if( asFunction != null )
		{
			m_currentView = asFunction;
			UpdateSchemeView();
		}
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

	private void AddNodeToCurrentFunction( BSNode node )
	{
		// Add node where needed, create a node view, etc
		m_currentView.IncludeNode ( node );
	}
}
