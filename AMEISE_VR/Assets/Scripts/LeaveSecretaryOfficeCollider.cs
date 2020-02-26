using UnityEngine;

namespace Assets.Scripts
{
	public class LeaveSecretaryOfficeCollider : MonoBehaviour
	{
		private void OnTriggerEnter(Collider collider)
		{
			var avatars = GameObjectCollection.AvatarsCollection.GetSecretaries();
			avatars.ForEach(a => a.HideBubble());
		}
	}
}
