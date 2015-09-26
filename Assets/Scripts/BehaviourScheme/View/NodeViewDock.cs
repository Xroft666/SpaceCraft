using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void OnDropDelegate(PointerEventData eventData);

public class NodeViewDock : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private GameObject s_currentlyDragged;


	public OnDropDelegate onDrop;

	public NodeView Node { get; private set; }
	public NodeViewDock ConnectedNode { get; private set; }

	private Image m_image;

	private GameObject m_line;
	private RectTransform m_lineRect;

	private void Awake()
	{
		m_image = gameObject.AddComponent<Image>();
	}

	private void Update()
	{
		if( ConnectedNode != null )
			UpdateLine( RectTransformUtility.WorldToScreenPoint( UIController.s_UICamera, ConnectedNode.transform.position));
	}

	public void AssignNode( NodeView view, OnDropDelegate deleg, Color color )
	{
		Node = view;
		onDrop += deleg;

		m_image.color = color;
	}

	public void ConnectToNode( Vector3 toPos, NodeViewDock other = null )	// in screen space
	{
		ConnectedNode = other;

		m_line = new GameObject("line", typeof(RectTransform) );
		
		m_lineRect = m_line.transform as RectTransform;
		m_lineRect.transform.SetParent( transform, false );
		m_lineRect.sizeDelta = Vector2.one;
		m_lineRect.pivot = Vector2.up * 0.5f;
		
		Image lineimg = m_line.AddComponent<Image>();
		lineimg.raycastTarget = false;


		UpdateLine(toPos);
	}

	private void UpdateLine(Vector3 toPos)
	{
		Vector2 locPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle( 
		                                                        transform as RectTransform, 
		                                                        toPos, 
		                                                        UIController.s_UICamera, 
		                                                        out locPos );
		
		m_lineRect.sizeDelta = new Vector2(locPos.magnitude, 1f);
		m_lineRect.localRotation = Quaternion.LookRotation(-locPos.normalized);
		m_lineRect.Rotate(0f, 90f, 0f, Space.Self);
	}



	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if( onDrop != null )
			onDrop(eventData);
	}
	#endregion

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		s_currentlyDragged = new GameObject("draggable", typeof(RectTransform) );
		
		RectTransform m_draggablekTr = s_currentlyDragged.transform as RectTransform;
		m_draggablekTr.transform.SetParent( transform, false );
		m_draggablekTr.sizeDelta = Vector2.one * 10;

		Image dragImg = s_currentlyDragged.AddComponent<Image>();
		dragImg.raycastTarget = false;


		ConnectToNode (RectTransformUtility.WorldToScreenPoint( UIController.s_UICamera, transform.position));
	}

	#endregion
	
	#region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)
	{
		Vector2 locPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle( 
		                                                        transform as RectTransform, 
		                                                        eventData.position, 
		                                                        UIController.s_UICamera, 
		                                                        out locPos );
		s_currentlyDragged.transform.localPosition = locPos;

		m_lineRect.sizeDelta = new Vector2(locPos.magnitude, 1f);
		m_lineRect.localRotation = Quaternion.LookRotation(-s_currentlyDragged.transform.localPosition.normalized);
		m_lineRect.Rotate(0f, 90f, 0f, Space.Self);
	}
	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		GameObject.Destroy(s_currentlyDragged);
		s_currentlyDragged = null;

		GameObject.Destroy(m_line);
		m_line = null;
	}

	#endregion
}
