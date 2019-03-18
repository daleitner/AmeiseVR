using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChatController : ControllerBase
{
	private GameObject _view;
	private readonly TextMeshPro _lblName;
	private readonly GameObject _commandBar;
	private readonly ListControl _historyList;
	private readonly ListControl _commandList;
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
		var history = _view.transform.Find("History").gameObject;
		_historyList = new ListControl(history, 6, Manager);
		var commandList = _view.transform.Find("CommandList").gameObject;
		_commandList = new ListControl(commandList, 6, Manager);
	}

	public override bool Accepts(string tag)
	{
		return tag == Tags.SendCommandTag ||
			tag == Tags.UpTag ||
			tag == Tags.DownTag;
	}

	public override void Execute(GameObject gameObject)
	{
		if (gameObject.tag == Tags.SendCommandTag)
		{
			if (!_isCommandsVisible)
				OpenCommandList();
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