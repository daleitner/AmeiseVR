using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChatController : ControllerBase
{
	private GameObject _view;
	private readonly TextMeshPro _lblName;
	private readonly TextMeshPro _tbMessage;
	private readonly GameObject _commandBar;
	private readonly ListControl _historyList;
	private readonly ListControl _commandList;
	private List<GameObject> _commands;
	private CommandInstance _selectedCommand;

	private static readonly Vector3 CommandBarPositionBottom = new Vector3(0.061f, -0.4375f, 0.0f);
	private static readonly Vector3 CommandBarPositionTop = new Vector3(0.061f, 0.3125f, 0.0f);

	private bool _isCommandsVisible;
	private bool _isCommandSelected;
	public ChatController(GameObject view, SmartPhoneManager manager)
		: base(manager)
	{
		_view = view;
		_lblName = _view.transform.Find("LblName").gameObject.GetComponent<TextMeshPro>();
		_commandBar = _view.transform.Find("CommandBar").gameObject;
		_tbMessage = _commandBar.transform.Find("MessageBox").Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
		var history = _view.transform.Find("History").gameObject;
		_historyList = new ListControl(history, 6, Manager);
		var commandList = _view.transform.Find("CommandList").gameObject;
		_commandList = new ListControl(commandList, 6, Manager);
	}

	public override bool Accepts(string tag)
	{
		return tag == Tags.SendCommandTag ||
			tag == Tags.UpTag ||
			tag == Tags.DownTag ||
			tag == Tags.CommandListItemTag ||
			tag == Tags.MessageBoxTag;
	}

	public override void Execute(GameObject gameObject)
	{
		if (gameObject.tag == Tags.MessageBoxTag)
		{
			if (!_isCommandsVisible)
			{
				_selectedCommand = null;
				_isCommandSelected = false;
				OpenCommandList();
			}

			return;
		}
		if (gameObject.tag == Tags.SendCommandTag)
		{
			if (_selectedCommand != null && _selectedCommand.GetNextEmptyParameter() == null)
				SendCommand();
			return;
		}
		if (gameObject.tag == Tags.UpTag)
		{
			if (_isCommandsVisible)
				_commandList.Up();
			else
				_historyList.Up();
			return;
		}

		if (gameObject.tag == Tags.DownTag)
		{
			if (_isCommandsVisible)
				_commandList.Down();
			else
				_historyList.Down();
			return;
		}

		if (gameObject.tag == Tags.CommandListItemTag)
		{
			if (!_isCommandSelected)
			{
				_isCommandSelected = true;
				SelectCommand(gameObject);
			}
			else
			{
				SelectParameter(gameObject);
			}
			var nextParam = _selectedCommand.GetNextEmptyParameter();
			if (nextParam != null)
				_commandList.Items = KnowledgeBase.Instance.GetValuesOfParameterType(nextParam.Parameter.Type)
					.Select(x => (object) x).ToList();
			else
				CloseCommandList();
			return;
		}
	}

	private void SendCommand()
	{
		var dict = new Dictionary<string, string>();
		dict.Add("command", _selectedCommand.Command.Name);
		var i = 0;
		foreach (var param in _selectedCommand.ParameterValues)
		{
			dict.Add("param" + (i + 1), param.Value);
			i++;
		}
		var message = new MessageObject(MessageTypeEnum.Command, dict);
		var connection = ClientConnection.GetInstance();
		connection.SendText(message);
	}

	private void SelectCommand(GameObject listItem)
	{
		var commandText = _commandList.GetSelectedItem(listItem);
		var command = KnowledgeBase.Instance.Commands.FirstOrDefault(x => x.Name == commandText.ToString());
		_selectedCommand = new CommandInstance(command);

		var firstEmployee = _selectedCommand.ParameterValues.FirstOrDefault(x => x.Parameter.Type == KnowledgeBase.EmployeeType);
		if (firstEmployee != null)
		{
			firstEmployee.Value = _lblName.text;
		}

		_tbMessage.text = _selectedCommand.ToString();
	}

	private void SelectParameter(GameObject listItem)
	{
		var nextParam = _selectedCommand.GetNextEmptyParameter();
		nextParam.Value = _commandList.GetSelectedItem(listItem).ToString();
		_tbMessage.text = _selectedCommand.ToString();
	}

	public override void Activate(object payload)
	{
		var gameObject = payload as GameObject;
		if (gameObject == null)
			return;
		var helper = new EntryHelper(gameObject);
		_lblName.text = helper.GetName();
	}

	public override void Back()
	{
		if (_isCommandsVisible)
		{
			CloseCommandList();
			_isCommandSelected = false;
		}
		else
			Manager.Show(ScreenEnum.ChatListScreen);
	}

	private void OpenCommandList()
	{
		_commandBar.transform.localPosition = CommandBarPositionTop;
		_historyList.SetActive(false);
		_commandList.SetActive(true);
		_isCommandsVisible = true;
		_commandList.Items = KnowledgeBase.Instance.Commands.Select(x => (object)x.Name).ToList();
	}


	private void CloseCommandList()
	{
		_commandBar.transform.localPosition = CommandBarPositionBottom;
		_historyList.SetActive(true);
		_commandList.SetActive(false);
		_isCommandsVisible = false;
	}
}