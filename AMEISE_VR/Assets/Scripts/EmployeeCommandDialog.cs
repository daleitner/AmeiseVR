using System.Linq;
using Boo.Lang;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Scripts
{
	public class EmployeeCommandDialog
	{
		private const string Title = "Give a command to ";
		private const float YFirst = 170.0f;
		private const float YOffset = 35.0f;
		private const float YCancelButton = -106.0f;
		private readonly GameObject _employeeCommandControl;
		private readonly FirstPersonController _player;
		private Avatar _avatar;
		private List<CommandButton> _buttons;

		public EmployeeCommandDialog(GameObject employeeCommandControl, FirstPersonController player)
		{
			_employeeCommandControl = employeeCommandControl;
			_player = player;
			_buttons = new List<CommandButton>();
		}

		public void CloseDialog()
		{
			_employeeCommandControl.SetActive(false);
			_player.UnlockCursor();
			_player.enabled = true;
		}

		public void OpenDialog()
		{
			_employeeCommandControl.SetActive(true);
			_player.LockCursor();
			_player.enabled = false;
		}

		public void SetAvatar(Avatar avatar)
		{
			_avatar = avatar;
			SetTitle();
		}

		public void AddButton(CommandButton button)
		{
			button.SetParent(_employeeCommandControl);
			button.MoveTo(new Vector3(0.0f, YFirst - YOffset * _buttons.Count, 0.0f));
			button.Rotate(new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
			button.Scale(new Vector3(1.0f, 1.0f, 1.0f));
			button.ButtonClicked += Button_ButtonClicked;
			_buttons.Add(button);
		}

		private void Button_ButtonClicked(CommandButton button)
		{
			var command = new CommandInstance(button.Command);
			var firstParameter = command.GetNextEmptyParameter();
			if (firstParameter != null)
			{
				if (firstParameter.Parameter.Type == KnowledgeBase.EmployeeType)
					firstParameter.Value = _avatar.Name;
			}
			ClientConnection.GetInstance().SendCommand(command.Command, command.ParameterValues.Select(x => x.Value).ToArray());
			CloseDialog();
		}

		public void AddCancelButton(GameObject cancelButton)
		{
			var button = new CommandButton(cancelButton, null);
			button.SetParent(_employeeCommandControl);
			button.MoveTo(new Vector3(0.0f, YCancelButton, 0.0f));
			button.Rotate(new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
			button.Scale(new Vector3(1.0f, 1.0f, 1.0f));
			button.ButtonClicked += Button_CancelClicked;
		}

		private void Button_CancelClicked(CommandButton button)
		{
			CloseDialog();
		}

		private void SetTitle()
		{
			var textBox = _employeeCommandControl.transform.Find("TextLabel").GetComponent<Text>();
			textBox.text = Title + _avatar.Name;
		}
	}
}
