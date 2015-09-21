using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;
using UnityEngine.UI;

public class BlueprintSchemeView : MonoBehaviour 
{
	#region data references

	private Device m_device;
	private NodeView m_selectedNode;

	private RectTransform blueprintRect;

	#endregion

	private void Awake()
	{
		blueprintRect = transform.FindChild("BlueprintView/Content") as RectTransform;
	}

	public void InitializeView( Device device )
	{
		m_device = device;


		for( int i = 0; i < device.Blueprint.m_nodes.Count; i++ )
		{
			CreateNode ("node", Vector3.zero);
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

	private void CreateNode( string itemName, Vector3 position )
	{
		Vector2 iconSize = new Vector2(100f, 20f);
		
		GameObject newAction = new GameObject(itemName, typeof(RectTransform));
		newAction.transform.SetParent( blueprintRect, false );
		
		RectTransform transf = newAction.GetComponent<RectTransform>();
		transf.sizeDelta = iconSize;
		

		
		GameObject background = new GameObject("background", typeof(RectTransform));
		RectTransform backTransf = background.GetComponent<RectTransform>();
		backTransf.sizeDelta = iconSize;
		backTransf.SetParent(transf, false);
		
		Image backImg = background.AddComponent<Image>();
		
		
		GameObject textGO = new GameObject("text", typeof(RectTransform));
		RectTransform textTransf = textGO.GetComponent<RectTransform>();
		textTransf.sizeDelta = iconSize;
		textTransf.SetParent(transf, false);
		
		
		Text text = textGO.AddComponent<Text>();
		text.text = itemName;
		text.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
		text.alignment = TextAnchor.MiddleCenter;
		text.color = Color.black;
		
		
		
		Selectable selectable = newAction.AddComponent<Selectable>();
		CanvasGroup canvas = newAction.AddComponent<CanvasGroup>();
		DeviceItem item = newAction.AddComponent<DeviceItem>();

		NodeView nodeView = newAction.AddComponent<NodeView>();
		
		
		newAction.transform.localPosition = position;
	}
}
