using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	/// <summary>
	/// Base Class for GameObject Model Classes
	/// </summary>
	public class GameObjectModelBase
	{
		public GameObjectModelBase(GameObject gameObject)
		{
			GameObject = gameObject;
		}

		public GameObject GameObject { get; }

		public virtual void SetParent(GameObject parent)
		{
			GameObject.transform.parent = parent.transform;
		}

		public virtual void MoveTo(Vector3 position)
		{
			GameObject.transform.localPosition = position;
		}

		public virtual void Rotate(Quaternion rotation)
		{
			GameObject.transform.localRotation = rotation;
		}

		public virtual void Scale(Vector3 scale)
		{
			GameObject.transform.localScale = scale;
		}

		public virtual void Show()
		{
			GameObject.SetActive(true);
		}

		public virtual void Hide()
		{
			GameObject.SetActive(false);
		}
	}
}
