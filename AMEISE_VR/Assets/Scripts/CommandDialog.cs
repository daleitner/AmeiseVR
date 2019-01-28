using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class CommandDialog
{
	private GameObject _commandControl;
	private FirstPersonController _player;
	private HistoryDialog _historyDialog;
	private InputField _command;
	private InputField _param1;
	private InputField _param2;
	private InputField _param3;
	private ListBox _lbCommands;
	private ListBox _lbEmployees;

	private List<Command> _commands;
	public CommandDialog(GameObject commandControl, FirstPersonController player, HistoryDialog historyDialog)
	{
		_commandControl = commandControl;
		_player = player;
		_historyDialog = historyDialog;
		_command = _commandControl.transform.Find("TbCommand").gameObject.GetComponent<InputField>();
		_param1 = _commandControl.transform.Find("TbParam1").gameObject.GetComponent<InputField>();
		_param2 = _commandControl.transform.Find("TbParam2").gameObject.GetComponent<InputField>();
		_param3 = _commandControl.transform.Find("TbParam3").gameObject.GetComponent<InputField>();

		_lbCommands = _commandControl.transform.Find("LbCommands").GetComponent<ListBox>();
		_lbEmployees = _commandControl.transform.Find("LbEmployees").GetComponent<ListBox>();

		var sendButton = _commandControl.transform.Find("BtnSend").gameObject.GetComponent<Button>();
		sendButton.onClick.AddListener(Send);

		var cancelButton = _commandControl.transform.Find("BtnCancel").gameObject.GetComponent<Button>();
		cancelButton.onClick.AddListener(CloseDialog);
	}

	public void Send()
	{
		MessageObject message;
		if (!_commands.Select(x => x.Name).ToList().Contains(_command.text))
		{
			int steps;
			if (!int.TryParse(_command.text, out steps))
				return;
			message = new MessageObject(MessageTypeEnum.Proceed, new Dictionary<string, string> { { "steps", steps.ToString()} });
		}
		else
		{
			var paramList = new List<string> { _param1.text, _param2.text, _param3.text };
			var command = _commands.Where(x => x.Name == _command.text).First();
			var dict = new Dictionary<string, string>();
			//var historyEntry = command.Name;
			dict.Add("command", command.Name);
			var i = 0;
			foreach (var param in command.ParameterTypes.Keys)
			{
				if (string.IsNullOrEmpty(paramList[i]))
					return;
				dict.Add("param" + (i + 1), paramList[i]);
				//historyEntry += " " + paramList[i];
				i++;
			}
			message = new MessageObject(MessageTypeEnum.Command, dict);
		}
		var connection = ClientConnection.GetInstance();
		connection.SendText(message);
		//_historyDialog.Add(historyEntry);
	}

	public void CloseDialog()
	{
		_commandControl.SetActive(false);
		_player.UnlockCursor();
		_player.enabled = true;
	}

	public void OpenDialog()
	{
		_commandControl.SetActive(true);
		_player.LockCursor();
		_player.enabled = false;
	}

	public void SetCommands(List<Command> commands)
	{
		_commands = commands;
		OpenDialog();
		foreach(var command in _commands)
		{
			var cmdstring = command.Name + "(";
			foreach(var param in command.ParameterTypes.Keys)
			{
				cmdstring += command.ParameterTypes[param].Substring(0, 1) + ", ";
			}
			if(command.ParameterTypes.Keys.Count > 0)
				cmdstring = cmdstring.Substring(0, cmdstring.Length - 2);
			cmdstring += ")";
			var item = new ListBox.ListItem(cmdstring);
			_lbCommands.AddItem(item);
		}
		CloseDialog();
	}

	public void SetParameters(Dictionary<string,List<string>> paramDict)
	{
		OpenDialog();
		foreach(var param in paramDict.Keys)
		{
			var separator = new ListBox.ListItem("----------------------------------------------------------------------------");
			var item = new ListBox.ListItem(param);
			_lbEmployees.AddItem(item);
			_lbEmployees.AddItem(separator);
			foreach(var val in paramDict[param])
			{
				var valItem = new ListBox.ListItem(val);
				_lbEmployees.AddItem(valItem);
			}
			_lbEmployees.AddItem(new ListBox.ListItem(""));
		}
		CloseDialog();
	}
}