using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{

	public class TaskBoard : GameObjectModelBase
	{
		private const float first_x = -0.95f;
		private const float diff_x = 0.55f;
		private const float first_y = 0.85f;
		private const float diff_y = 0.45f;
		private const float z = 0.0f;
		private const int columns = 5;

		public TaskBoard(GameObject taskBoard)
			:base(taskBoard)
		{
			Tasks = new List<Task>();
		}

		public List<Task> Tasks { get; }

		public void AddTask(Task task)
		{
			task.SetParent(GameObject);
			var position = CalculatePosition(Tasks.Count);
			task.MoveTo(position);
			task.Rotate(new Quaternion(0,0,0,0));
			Tasks.Add(task);
		}

		private Vector3 CalculatePosition(int index)
		{
			var currentRow = index / columns;
			var currentColumn = index % columns;

			var current_x = first_x;
			for (int i = 0; i < currentColumn; i++)
			{
				current_x += diff_x;
			}

			var current_y = first_y;
			for (int i = 0; i < currentRow; i++)
			{
				current_y -= diff_y;
			}

			return new Vector3(current_x, current_y, z);
		}
	}
}
