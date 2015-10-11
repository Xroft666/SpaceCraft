using UnityEngine;
using System.Collections;

using BehaviourScheme;
using SpaceSandbox;

public class DraggableItem : MonoBehaviour 
{
	public enum ControlType
	{
		Selection,
		Sequence, 
		Evaluation,
		Foreach,
		Entry
	}

	public enum QueryType
	{
		Query,
		Check
	}

	public enum DeviceActionType
	{
		Entry, 
		Action
	}

	public Entity EntityContainment { get; set; }
	public Device DeviceContainment { get; set; }
	public string MethodName { get; set; }
	
	public ControlType controlType { get; set; }
	public QueryType queryType { get; set; }
	public DeviceActionType eventType { get; set; }
}
