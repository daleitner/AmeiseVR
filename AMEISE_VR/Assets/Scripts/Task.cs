using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
	public class Task : GameObjectModelBase
	{
		private static readonly Color DefaultColor = new Color(0.25f, 0.5f, 1.0f);
		private static readonly Color SelectedColor = new Color(0.25f, 0.8f, 1.0f);
		private static readonly Color ErrorColor = new Color(1.0f, 0.16f, 0.13f);
		private static readonly Color SuccessColor = new Color(0.16f, 1.0f, 0.22f);
		private static readonly Color IncompleteColor = new Color(1.0f, 0.875f, 0.25f);
		private readonly TextMeshPro _text;
		private readonly Material _material;
		private int _currentParameterIndex = -1;
		private List<string> _variableParameterValues;
		public Task(GameObject task, Command command)
			:base(task)
		{
			Command = command;
			var variableParameterType = Command.Parameters.Select(p => p.Type).FirstOrDefault(t => t != KnowledgeBase.EmployeeType);
			if (!string.IsNullOrEmpty(variableParameterType))
			{
				_variableParameterValues = KnowledgeBase.Instance.GetValuesOfParameterType(variableParameterType);
				_currentParameterIndex = 0;
			}

			_text = task.transform.Find("Text").GetComponent<TextMeshPro>();
			SetText();

			_material = task.transform.Find("Paper").GetComponent<Renderer>().material;
			IsSelected = false;
		}
		
		public Command Command { get; }

		public bool IsSent { get; private set; }
		public bool IsIncomplete { get; private set; }

		public bool IsSelected { get; private set; }

		public void ChangeParameter()
		{
			if (_variableParameterValues == null)
				return;

			_currentParameterIndex++;
			if (_currentParameterIndex == _variableParameterValues.Count)
				_currentParameterIndex = 0;
			SetText();
		}

		public void SetSelection(bool selected)
		{
			if (IsSent)
				return;
			IsSelected = selected;
			_material.color = selected ? SelectedColor : DefaultColor;
		}

		public Task Clone(GameObject gameObject)
		{
			var task = new Task(gameObject, Command);
			task._currentParameterIndex = _currentParameterIndex;
			task.SetText();
			return task;
		}

		public void TaskSentSuccessful()
		{
			IsSent = true;
			_material.color = SuccessColor;
		}

		public void TaskSentError()
		{
			IsSent = true;
			_material.color = ErrorColor;
		}

		public void SetIncomplete()
		{
			IsIncomplete = true;
			_material.color = IncompleteColor;
		}

		public void SetComplete()
		{
			IsIncomplete = false;
			_material.color = DefaultColor;
		}

		private void SetText()
		{
			_text.text = Command.Name;
			if (_variableParameterValues != null)
				_text.text += " " + _variableParameterValues[_currentParameterIndex];
		}
	}
}
