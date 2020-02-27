using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class Avatar : GameObjectModelBase
	{
		private readonly MessageListener _listener;
		private const string DefaultText = "Loading...";
		private SpeechBubble _speechBubble;
		private DialogCreator _dialogCreator;
		public Avatar(GameObject avatar, MessageListener listener)
			:base(avatar)
		{
			_listener = listener;
			_dialogCreator = avatar.GetComponent<DialogCreator>();
		}

		public string Name { get; private set; }

		public bool IsDummy => string.IsNullOrEmpty(Name) && !IsSecretary;
		public bool IsSecretary { get; private set; }

		public void AssignEmployee(string name, GameObject speechBubble)
		{
			Name = name;
			CreateSpeechBubble(speechBubble);
			HideBubble();
		}

		private void CreateSpeechBubble(GameObject speechBubble)
		{
			_speechBubble = new SpeechBubble(speechBubble);
			_speechBubble.SetParent(GameObject);
			_speechBubble.MoveTo(new Vector3(0.0f, 2.0f, 0.0f));
		}

		public void SetText(string text)
		{
			_speechBubble.SetText(text);
			_speechBubble.Show();
		}

		public void HideBubble()
		{
			_speechBubble.Hide();
			_speechBubble.SetText(DefaultText);
		}

		public void AssignSecretary(GameObject speechBubble)
		{
			IsSecretary = true;
			CreateSpeechBubble(speechBubble);
			HideBubble();
		}

		public void ShowBubble()
		{
			SetText(DefaultText);
		}

		public void ShowDialog()
		{
			var dict = new Dictionary<string, object>();
			if(IsSecretary)
				KnowledgeBase.Instance.SecretaryCommands.ForEach(cmd => dict.Add(cmd.Name, cmd));
			else
				KnowledgeBase.Instance.EmployeeCommands.ForEach(cmd => dict.Add(cmd.Name, cmd));
			_dialogCreator.ShowSelectionDialog("Give a command to " + (string.IsNullOrEmpty(Name) ? "Secretary" : Name), dict);
		}

		public void ButtonClicked(GameObject button)
		{
			var buttons = _dialogCreator.GetButtons();
			if (buttons[button] is Command cmd)
			{
				SendCommand(cmd);
				ShowBubble();
			}

			_dialogCreator.CloseDialog();
		}

		private void SendCommand(Command cmd)
		{
			var command = new CommandInstance(cmd);
			var firstParameter = command.GetNextEmptyParameter();
			if (firstParameter != null)
			{
				if (firstParameter.Parameter.Type == KnowledgeBase.EmployeeType)
					firstParameter.Value = Name;
			}

			_listener.ReceivedMessage += _listener_ReceivedMessage;
			ClientConnection.GetInstance().SendCommand(command.Command, command.ParameterValues.Select(x => x.Value).ToArray());
		}

		private void _listener_ReceivedMessage(MessageObject messageObject)
		{
			if (messageObject.Type == MessageTypeEnum.Feedback)
			{
				_listener.ReceivedMessage -= _listener_ReceivedMessage;
				var message = messageObject.GetValueOf("feedback");
				if (IsSecretary)
					message += " You will find the information tomorrow in your history book.";
				SetText(message);
			}
		}
	}
}
