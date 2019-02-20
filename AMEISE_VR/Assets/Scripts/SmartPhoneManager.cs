using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class SmartPhoneManager : MonoBehaviour
{
	private enum ScreenEnum
	{
		MainScreen,
		ChatListScreen,
		ChatScreen
	}
	private const string ChatTag = "Chat";
	private const string PersonChatTag = "PersonChat";

	private List<GameObject> _chatEntries;

	public GameObject SmartPhone;
	public float Reach = 4.0F;
	[HideInInspector] public bool InReach;

	private Dictionary<ScreenEnum, GameObject> _views;
	private GameObject _activeView;
	private bool alreadyClicked = false;

	private void Start()
	{
		KnowledgeBase.Instance.Employees.AddRange(new []{ "Alex", "Richard", "Stefanie", "Christine", "Thomas", "Daniel"});
		_views = new Dictionary<ScreenEnum, GameObject>();
		var values = Enum.GetValues(typeof(ScreenEnum)).Cast<ScreenEnum>().ToList();
		values.ForEach(x => _views.Add(x, SmartPhone.transform.Find(x.ToString()).gameObject));
		CreateChatEntries();
		Show(ScreenEnum.MainScreen);
	}

	private void Update()
	{
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0F));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Reach))
		{
			if (Input.GetMouseButton(0))
			{
				if (alreadyClicked)
					return;
				alreadyClicked = true;
				if (hit.collider.tag == ChatTag)
				{
					InReach = true;
					LoadChatList();
					Show(ScreenEnum.ChatListScreen);

				}
				else if (hit.collider.tag == PersonChatTag)
				{
					InReach = true;
					Show(ScreenEnum.ChatScreen);
				}
				else
				{
					InReach = false;
				}
			}
			else
			{
				alreadyClicked = false;
			}
		}
		else
		{
			InReach = false;
		}
	}

	private void Show(ScreenEnum screenName)
	{
		foreach(var key in _views.Keys)
		{
			_views[key].SetActive(screenName == key);
		}
		_activeView = _views[screenName];
	}

	private void CreateChatEntries()
	{
		_chatEntries = new List<GameObject>();
		var chatListScreen = _views[ScreenEnum.ChatListScreen];
		var entry = chatListScreen.transform.Find("Entry").gameObject;
		var yOffset = 0.31f;
		for (var i = 0; i<6; i++)
		{
			var newEntry = Instantiate(entry);
			newEntry.transform.SetParent(chatListScreen.transform);
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
		for(var i = 0; i< _chatEntries.Count; i++)
		{
			var nameObject = _chatEntries[i].transform.Find("LblName").gameObject;
			nameObject.GetComponent<TextMeshPro>().text = KnowledgeBase.Instance.Employees[i];
		}
	}

}