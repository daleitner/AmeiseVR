﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class AvatarsCollection
	{
		private GameObject _avatarsContainer;
		private List<Avatar> _avatars;

		public AvatarsCollection(GameObject avatarsContainer)
		{
			_avatars = new List<Avatar>();
			_avatarsContainer = avatarsContainer;
		}

		public void AddAvatar(GameObject avatar)
		{
			if (_avatars.Count >= MaxAvatars)
				throw new Exception("max count of avatars reached!");
			avatar.transform.parent = _avatarsContainer.transform;
			avatar.transform.localPosition = OfficePlaces[_avatars.Count];
			_avatars.Add(new Avatar(avatar));
			avatar.SetActive(true);
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
			new Vector3(-26.759f, YOffset, -24.164f)
		};

		private static float YOffset = 0.34f;
	}
}
