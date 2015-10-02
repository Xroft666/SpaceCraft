using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class BlueprintSchemeView : MonoBehaviour, IDropHandler
{
	#region data references

	private Device m_device;
	private NodeView m_selectedNode;

	private RectTransform blueprintRect;

	#endregion

	float horizDistance = 300f;
	float vertDistance = -50f;

	private void Awake()
	{
		blueprintRect = transform.FindChild("BlueprintView/Content") as RectTransform;
	}

	public void InitializeView( Device device )
	{
		m_device = device;

		foreach( BSNode node in m_device.Blueprint.m_nodes )
			GenerateNode( node );
		
		foreach( BSNode node in m_device.Blueprint.m_nodes )
			PositionNode( node );


//		ConflictsTraverse(device.Blueprint.m_entryPoint);
		GenerateConnections(device.Blueprint.m_entryPoint);



		Rect box = new Rect();
		GetBoundingBox(device.Blueprint.m_entryPoint, ref box); 

		blueprintRect.sizeDelta = new Vector3(box.width * horizDistance * 1.5f, -box.height * vertDistance * 5f + box.height * vertDistance * 2.5f);
	
	}

	private void GenerateNode( BSNode current )
	{
		string name = string.IsNullOrEmpty(current.m_name) ? "" : ": " + current.m_name;
		CreateNode (current.m_type + name, current );
	}

	public void PositionNode( BSNode current )
	{
		Vector2 globalOffset = -Vector2.up * 2.5f;
		int subling = 0;
		int sublingsCount = 1;

		if( current.m_parents.Count > 0 )
		{
			BSNode parentNode = current.m_parents[0];

			globalOffset = parentNode.m_view.globalOffset;
			subling = parentNode.m_children.IndexOf( current );
			sublingsCount = parentNode.m_children.Count;
		}

		NodeView view = current.m_view;

		Vector2 localOffset = new Vector2( sublingsCount == 1 ? 0f : ((subling / (float)(sublingsCount - 1)) - 0.5f), 1f );

		BSQuery query = current as BSQuery;
		if( query != null && query.connectedActionNode != null)
		{
			globalOffset = query.connectedActionNode.m_view.globalOffset;
			localOffset = Vector2.right * -2.0f;
		}
		

		current.m_view.globalOffset = globalOffset + localOffset;
		current.m_view.transform.localPosition = new Vector3(current.m_view.globalOffset.x * horizDistance , current.m_view.globalOffset.y * vertDistance);


		for( int i = 0; i < current.m_children.Count; i++ )
			PositionNode( current.m_children[i] );
	}

	private void GenerateConnections(BSNode current)
	{
		for( int i = 0; i < current.m_children.Count; i++ )
		{
			NodeViewDock outputDock = current.m_view.AddOutputDock();
			NodeViewDock inputDock = current.m_children[i].m_view.AddInputDock();
			
			outputDock.ConnectToNode( RectTransformUtility.WorldToScreenPoint( UIController.s_UICamera, inputDock.transform.position ), inputDock);
		}

		for( int i = 0; i < current.m_children.Count; i++ )
			GenerateConnections( current.m_children[i] );
	}

	private void ConflictsTraverse( BSNode current )
	{
		switch( current.m_children.Count )
		{
		case 0:
			break;
		case 1:
			ConflictsTraverse ( current.m_children[0] );
			break;
		default:
			for( int i = 0; i < current.m_children.Count - 1; i ++ )
				ResolveOverlap(current.m_children[i], current.m_children[i+1]);

			for( int i = 0; i < current.m_children.Count; i ++ )
				ConflictsTraverse(current.m_children[i]);
			break;
		}
	}

	private void ResolveOverlap( BSNode left, BSNode right )
	{
		Rect leftRect = new Rect();
		Rect rightRect = new Rect();

		GetBoundingBox( left, ref leftRect );
		GetBoundingBox( right, ref rightRect );

		if( leftRect.Overlaps(rightRect) )
			ShiftSubtreePosition( right, new Vector2((rightRect.center - leftRect.center).x, 0f) );
	}

	private void GetBoundingBox( BSNode current, ref Rect box )
	{
		box.xMin = Mathf.Min( current.m_view.globalOffset.x, box.xMin);
		box.xMax = Mathf.Max( current.m_view.globalOffset.x, box.xMax);

		box.yMin = Mathf.Min( current.m_view.globalOffset.y, box.yMin);
		box.yMax = Mathf.Max( current.m_view.globalOffset.y, box.yMax);

		foreach( BSNode child in current.m_children )
			GetBoundingBox( child, ref box );
	}

	private void ShiftSubtreePosition( BSNode current, Vector2 offset )
	{
		current.m_view.globalOffset += offset;
		current.m_view.transform.localPosition = new Vector3(current.m_view.globalOffset.x * horizDistance , current.m_view.globalOffset.y * vertDistance);

		foreach( BSNode child in current.m_children )
			ShiftSubtreePosition( child, offset );
	}


	public void UpdateSchemeView()
	{
		// Update all the visuals

		// Instantiate Node Views only for the current VIEW
	}

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if( eventData.selectedObject == null )
			return;

		// Draggable object
		DeviceItem item = eventData.selectedObject.GetComponent<DeviceItem>();
		if( item == null )
			return;


	
	}
	#endregion


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

	private NodeView CreateNode( string itemName, BSNode node )
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
		nodeView.Node = node;
		node.m_view = nodeView;


		return nodeView;
	}
}
