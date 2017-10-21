using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceSandbox;

[CreateAssetMenu(fileName = "BlueprintItem.asset", menuName = "Data/Blueprint")]
public class BlueprintScriptableObject : ScriptableObject 
{
	public BlueprintScheme m_scheme;
}
