using UnityEngine;

namespace Assets.Scripts
{
	public class Avatar : GameObjectModelBase
	{
		public Avatar(GameObject avatar, string employee)
			:base(avatar)
		{
			Name = employee;
		}

		public string Name { get; }

		public bool IsDummy => string.IsNullOrEmpty(Name);
	}
}
