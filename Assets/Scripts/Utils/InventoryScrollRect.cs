using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryScrollRect : ScrollRect, IDropHandler
{
	private static GameObject s_draggableObject = null;

	private Transform rootTransform;

	public void Awake()
	{
		rootTransform = GetComponentInParent<Canvas>().transform;
	}

	public override void OnBeginDrag (PointerEventData data) 
	{
		s_draggableObject = data.selectedObject;

		s_draggableObject.transform.parent = rootTransform;

		CanvasGroup canvasGr = s_draggableObject.GetComponent<CanvasGroup>();
		canvasGr.blocksRaycasts = false;
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
