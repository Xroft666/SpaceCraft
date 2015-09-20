using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour, IScrollHandler
{
	void IScrollHandler.OnScroll (PointerEventData eventData)
	{
		Debug.Log("OnScroll");
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + eventData.scrollDelta.y, 1f, 10f);
	}

	private void Update()
	{
		if( EventSystem.current.currentSelectedGameObject == null )
		{
			Vector3 input = Vector3.zero; 
			if( Input.GetKey(KeyCode.UpArrow) )
				input += Vector3.forward;
			
			if( Input.GetKey(KeyCode.DownArrow) )
				input += Vector3.back;
			
			if( Input.GetKey(KeyCode.LeftArrow) )
				input += Vector3.left;
			
			if( Input.GetKey(KeyCode.RightArrow) )
				input = Vector3.right;
			
			Camera.main.transform.position += input * Time.deltaTime;
		}
	}
}
