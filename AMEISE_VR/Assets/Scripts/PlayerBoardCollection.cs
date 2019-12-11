using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public class PlayerBoardCollection : GameObjectModelBase
	{
		private const float first_x = 2.6f;
		private const float diff_x = 0.8f;
		private const float y = 1.77f;
		private const float z = 0.0f;
		public PlayerBoardCollection(GameObject whiteBoard)
			: base(whiteBoard)
		{
			PlayerBoards = new List<PlayerBoard>();
		}

		public List<PlayerBoard> PlayerBoards { get; }

		public void AddPlayerBoard(PlayerBoard newPlayerBoard)
		{
			newPlayerBoard.SetParent(GameObject);
			var vector = CalculateNextPosition();
			newPlayerBoard.MoveTo(vector);
			newPlayerBoard.Rotate(new Quaternion(0,0,0,0));
			PlayerBoards.Add(newPlayerBoard);
		}

		private Vector3 CalculateNextPosition()
		{
			var x = first_x + diff_x * PlayerBoards.Count;
			return new Vector3(x, y, z);
		}
	}
}
