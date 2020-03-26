using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public class LeaveBigOfficeCollider : MonoBehaviour
	{
		/// <summary>
		/// Is triggered when user leaves the big office.
		/// Hides the speech bubbles of all employees.
		/// </summary>
		/// <param name="collider"></param>
		private void OnTriggerEnter(Collider collider)
		{
			var avatars = GameObjectCollection.AvatarsCollection.GetEmployees();
			avatars.ForEach(a => a.HideBubble());
		}
	}
}
