
using System.Collections.Generic;


namespace SpaceSandbox
{
	public class Container : Entity
	{
		public System.Action onDestroy;

		public ContainerView View { get; set; }

		public virtual void InitializeView() {}
		public virtual void UpdateView() {}
		public virtual void LateUpdate() {}

		public virtual void OnDrawGizmos(){}

		public override void Destroy()
		{
			View.gameObject.SetActive(false);

			if( onDestroy != null )
				onDestroy();
		}
	}
}