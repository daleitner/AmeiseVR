using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class CommandDialog
{
	private GameObject _commandControl;
	private FirstPersonController _player;
	private InputField _command;
	private InputField _param1;
	private InputField _param2;
	private InputField _param3;
	private ListBox _lbCommands;
	private ListBox _lbEmployees;

	private List<Command> _commands;
	public CommandDialog(GameObject commandControl, FirstPersonController player)
	{
		_commandControl = commandControl;
		_player = player;
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
			var item = new ListBox.ListItem(command.Name);
			_lbCommands.AddItem(item);
		}
		CloseDialog();
	}

	public void SetParameters(Dictionary<string,List<string>> paramDict)
	{
		OpenDialog();
		foreach(var param in paramDict.Keys)
		{
			var item = new ListBox.ListItem(param);
			_lbEmployees.AddItem(item);
		}
		CloseDialog();
	}
}