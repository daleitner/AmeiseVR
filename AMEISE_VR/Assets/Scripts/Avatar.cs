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
		private Command _tempCommand;
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
			_speechBubble.MoveTo(new Vector3(0.0f, 14.0f, 0.0f));
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

			if (buttons[button] == null)
			{
				_tempCommand = null;
				_dialogCreator.CloseDialog();
				return;
			}

			var cmd = _tempCommand ?? buttons[button] as Command;
			var parameter = buttons[button] as string;
			
			var command = CreateCommandInstance(cmd, parameter);
			var emptyParameter = command.GetNextEmptyParameter();
			if (emptyParameter != null)
			{
				_dialogCreator.CloseDialog();
				_tempCommand = cmd;
				var dict = new Dictionary<string, object>();
				KnowledgeBase.Instance.GetValuesOfParameterType(emptyParameter.Parameter.Type).ForEach(par => dict.Add(par, par));
				_dialogCreator.ShowSelectionDialog("Select a parameter", dict);
				return;
			}

			SendCommand(command);
			ShowBubble();

			_dialogCreator.CloseDialog();
		}

		private CommandInstance CreateCommandInstance(Command cmd, string parameter = null)
		{
			var command = new CommandInstance(cmd);
			var firstParameter = command.GetNextEmptyParameter();
			if (firstParameter != null)
			{
				firstParameter.Value = firstParameter.Parameter.Type == KnowledgeBase.EmployeeType ? Name : parameter;
			}
			return command;
		}

		private void SendCommand(CommandInstance cmd)
		{
			_listener.ReceivedMessage += _listener_ReceivedMessage;
			ClientConnection.GetInstance().SendCommand(cmd.Command, cmd.ParameterValues.Select(x => x.Value).ToArray());
			_tempCommand = null;
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
