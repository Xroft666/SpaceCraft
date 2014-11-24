using UnityEngine;
using System.Collections;

public class GotoTarget : MonoBehaviour 
{
	private static GotoTarget _instance;

	void Awake()
	{
		_instance = this;
	}

	public static Vector3 Position
	{
		get 
		{
			return _instance.transform.position;
		}
	}
}
