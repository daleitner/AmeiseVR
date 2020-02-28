using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public class TransformObject
	{
		public TransformObject(Vector3 position, Quaternion rotation)
		{
			Position = position;
			Rotation = rotation;
		}
		public Vector3 Position { get; }
		public Quaternion Rotation { get; }
	}
}
