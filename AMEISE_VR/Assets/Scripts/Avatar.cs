using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	/// <summary>
	/// Handles interactions with Avatar Game objects.
	/// </summary>
	public class Avatar : GameObjectModelBase
	{
		private static readonly Vector3 DialogPosition = new Vector3(0.0f, 11.0f, -7.0f);
		private static readonly Vector3 DialogScale = new Vector3(0.001f, 10.0f, 5.0f);
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

		/// <summary>
		/// Assign an employee to the avatar
		/// </summary>
		/// <param name="name"></param>
		/// <param name="speechBubble"></param>
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
			_speechBubble.MoveTo(new Vector3(0.0f, 14.5f, 0.0f));
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

		/// <summary>
		/// Assign a secretary to the avatar.
		/// </summary>
		/// <param name="speechBubble"></param>
		public void AssignSecretary(GameObject speechBubble)
		{
			IsSecretary = true;
			CreateSpeechBubble(speechBubble);
			HideBubble();
		}

		/// <summary>
		/// Show the speech bubble of the avatar which contains the answer of the last sent command.
		/// </summary>
		public void ShowBubble()
		{
			SetText(DefaultText);
		}

		/// <summary>
		/// Show a dialog with possible commands a user can give to the avatar.
		/// </summary>
		public void ShowDialog()
		{
			var dict = new Dictionary<string, object>();
			if(IsSecretary)
				KnowledgeBase.Instance.SecretaryCommands.ForEach(cmd => dict.Add(cmd.Name, cmd));
			else
				KnowledgeBase.Instance.EmployeeCommands.ForEach(cmd => dict.Add(cmd.Name, cmd));
			_dialogCreator.ShowSelectionDialog("Give a command to " + (string.IsNullOrEmpty(Name) ? "Secretary" : Name), dict, DialogPosition, DialogScale);
		}

		/// <summary>
		/// Is called when a Command Button in Dialog was clicked
		/// </summary>
		/// <param name="button"></param>
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
			//if parameters are needed for command.
			if (emptyParameter != null)
			{
				_dialogCreator.CloseDialog();
				_tempCommand = cmd;
				var dict = new Dictionary<string, object>();
				KnowledgeBase.Instance.GetValuesOfParameterType(emptyParameter.Parameter.Type).ForEach(par => dict.Add(par, par));
				_dialogCreator.ShowSelectionDialog("Select a parameter", dict, DialogPosition, DialogScale);
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

		/// <summary>
		/// is called when the answer of the sent command is received.
		/// </summary>
		/// <param name="messageObject"></param>
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
