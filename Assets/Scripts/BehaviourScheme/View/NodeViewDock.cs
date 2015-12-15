using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void OnHandlerDelegate(PointerEventData eventData, NodeViewDock dock);

public class NodeViewDock : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private GameObject s_currentlyDragged;


	public OnHandlerDelegate m_onDrop;
	public OnHandlerDelegate m_onDragBegin;
	public OnHandlerDelegate m_onDragEnd;
	public OnHandlerDelegate m_onDrag;


	public NodeView Node { get; private set; }
	public NodeViewDock ConnectedNode { get; set; }

	private Image m_image;

	private GameObject m_line;
	private RectTransform m_lineRect;

	private void Awake()
	{
		m_image = gameObject.AddComponent<Image>();
		m_image.sprite = UIController.Instance.m_spriteTexture;
	}

	private void Update()
	{
		if( m_line != null && ConnectedNode != null)
		{
			UpdateLine( transform.InverseTransformPoint(ConnectedNode.transform.position) );
		}
	}

	public void AssignNode( NodeView view, Color color, OnHandlerDelegate onDrop = null, 
	                       								OnHandlerDelegate onDragBegin = null, 
	                       								OnHandlerDelegate onDragEnd = null)
	{
		Node = view;
		m_image.color = color;

		m_onDrop += onDrop;
		m_onDragBegin += onDragBegin;
		m_onDragEnd += onDragEnd;
	}

	public void ConnectToNode( Vector3 toPos, NodeViewDock other = null )	// in screen space
	{
		ConnectedNode = other;
	//	other.ConnectedNode = this;

		CreateLine();
	}

	public void CreateCursor()
	{
		s_currentlyDragged = new GameObject("draggable", typeof(RectTransform) );
		
		RectTransform m_draggablekTr = s_currentlyDragged.transform as RectTransform;
		m_draggablekTr.transform.SetParent( transform, false );
		m_draggablekTr.sizeDelta = Vector2.one * 10;
		
		Image dragImg = s_currentlyDragged.AddComponent<Image>();
		dragImg.sprite = UIController.Instance.m_spriteTexture;
		dragImg.raycastTarget = false;
	}

	public void CreateLine()
	{
		m_line = new GameObject("line", typeof(RectTransform) );
		
		m_lineRect = m_line.transform as RectTransform;
		m_lineRect.transform.SetParent( transform, false );
		m_lineRect.sizeDelta = Vector2.one;
		m_lineRect.pivot = Vector2.up * 0.5f;
		
		Image lineimg = m_line.AddComponent<Image>();
		lineimg.sprite = UIController.Instance.m_spriteTexture;
		lineimg.raycastTarget = false;
	}

	public void UpdateCursor(Vector3 locPos)
	{
		s_currentlyDragged.transform.localPosition = locPos;
	}

	public void UpdateLine(Vector3 locPos)
	{
		m_lineRect.sizeDelta = new Vector2(locPos.magnitude, 1f);
		m_lineRect.localRotation = Quaternion.LookRotation(-locPos.normalized);
		m_lineRect.Rotate(0f, 90f, 0f, Space.Self);
	}

	public void DestroyCursor()
	{
		GameObject.Destroy(s_currentlyDragged);
		s_currentlyDragged = null;
	}

	public void DestroyLine()
	{
		GameObject.Destroy(m_line);
		m_line = null;
	}


	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if( m_onDrop != null )
			m_onDrop(eventData, this);
	}
	#endregion

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		if( m_onDragBegin != null )
			m_onDragBegin(eventData, this);
	}

	#endregion
	
	#region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)
	{
		if( m_onDrag != null )
			m_onDrag(eventData, this);
	}
	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		if( m_onDragEnd != null )
			m_onDragEnd(eventData, this);
	}

	#endregion
}
