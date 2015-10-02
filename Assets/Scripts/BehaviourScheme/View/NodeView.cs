using UnityEngine;
using System.Collections.Generic;

using BehaviourScheme;

using UnityEngine.EventSystems;

// the visual representation of a node
// it s a monobehaviour, so it contains all the callbacks, visuals and references 
public class NodeView : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	// blueprint position
//	public float x;
//	public float y;
//	[HideInInspector]
	public Vector2 globalOffset;

	public BSNode Node { get; set; }
	public BlueprintSchemeView SchemeView { get; private set; }
	

	public List<NodeViewDock> m_inputs = new List<NodeViewDock>();
	public List<NodeViewDock> m_outputs = new List<NodeViewDock>();
	public List<NodeViewDock> m_queries = new List<NodeViewDock>();
	public List<NodeViewDock> m_predecates = new List<NodeViewDock>();

	private void Awake()
	{
		AddInputDock();
		AddOutputDock();
		AddQueryDock();
		AddPredecatesDock();
	}

	private NodeViewDock CreateDock( string name, Color color, Vector3 position)
	{
		GameObject inputGo = new GameObject(name, typeof(RectTransform) );
		RectTransform m_inputDockTr = inputGo.transform as RectTransform;	
		m_inputDockTr.transform.SetParent( transform, false );
		
		m_inputDockTr.sizeDelta = Vector2.one * 10;
		
		m_inputDockTr.transform.localPosition = position;
		
		NodeViewDock inputDock = inputGo.AddComponent<NodeViewDock>();
		inputDock.AssignNode( this, color );

		return inputDock;
	}
	
	public NodeViewDock AddInputDock()
	{
		Vector3 position = Vector3.up * 10f + Vector3.right * 15f * m_inputs.Count;
		NodeViewDock dock = CreateDock("input", Color.red, position);
		m_inputs.Add(dock);

		dock.m_onDrag = OnInputDrag;
		dock.m_onDragBegin = OnInputDragBegin;
		dock.m_onDragEnd = OnInputDragEnd;
		dock.m_onDrop = OnInputDockDropped;

		return dock;
	}

	public void RemoveInputDock(NodeViewDock dock)
	{
		m_inputs.Remove( dock );
		GameObject.Destroy( dock.gameObject );
	}

	public NodeViewDock AddOutputDock()
	{
		Vector3 position = -Vector3.up * 10f + Vector3.right * 15f * m_outputs.Count;
		NodeViewDock dock = CreateDock("output" + m_outputs.Count, Color.red, position);
		m_outputs.Add(dock);

		dock.m_onDrag = OnOutputDrag;
		dock.m_onDragBegin = OnOutputDragBegin;
		dock.m_onDragEnd = OnOutputDragEnd;
		dock.m_onDrop = OnOutputDockDropped;

		return dock;
	}
	
	public void RemoveOutputDock( NodeViewDock dock )
	{
		m_outputs.Remove( dock );
		GameObject.Destroy( dock.gameObject );
	}

	public NodeViewDock AddQueryDock()
	{
		Vector3 position = Vector3.right * 50f + Vector3.up * 15f * m_queries.Count;
		NodeViewDock dock = CreateDock("query" + m_queries.Count, Color.blue, position);
		m_queries.Add(dock);

		dock.m_onDrag = OnQueryDrag;
		dock.m_onDragBegin = OnQueryDragBegin;
		dock.m_onDragEnd = OnQueryDragEnd;
		dock.m_onDrop = OnQueryDockDropped;

		return dock;
	}
	
	public void RemoveQueryDock(NodeViewDock dock)
	{
		m_queries.Remove( dock );
		GameObject.Destroy( dock.gameObject );
	}

	public NodeViewDock AddPredecatesDock()
	{
		Vector3 position = -Vector3.right * 50f - Vector3.up * 15f * m_predecates.Count;
		NodeViewDock dock = CreateDock("query" + m_predecates.Count, Color.green, position);
		m_predecates.Add(dock);

		dock.m_onDrag = OnPredecateDrag;
		dock.m_onDragBegin = OnPredecateDragBegin;
		dock.m_onDragEnd = OnPredecateDragEnd;
		dock.m_onDrop = OnPredecateDockDropped;

		return dock;
	}
	
	public void RemovePredecatesDock(NodeViewDock dock)
	{
		m_predecates.Remove( dock );
		GameObject.Destroy( dock.gameObject );
	}

	public void OnInputDockDropped(PointerEventData eventData, NodeViewDock dock)
	{
		NodeView nodeView = eventData.selectedObject.GetComponent<NodeView>();


		NodeViewDock outputDock = nodeView.AddOutputDock();
		NodeViewDock inputDock = AddInputDock();
		
		outputDock.ConnectToNode( RectTransformUtility.WorldToScreenPoint( UIController.s_UICamera, inputDock.transform.position ), inputDock);
	
		nodeView.Node.AddChild( Node );
	}

	public void OnOutputDockDropped(PointerEventData eventData, NodeViewDock dock)
	{
		NodeView nodeView = eventData.selectedObject.GetComponent<NodeView>();
		
		
		NodeViewDock outputDock = AddOutputDock();
		NodeViewDock inputDock = nodeView.AddInputDock();
		
		outputDock.ConnectToNode( RectTransformUtility.WorldToScreenPoint( UIController.s_UICamera, inputDock.transform.position ), inputDock);
	
		Node.AddChild( nodeView.Node );
	}

	public void OnQueryDockDropped(PointerEventData eventData, NodeViewDock dock)
	{
		Debug.Log("OnQueryDockDropped: " + eventData.selectedObject.name);
	}

	public void OnPredecateDockDropped(PointerEventData eventData, NodeViewDock dock)
	{
		Debug.Log("OnPredecateDockDropped: " + eventData.selectedObject.name);
	}

	public void InitializeNode( BSNode node, BlueprintSchemeView schemeView )
	{
		Node = node;
		SchemeView = schemeView;

		// initialize visuals that depends on the type

		if( (node as BSEntry) != null )
		{
			// Initialize as Entry
		}
		else if( (node as BSExit) != null )
		{
			// Initialize as Exit
		}
		else if( (node as BSAction) != null )
		{
			// Initialize as Action
		}
		else if( (node as BSEvaluate) != null )
		{
			// Initialize as Evaluate
		}
		else if( (node as BSBranch) != null )
		{
			// Initialize as Select
		}
	}
	
	#region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)
	{
		Vector2 localPos;

		RectTransformUtility.ScreenPointToLocalPointInRectangle( 
			transform.parent as RectTransform, eventData.position, UIController.s_UICamera, out localPos );

		transform.localPosition = localPos;
	}
	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{

	}

	#endregion

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{

	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{

	}

	#endregion

	public void OnInputDragBegin( PointerEventData eventData, NodeViewDock dock )
	{

	}

	public void OnOutputDragBegin(PointerEventData eventData, NodeViewDock dock)
	{
		dock.CreateCursor();

		if( dock.ConnectedNode == null )
		{
			dock.CreateLine();
		}
	}

	public void OnQueryDragBegin(PointerEventData eventData, NodeViewDock dock)
	{

	}

	public void OnPredecateDragBegin(PointerEventData eventData, NodeViewDock dock)
	{

	}



	public void OnInputDrag(PointerEventData eventData, NodeViewDock dock)
	{

	}

	public void OnOutputDrag(PointerEventData eventData, NodeViewDock dock)
	{
		Vector2 locPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle( 
		                                                        transform as RectTransform, 
		                                                        eventData.position, 
		                                                        UIController.s_UICamera, 
		                                                        out locPos );

		dock.UpdateCursor(locPos);
		dock.UpdateLine(locPos);
	}

	public void OnQueryDrag(PointerEventData eventData, NodeViewDock dock)
	{

	}

	public void OnPredecateDrag(PointerEventData eventData, NodeViewDock dock)
	{

	}




	public void OnInputDragEnd( PointerEventData eventData, NodeViewDock dock )
	{
		
	}
	
	public void OnOutputDragEnd(PointerEventData eventData, NodeViewDock dock)
	{
		dock.DestroyCursor();
		dock.DestroyLine();

		if( dock.ConnectedNode != null )
		{
			RemoveOutputDock(dock);
			dock.ConnectedNode.Node.RemoveInputDock(dock.ConnectedNode);
			dock.ConnectedNode = null;
		}
	}
	
	public void OnQueryDragEnd(PointerEventData eventData, NodeViewDock dock)
	{
		
	}
	
	public void OnPredecateDragEnd(PointerEventData eventData, NodeViewDock dock)
	{
		
	}
}
