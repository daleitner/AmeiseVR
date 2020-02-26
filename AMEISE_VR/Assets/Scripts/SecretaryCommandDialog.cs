using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Scripts
{
	public class SecretaryCommandDialog
	{
		private const float YFirst = 170.0f;
		private const float YOffset = 35.0f;
		private const float YCancelButton = -180.0f;
		private readonly GameObject _secretaryCommandControl;
		private readonly FirstPersonController _player;
		private Avatar _avatar;
		private List<CommandButton> _buttons;
		private readonly MessageListener _listener;

		public SecretaryCommandDialog(GameObject secretaryCommandControl, MessageListener listener, FirstPersonController player)
		{
			_secretaryCommandControl = secretaryCommandControl;
			_player = player;
			_buttons = new List<CommandButton>();
			_listener = listener;
		}

		public void CloseDialog()
		{
			_secretaryCommandControl.SetActive(false);
			_player.UnlockCursor();
			_player.enabled = true;
		}

		public void OpenDialog()
		{
			_secretaryCommandControl.SetActive(true);
			_player.LockCursor();
			_player.enabled = false;
		}

		public void SetAvatar(Avatar avatar)
		{
			_avatar = avatar;
		}

		public void AddButton(CommandButton button)
		{
			button.SetParent(_secretaryCommandControl);
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

			_listener.ReceivedMessage += _listener_ReceivedMessage;
			ClientConnection.GetInstance().SendCommand(command.Command, command.ParameterValues.Select(x => x.Value).ToArray());
			_avatar.ShowBubble();

			CloseDialog();
		}

		private void _listener_ReceivedMessage(MessageObject messageObject)
		{
			if (messageObject.Type == MessageTypeEnum.Feedback)
			{
				_listener.ReceivedMessage -= _listener_ReceivedMessage;
				_avatar.SetText(messageObject.GetValueOf("feedback") + " You will find the information tomorrow in your history book.");
			}
		}

		public void AddCancelButton(GameObject cancelButton)
		{
			var button = new CommandButton(cancelButton, null);
			button.SetParent(_secretaryCommandControl);
			button.MoveTo(new Vector3(0.0f, YCancelButton, 0.0f));
			button.Rotate(new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
			button.Scale(new Vector3(1.0f, 1.0f, 1.0f));
			button.ButtonClicked += Button_CancelClicked;
		}

		private void Button_CancelClicked(CommandButton button)
		{
			CloseDialog();
		}
	}
}
