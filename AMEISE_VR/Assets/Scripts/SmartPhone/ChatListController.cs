using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatListController : ControllerBase
{
	private GameObject _view;
	private List<GameObject> _chatEntries;
	public ChatListController(GameObject view, SmartPhoneManager manager)
		:base(manager)
	{
		_view = view;
		CreateChatEntries();
	}

	public override bool Accepts(string tag)
	{
		return tag == Tags.PersonChatTag;
	}

	public override void Execute(GameObject gameObject)
	{
		if(gameObject.tag == Tags.PersonChatTag)
		{
			Manager.Show(ScreenEnum.ChatScreen, gameObject);
		}
	}

	public override void Activate(object payload)
	{
		LoadChatList();
	}

	public override void Back()
	{
		Manager.Show(ScreenEnum.MainScreen);
	}

	private void CreateChatEntries()
	{
		_chatEntries = new List<GameObject>();
		var entry = _view.transform.Find("Entry").gameObject;
		var yOffset = 0.31f;
		for (var i = 0; i < 6; i++)
		{
			var newEntry = Manager.Create(entry);
			newEntry.transform.SetParent(_view.transform);
			newEntry.transform.localScale = new Vector3(1, 0.15f, 1);
			newEntry.transform.localPosition = new Vector3(0.06f, yOffset, 0);
			yOffset -= 0.15f;
			newEntry.transform.rotation = new Quaternion(0, 0, 0, 0);
			newEntry.SetActive(true);
			_chatEntries.Add(newEntry);
		}
	}

	private void LoadChatList()
	{
		for (var i = 0; i < _chatEntries.Count; i++)
		{
			var nameObject = _chatEntries[i].transform.Find("LblName").gameObject;
			nameObject.GetComponent<TextMeshPro>().text = KnowledgeBase.Instance.Employees[i];
		}
	}
}