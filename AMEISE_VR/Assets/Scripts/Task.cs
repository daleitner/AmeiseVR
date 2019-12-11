using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
	public class Task : GameObjectModelBase
	{
		private readonly TextMeshPro _text;
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
		}
		
		public Command Command { get; }

		public void ChangeParameter()
		{
			if (_variableParameterValues == null)
				return;

			_currentParameterIndex++;
			if (_currentParameterIndex == _variableParameterValues.Count)
				_currentParameterIndex = 0;
			SetText();
		}

		private void SetText()
		{
			_text.text = Command.Name;
			if (_variableParameterValues != null)
				_text.text += " " + _variableParameterValues[_currentParameterIndex];
		}
	}
}
