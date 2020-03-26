using UnityEngine;

namespace Assets.Scripts
{
	public class LeaveSecretaryOfficeCollider : MonoBehaviour
	{
		/// <summary>
		/// Is triggered when user leaves the secretary.
		/// Hides the speech bubbles of the secretary avatars.
		/// </summary>
		/// <param name="collider"></param>
		private void OnTriggerEnter(Collider collider)
		{
			var avatars = GameObjectCollection.AvatarsCollection.GetSecretaries();
			avatars.ForEach(a => a.HideBubble());
		}
	}
}
