using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void OnScrollViewHandler(PointerEventData data, DraggableItemsScrollRect scrollView);

public class DraggableItemsScrollRect : ScrollRect, IDropHandler
{
	public OnScrollViewHandler m_onDrop;


	private static GameObject s_draggableObject = null;

	public override void OnBeginDrag (PointerEventData data) 
	{
		s_draggableObject = data.selectedObject;

		s_draggableObject.transform.parent = UIController.s_Canvas.transform;

		CanvasGroup canvasGr = s_draggableObject.GetComponent<CanvasGroup>();
		canvasGr.blocksRaycasts = false;

		SortItems();
	}

	public override void OnDrag (PointerEventData data) 
	{
		s_draggableObject.transform.localPosition = (data.position - new Vector2(Screen.width, Screen.height) * 0.5f);
	}

	public override void OnEndDrag (PointerEventData data) 
	{
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
		s_draggableObject.transform.SetParent( content );

		CanvasGroup canvasGr = s_draggableObject.GetComponent<CanvasGroup>();
		canvasGr.blocksRaycasts = true;

		s_draggableObject = null;

		SortItems();

		if( m_onDrop != null )
			m_onDrop(eventData, this);
	}

	#endregion

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
