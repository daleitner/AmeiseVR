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
			avatarObject.transform.localPosition = OfficePlaces[_avatars.Count].Position;
			avatarObject.transform.localRotation = OfficePlaces[_avatars.Count].Rotation;

		var avatar = new Avatar(avatarObject, _listener); 
			_avatars.Add(avatar);
			avatarObject.SetActive(true);
			return avatar;
		}

		public static int MaxAvatars => OfficePlaces.Length;
		private static float YOffset = -0.65f;

		private static TransformObject[] OfficePlaces =
		{
			new TransformObject(new Vector3(-18.86f, YOffset, -4.603f), Quaternion.Euler(0.0f, -90.0f, 0.0f)),
			new TransformObject(new Vector3(-18.86f, YOffset, -7.088f), Quaternion.Euler(0.0f, -90.0f, 0.0f)),
			new TransformObject(new Vector3(-18.86f, YOffset, -9.621f), Quaternion.Euler(0.0f, -90.0f, 0.0f)),
			new TransformObject(new Vector3(-18.86f, YOffset, -19.667f), Quaternion.Euler(0.0f, -90.0f, 0.0f)),
			new TransformObject(new Vector3(-18.86f, YOffset, -22.140f), Quaternion.Euler(0.0f, -90.0f, 0.0f)),
			new TransformObject(new Vector3(-18.86f, YOffset, -24.64f), Quaternion.Euler(0.0f, -90.0f, 0.0f)),
			new TransformObject(new Vector3(-21.3f, YOffset, -4.151f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-21.3f, YOffset, -6.635f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-21.3f, YOffset, -9.107f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-21.3f, YOffset, -19.179f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-21.3f, YOffset, -21.696f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-21.3f, YOffset, -24.164f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-26.327f, YOffset, -4.151f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-26.327f, YOffset, -6.635f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-26.327f, YOffset, -9.107f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-26.327f, YOffset, -19.179f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-26.327f, YOffset, -21.696f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(-26.327f, YOffset, -24.164f), Quaternion.Euler(0.0f, 90.0f, 0.0f)),
			new TransformObject(new Vector3(21.931f, -0.86f, -6.25f), Quaternion.Euler(0.0f, -90.0f, 0.0f)),
			new TransformObject(new Vector3(21.931f, -0.86f, -9.1f), Quaternion.Euler(0.0f, -90.0f, 0.0f))
		};
		
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
