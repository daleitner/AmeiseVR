using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class AvatarsCollection
	{
		private readonly GameObject _avatarsContainer;
		private readonly MessageListener _listener;
		private readonly List<Avatar> _avatars;

		public AvatarsCollection(GameObject avatarsContainer, MessageListener listener)
		{
			_avatars = new List<Avatar>();
			_avatarsContainer = avatarsContainer;
			_listener = listener;
		}

		public Avatar AddAvatar(GameObject avatarObject)
		{
			if (_avatars.Count >= MaxAvatars)
				throw new Exception("max count of avatars reached!");

			avatarObject.transform.parent = _avatarsContainer.transform;
			avatarObject.transform.localPosition = OfficePlaces[_avatars.Count];
			var avatar = new Avatar(avatarObject, _listener); 
			_avatars.Add(avatar);
			avatarObject.SetActive(true);
			return avatar;
		}

		public static int MaxAvatars => OfficePlaces.Length;

		private static Vector3[] OfficePlaces =
		{
			new Vector3(-18.44f, YOffset, -4.603f),
			new Vector3(-18.44f, YOffset, -7.088f),
			new Vector3(-18.44f, YOffset, -9.621f),
			new Vector3(-18.44f, YOffset, -19.667f),
			new Vector3(-18.44f, YOffset, -22.140f),
			new Vector3(-18.44f, YOffset, -24.64f),
			new Vector3(-21.732f, YOffset, -4.151f),
			new Vector3(-21.732f, YOffset, -6.635f),
			new Vector3(-21.732f, YOffset, -9.107f),
			new Vector3(-21.732f, YOffset, -19.179f),
			new Vector3(-21.732f, YOffset, -21.696f),
			new Vector3(-21.732f, YOffset, -24.164f),
			new Vector3(-26.759f, YOffset, -4.151f),
			new Vector3(-26.759f, YOffset, -6.635f),
			new Vector3(-26.759f, YOffset, -9.107f),
			new Vector3(-26.759f, YOffset, -19.179f),
			new Vector3(-26.759f, YOffset, -21.696f),
			new Vector3(-26.759f, YOffset, -24.164f),
			new Vector3(22.5f, YOffset, -6.25f),
			new Vector3(22.5f, YOffset, -9.1f)
		};

		private static float YOffset = 0.34f;

		public Avatar Get(GameObject gameObject)
		{
			return _avatars.SingleOrDefault(a => a.GameObject == gameObject);
		}

		public List<Avatar> GetEmployees()
		{
			return _avatars.Where(x => !x.IsDummy).ToList();
		}

		public List<Avatar> GetSecretaries()
		{
			return _avatars.Where(x => x.IsSecretary).ToList();
		}
	}
}
