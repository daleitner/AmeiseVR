using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
	public class PlayerBoard : GameObjectModelBase
	{
		private const float x = 0.0f;
		private const float z = 0.0f;
		private const float first_y = 0.85f;
		private const float diff_y = 0.45f;
		private TextMeshPro _title;
		public PlayerBoard(GameObject gameObject, string employee)
			:base(gameObject)
		{
			Name = employee;
			_title = gameObject.transform.Find("Title").GetComponent<TextMeshPro>();
			_title.text = Name;
			Tasks = new List<Task>();
		}

		public string Name { get; }

		public List<Task> Tasks { get; }

		public void AddTask(Task newTask)
		{
			newTask.SetParent(GameObject);
			newTask.MoveTo(CalculatePosition(Tasks.Count));
			newTask.Rotate(new Quaternion(0,0,0,0));
			Tasks.Add(newTask);
		}

		public void RemoveTask(Task task)
		{
			Tasks.Remove(task);
			var index = 0;
			foreach (var currentTask in Tasks)
			{
				currentTask.MoveTo(CalculatePosition(index));
				index++;
			}
		}

		private Vector3 CalculatePosition(int index)
		{
			var y = first_y - diff_y * index;
			return new Vector3(x, y, z);
		}
	}
}
