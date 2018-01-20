
using System.Collections.Generic;


namespace SpaceSandbox
{
	public class Container : Entity, IDestroyable
	{
		public System.Action onDestroy;

		public ContainerView View { get; set; }

		public virtual void OnDrawGizmos(){}

		public virtual void Destroy()
		{
			View.gameObject.SetActive(false);

			if( onDestroy != null )
				onDestroy();
		}
	}
}