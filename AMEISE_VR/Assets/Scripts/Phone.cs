using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class Phone : GameObjectModelBase
	{
		private static readonly Vector3 DialogPosition = new Vector3(0.0f, 0.52f, -0.14f);
		private static readonly Vector3 DialogScale = new Vector3(0.001f, 0.7f, 0.4f);
		private DialogCreator _dialogCreator;

		public Phone(GameObject phone) : base(phone)
		{
			_dialogCreator = phone.GetComponent<DialogCreator>();
		}

		public void ShowDialog()
		{
			var dict = new Dictionary<string, object>();
			KnowledgeBase.Instance.CustomerCommands.ForEach(cmd => dict.Add(cmd.Name, cmd));
			_dialogCreator.ShowSelectionDialog("Give a command to Customer", dict, DialogPosition, DialogScale);
		}

		public void ButtonClicked(GameObject button)
		{
			var buttons = _dialogCreator.GetButtons();

			if (buttons[button] == null)
			{
				_dialogCreator.CloseDialog();
				return;
			}

			var cmd = buttons[button] as Command;
			
			ClientConnection.GetInstance().SendCommand(cmd);
			GameObjectCollection.HistoryBook.Open();

			_dialogCreator.CloseDialog();
		}
	}
}
