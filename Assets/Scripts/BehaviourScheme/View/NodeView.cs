using UnityEngine;
using System.Collections.Generic;

using BehaviourScheme;

using UnityEngine.EventSystems;

// the visual representation of a node
// it s a monobehaviour, so it contains all the callbacks, visuals and references 
public class NodeView : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	// blueprint position
	public float x;
	public float y;

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

	private NodeViewDock CreateDock( string name, Color color, Vector3 position, OnDropDelegate deleg)
	{
		GameObject inputGo = new GameObject(name, typeof(RectTransform) );
		RectTransform m_inputDockTr = inputGo.transform as RectTransform;	
		m_inputDockTr.transform.SetParent( transform, false );
		
		m_inputDockTr.sizeDelta = Vector2.one * 10;
		
		m_inputDockTr.transform.localPosition = position;
		
		NodeViewDock inputDock = inputGo.AddComponent<NodeViewDock>();
		inputDock.AssignNode( this, deleg, color );

		return inputDock;
	}

	public NodeViewDock AddInputDock()
	{
		Vector3 position = Vector3.up * 10f + Vector3.right * 15f * m_inputs.Count;
		NodeViewDock dock = CreateDock("input", Color.red, position, OnInputDockDropped);
		m_inputs.Add(dock);

		return dock;
	}

	public void RemoveInputDock()
	{

	}

	public NodeViewDock AddOutputDock()
	{
		Vector3 position = -Vector3.up * 10f + Vector3.right * 15f * m_outputs.Count;
		NodeViewDock dock = CreateDock("output" + m_outputs.Count, Color.red, position, OnOutputDockDropped);
		m_outputs.Add(dock);

		return dock;
	}
	
	public void RemoveOutputDock()
	{
		
	}

	public NodeViewDock AddQueryDock()
	{
		Vector3 position = Vector3.right * 50f + Vector3.up * 15f * m_queries.Count;
		NodeViewDock dock = CreateDock("query" + m_queries.Count, Color.blue, position, OnQueryDockDropped);
		m_queries.Add(dock);

		return dock;
	}
	
	public void RemoveQueryDock()
	{
		
	}

	public NodeViewDock AddPredecatesDock()
	{
		Vector3 position = -Vector3.right * 50f - Vector3.up * 15f * m_predecates.Count;
		NodeViewDock dock = CreateDock("query" + m_predecates.Count, Color.green, position, OnQueryDockDropped);
		m_predecates.Add(dock);

		return dock;
	}
	
	public void RemovePredecatesDock()
	{
		
	}

	public void OnInputDockDropped(PointerEventData eventData)
	{
		Debug.Log("OnInputDockDropped: " + eventData.selectedObject.name);
	}

	public void OnOutputDockDropped(PointerEventData eventData)
	{
		Debug.Log("OnOutputDockDropped: " + eventData.selectedObject.name);
	}

	public void OnQueryDockDropped(PointerEventData eventData)
	{
		Debug.Log("OnQueryDockDropped: " + eventData.selectedObject.name);
	}

	public void OnPredecateDockDropped(PointerEventData eventData)
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

	public void OnGUI()
	{
		//BSMultiChildNode multiChild = Node as BSMultiChildNode;
		//if( multiChild != null )
		//{
		//	foreach( BSNode child in multiChild.m_children )
		//		Debug.DrawLine( transform.position, child.m_view.transform.position, Color.white, Time.deltaTime );
		//}
		//else
		//{
		//	if( Node.m_connectNode != null )
		//	Debug.DrawLine( transform.position, Node.m_connectNode.m_view.transform.position, Color.white, Time.deltaTime );
		//}
	}
}
