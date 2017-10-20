using UnityEngine;
using System.Collections.Generic;

using BehaviourScheme;
using SpaceSandbox;

using UnityEngine.EventSystems;


public delegate void NodeViewClick( NodeView view );

// the visual representation of a node
// it s a monobehaviour, so it contains all the callbacks, visuals and references 
public class NodeView : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
//	[HideInInspector]
	public Vector2 globalOffset;

	public BSNode Node { get; private set; }
	public BlueprintSchemeView SchemeView { get; private set; }
	

	public List<NodeViewDock> m_inputs = new List<NodeViewDock>();
	public List<NodeViewDock> m_outputs = new List<NodeViewDock>();
	public List<NodeViewDock> m_queries = new List<NodeViewDock>();
	public List<NodeViewDock> m_predecates = new List<NodeViewDock>();

	public NodeViewClick onNodeDoubleClick;

	public void InitializeNode( BSNode node )
	{
		Node = node;

		BSAction action = node as BSAction;
		BSQuery query = node as BSQuery;
		BSEntry entry = node as BSEntry;
		BSCheck check = node as BSCheck;
		BSForeach foreac = node as BSForeach;
		BSExit exit = node as BSExit;
		BSEvaluate eval = node as BSEvaluate;
		BSSequence seq = node as BSSequence;
		BSSelect sel = node as BSSelect;


		NodeViewDock inputDock;
		NodeViewDock outputDock;
		NodeViewDock queryDock;
		NodeViewDock checkDock;


		if( action != null )
		{
			inputDock = AddInputDock();
			inputDock.m_onDrop = OnInputDockDropped;

			outputDock = AddOutputDock();
			outputDock.m_onDrag = OnOutputDrag;
			outputDock.m_onDragBegin = OnOutputDragBegin;
			outputDock.m_onDragEnd = OnOutputDragEnd;


			queryDock = AddQueryDock();
			queryDock.m_onDrop = OnQueryDockDropped;
		} 

		if( query != null )
		{
			queryDock = AddQueryDock();
			queryDock.m_onDrag = OnQueryDrag;
			queryDock.m_onDragBegin = OnQueryDragBegin;
			queryDock.m_onDragEnd = OnQueryDragEnd;
		} 

		if( entry != null )
		{
			outputDock = AddOutputDock();
			outputDock.m_onDrag = OnOutputDrag;
			outputDock.m_onDragBegin = OnOutputDragBegin;
			outputDock.m_onDragEnd = OnOutputDragEnd;
		} 

		if( check != null )
		{
			checkDock = AddPredecatesDock();
			checkDock.m_onDrag = OnPredecateDrag;
			checkDock.m_onDragBegin = OnPredecateDragBegin;
			checkDock.m_onDragEnd = OnPredecateDragEnd;
		} 

		if( foreac != null )
		{

		}

		if( exit != null )
		{
			inputDock = AddInputDock();
			inputDock.m_onDrop = OnInputDockDropped;
		}

		if( eval != null )
		{
			inputDock = AddInputDock();
			inputDock.m_onDrop = OnInputDockDropped;
			
			outputDock = AddOutputDock();
			outputDock.m_onDrag = OnOutputDrag;
			outputDock.m_onDragBegin = OnOutputDragBegin;
			outputDock.m_onDragEnd = OnOutputDragEnd;
		}

		if( seq != null )
		{
			inputDock = AddInputDock();
			inputDock.m_onDrop = OnInputDockDropped;

			outputDock = AddOutputDock();
			outputDock.m_onDrag = OnOutputDrag;
			outputDock.m_onDragBegin = OnOutputDragBegin;
			outputDock.m_onDragEnd = OnOutputDragEnd;
		}

		if( sel != null )
		{
			inputDock = AddInputDock();
			inputDock.m_onDrop = OnInputDockDropped;

			outputDock = AddOutputDock();
			outputDock.m_onDrag = OnOutputDrag;
			outputDock.m_onDragBegin = OnOutputDragBegin;
			outputDock.m_onDragEnd = OnOutputDragEnd;

			checkDock = AddPredecatesDock();
			checkDock.m_onDrop = OnPredecateDockDropped;
		}
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

	public void OnQueryDockDropped(PointerEventData eventData, NodeViewDock dock)
	{
		NodeView nodeView = eventData.selectedObject.GetComponent<NodeView>();

		NodeViewDock outputDock = nodeView.AddQueryDock();
		NodeViewDock inputDock = AddQueryDock();

		outputDock.ConnectToNode( RectTransformUtility.WorldToScreenPoint( UIController.s_UICamera, inputDock.transform.position ), inputDock);
	
		BSQuery query = nodeView.Node as BSQuery;
		BSAction action = Node as BSAction;

		action.ConnectToQuery( query );
	}

	public void OnPredecateDockDropped(PointerEventData eventData, NodeViewDock dock)
	{
		NodeView nodeView = eventData.selectedObject.GetComponent<NodeView>();
		
		NodeViewDock inputDock = nodeView.AddPredecatesDock();
		NodeViewDock outputDock = AddPredecatesDock();
		
		outputDock.ConnectToNode( RectTransformUtility.WorldToScreenPoint( UIController.s_UICamera, inputDock.transform.position ), inputDock);
		
		BSCheck check = nodeView.Node as BSCheck;
		BSSelect select = Node as BSSelect;

		select.AddCondition( check.m_device, check.m_checkName );
		//check.connectedNode = select;
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
		if( eventData.clickCount == 2 )
		{
			if( onNodeDoubleClick != null )
				onNodeDoubleClick( this );
		}
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


	private void DragBegin( NodeViewDock dock )
	{
		dock.CreateCursor();
		
		if( dock.ConnectedNode == null )
		{
			dock.CreateLine();
		}
	}

	private void Dragging(PointerEventData eventData, NodeViewDock dock )
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

	private void DragEnd( NodeViewDock dock )
	{
		dock.DestroyCursor();
		dock.DestroyLine();
	}
	

	public void OnOutputDragBegin(PointerEventData eventData, NodeViewDock dock)
	{
		DragBegin( dock );
	}

	public void OnQueryDragBegin(PointerEventData eventData, NodeViewDock dock)
	{
		DragBegin( dock );
	}

	public void OnPredecateDragBegin(PointerEventData eventData, NodeViewDock dock)
	{
		DragBegin( dock );
	}
	
	public void OnOutputDrag(PointerEventData eventData, NodeViewDock dock)
	{
		Dragging( eventData, dock );
	}

	public void OnQueryDrag(PointerEventData eventData, NodeViewDock dock)
	{
		Dragging( eventData, dock );
	}

	public void OnPredecateDrag(PointerEventData eventData, NodeViewDock dock)
	{
		Dragging( eventData, dock );
	}

	public void OnOutputDragEnd(PointerEventData eventData, NodeViewDock dock)
	{
		DragEnd( dock );

		if( dock.ConnectedNode != null )
		{
			RemoveOutputDock(dock);
			dock.ConnectedNode.Node.RemoveInputDock(dock.ConnectedNode);
			dock.ConnectedNode = null;
		}
	}
	
	public void OnQueryDragEnd(PointerEventData eventData, NodeViewDock dock)
	{
		DragEnd( dock );
	}
	
	public void OnPredecateDragEnd(PointerEventData eventData, NodeViewDock dock)
	{
		DragEnd( dock );
	}
}
