using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;

public class GameConfiguration
{
	private readonly LoginDialog _loginDialog;
	private readonly LoginFailedDialog _loginFailedDialog;
	private readonly GameSelectionDialog _gameSelectionDialog;
	private readonly CommandDialog _commandDialog;
	private readonly Book _historyBook;

	public GameConfiguration()
	{

		_loginDialog = GameObjectCollection.LoginDialog;
		_loginFailedDialog = GameObjectCollection.LoginFailedDialog;
		_gameSelectionDialog = GameObjectCollection.GameSelectionDialog;
		_commandDialog = GameObjectCollection.CommandDialog;
		_historyBook = GameObjectCollection.HistoryBook;
		var messageListener = GameObjectCollection.MessageListener;
		messageListener.ReceivedMessage += ReceivedMessage;

		_loginFailedDialog.AddOkClickedListener(OkClicked);
		_gameSelectionDialog.AddBackClickedListener(BackClicked);
	}

	public bool LoggedIn { get; private set; }

	public void OpenLoginDialog()
	{
		_loginDialog.OpenDialog();
	}

	public void OpenCommandDialog()
	{
		_commandDialog.OpenDialog();
	}

	public void CloseCommandDialog()
	{
		_commandDialog.CloseDialog();
	}

	private void BackClicked()
	{
		_loginDialog.OpenDialog();
	}

	private void OkClicked()
	{
		_loginDialog.OpenDialog();
	}

	void ReceivedMessage(MessageObject messageObject)
	{
		if (messageObject.Type == MessageTypeEnum.Login)
		{
			_loginDialog.CloseDialog();
			var loginSuccessed = bool.Parse(messageObject.GetValueOf("success"));
			if (loginSuccessed)
			{
				_gameSelectionDialog.OpenDialog();
				LoggedIn = true;
			}
			else
			{
				_loginFailedDialog.OpenDialog();
			}
		}
		else if (messageObject.Type == MessageTypeEnum.GameChoice)
		{
			var count = int.Parse(messageObject.GetValueOf("count"));
			var games = new List<string>();
			for (var i = 0; i < count; i++)
			{
				var game = messageObject.GetValueOf("game" + i);
				games.Add(game);
			}
			_gameSelectionDialog.AddListItems(games);
		}
		else if (messageObject.Type == MessageTypeEnum.ContinueGame)
		{
			var today = DateTime.Parse(messageObject.GetValueOf("current"));
			KnowledgeBase.Instance.Date = today;
			GameObjectCollection.MovePlayer(new Vector3(230.0f, 21.0f, 163.0f));
		}
		else if(messageObject.Type == MessageTypeEnum.Feedback || messageObject.Type == MessageTypeEnum.Callback)
		{
			var feedback = messageObject.GetValueOf("feedback");
			var feedbacks = feedback.Split('\n');
			for (var i = 0; i < feedbacks.Length; i++)
			{
				KnowledgeBase.Instance.AddMessage(feedbacks[i]);
				_historyBook.AppendText(feedbacks[i]);
			}
			_historyBook.AppendText("\n------------------------\n");
		}
		else if (messageObject.Type == MessageTypeEnum.DictionaryAndParameter)
		{
			var commandCount = int.Parse(messageObject.GetValueOf("count"));
			var commands = new List<Command>();
			var paramTypes = new List<string>();
			for (var i = 0; i < commandCount; i++)
			{
				var command = new Command();

				command.Name = messageObject.GetValueOf("command" + i);
				int paramcnt = int.Parse(messageObject.GetValueOf("command" + i + "_paramcnt"));
				for(var j = 0; j<paramcnt; j++)
				{
					var paramName = messageObject.GetValueOf("command" + i + "_param" + j);
					var paramType = messageObject.GetValueOf("command" + i + "_paramtype" + j);
					command.Parameters.Add(new Parameter(paramName, paramType));

					if (!paramTypes.Contains(paramType))
						paramTypes.Add(paramType);
				}
				commands.Add(command);
			}
			_commandDialog.SetCommands(commands);
			KnowledgeBase.Instance.Commands = commands;

			var paramDict = new Dictionary<string, List<string>>();
			foreach(var type in paramTypes)
			{
				var parameters = new List<string>();
				var cnt = int.Parse(messageObject.GetValueOf(type + "_cnt"));
				for(var i = 0; i<cnt; i++)
				{
					parameters.Add(messageObject.GetValueOf(type + i));
				}
				paramDict.Add(type, parameters);
				KnowledgeBase.Instance.AddParameterType(type, parameters);
			}
			_commandDialog.SetParameters(paramDict);
			KnowledgeBase.Instance.SetLoadingCommandsFinished();

			var str = "";
			commands.ForEach(x => str += x + "\r\n");
			Debug.Log("commands:\r\n" + str);
		}
	}
}
