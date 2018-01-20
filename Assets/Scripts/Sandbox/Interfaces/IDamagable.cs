using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
	void TakeDamage (float damage, float radius, UnityEngine.Vector2 point);
}
