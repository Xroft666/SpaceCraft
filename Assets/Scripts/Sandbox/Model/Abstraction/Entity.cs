﻿using System;

namespace SpaceSandbox
{
	public class Entity : Object
	{
		public float Volume { get; set; }

		public string EntityName { get; set; }

		public virtual void Initialize() {}
		public virtual void Update(){}
		
		public virtual void TakeDamage( float damage, float radius, UnityEngine.Vector2 point ) {}
		public virtual void Destroy() {}
	}
}
