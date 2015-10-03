using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void OnScrollViewHandler(PointerEventData data, DraggableItemsScrollRect scrollView);

public class DraggableItemsScrollRect : ScrollRect, IDropHandler
{
	public Dictionary<DraggableItemsScrollRect, OnScrollViewHandler> m_onDropHandlers = new Dictionary<DraggableItemsScrollRect, OnScrollViewHandler>();


	public static GameObject s_draggableObject = null;
	public static DraggableItemsScrollRect s_ViewDraggedFrom = null;


	public override void OnBeginDrag (PointerEventData data) 
	{
		if( data.selectedObject == null )
			return;

		s_draggableObject = data.selectedObject;
		s_ViewDraggedFrom = this;


		s_draggableObject.transform.parent = UIController.s_Canvas.transform;

		CanvasGroup canvasGr = s_draggableObject.GetComponent<CanvasGroup>();
		canvasGr.blocksRaycasts = false;

		SortItems();
	}

	public override void OnDrag (PointerEventData data) 
	{
		if( s_draggableObject == null )
			return;

		s_draggableObject.transform.localPosition = (data.position - new Vector2(Screen.width, Screen.height) * 0.5f);
	}

	public override void OnEndDrag (PointerEventData data) 
	{
		s_ViewDraggedFrom = null;

		if( s_draggableObject == null )
			return;

		s_draggableObject.transform.SetParent( content );

		CanvasGroup canvasGr = s_draggableObject.GetComponent<CanvasGroup>();
		canvasGr.blocksRaycasts = true;

		SortItems();
	}

	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
		if( eventData.selectedObject == null )
			return;
		if( !m_onDropHandlers.ContainsKey( s_ViewDraggedFrom ) )
			return;

		OnScrollViewHandler onDrop = m_onDropHandlers[s_ViewDraggedFrom];

		if( onDrop != null )
			onDrop(eventData, this);
	}

	#endregion


	public void AssignObject( GameObject droppedObject )
	{
		s_draggableObject.transform.SetParent( content );
		
		CanvasGroup canvasGr = s_draggableObject.GetComponent<CanvasGroup>();
		canvasGr.blocksRaycasts = true;

		s_draggableObject = null;

		SortItems();
	}

	private void SortItems()
	{
		int childNum = content.childCount;
		int count = -childNum / 2;

		foreach( Transform child in content )
		{
			child.transform.localPosition = Vector3.up * count * 20f;

			count++;
		}
		
	}
}
