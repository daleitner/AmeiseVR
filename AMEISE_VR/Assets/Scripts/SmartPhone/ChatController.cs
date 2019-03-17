using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class ChatController : ControllerBase
{
	private GameObject _view;
	private readonly TextMeshPro _lblName;
	private readonly GameObject _commandBar;
	private readonly GameObject _history;
	private readonly GameObject _commandList;
	private List<GameObject> _commands;

	private static readonly Vector3 CommandBarPositionBottom = new Vector3(0.061f, -0.4375f, 0.0f);
	private static readonly Vector3 CommandBarPositionTop = new Vector3(0.061f, 0.3125f, 0.0f);

	private bool _isCommandsVisible;
	public ChatController(GameObject view, SmartPhoneManager manager)
		: base(manager)
	{
		_view = view;
		_lblName = _view.transform.Find("LblName").gameObject.GetComponent<TextMeshPro>();
		_commandBar = _view.transform.Find("CommandBar").gameObject;
		_history = _view.transform.Find("History").gameObject;
		_commandList = _view.transform.Find("CommandList").gameObject;
		CreateCommandListItems();
	}

	public override bool Accepts(string tag)
	{
		return tag == Tags.SendCommandTag;
	}

	public override void Execute(GameObject gameObject)
	{
		if (gameObject.tag != Tags.SendCommandTag)
			return;

		if (!_isCommandsVisible)
			OpenCommandList();
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
			CloseCommandList();
		else
			Manager.Show(ScreenEnum.ChatListScreen);
	}

	private void OpenCommandList()
	{
		_commandBar.transform.localPosition = CommandBarPositionTop;
		_history.SetActive(false);
		_commandList.SetActive(true);
		_isCommandsVisible = true;
		LoadCommandList();
	}


	private void CloseCommandList()
	{
		_commandBar.transform.localPosition = CommandBarPositionBottom;
		_history.SetActive(true);
		_commandList.SetActive(false);
		_isCommandsVisible = false;
	}

	private void CreateCommandListItems()
	{
		_commands = new List<GameObject>();
		var entry = _commandList.transform.Find("Entry").gameObject;
		var yOffset = 0.385f;
		for (var i = 0; i < 6; i++)
		{
			var newEntry = Manager.Create(entry);
			newEntry.transform.SetParent(_commandList.transform);
			newEntry.transform.localScale = new Vector3(1, 0.15f, 1);
			newEntry.transform.localPosition = new Vector3(0.06f, yOffset, 0);
			yOffset -= 0.15f;
			newEntry.transform.rotation = new Quaternion(0, 0, 0, 0);
			newEntry.SetActive(true);
			_commands.Add(newEntry);
		}
	}

	private void LoadCommandList()
	{
		for (var i = 0; i < _commands.Count; i++)
		{
			var nameObject = _commands[i].transform.Find("LblCommand").gameObject;
			nameObject.GetComponent<TextMeshPro>().text = KnowledgeBase.Instance.Commands[i].Name;
		}
	}
}