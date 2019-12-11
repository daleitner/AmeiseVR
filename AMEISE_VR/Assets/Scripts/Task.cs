using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
	public class Task : GameObjectModelBase
	{
		private static readonly Color DefaultColor = new Color(1.0f, 0.16f, 0.13f);
		private static readonly Color SelectedColor = new Color(0.16f, 1.0f, 0.22f);
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

		private void SetText()
		{
			_text.text = Command.Name;
			if (_variableParameterValues != null)
				_text.text += " " + _variableParameterValues[_currentParameterIndex];
		}
	}
}
