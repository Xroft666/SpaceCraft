using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	void IBeginDragHandler.OnBeginDrag (PointerEventData eventData)
	{

	}

	void IDragHandler.OnDrag (PointerEventData eventData)
	{

	}

	void IEndDragHandler.OnEndDrag (PointerEventData eventData)
	{

	}

	void IDropHandler.OnDrop(PointerEventData eventData)
	{

	}
}
