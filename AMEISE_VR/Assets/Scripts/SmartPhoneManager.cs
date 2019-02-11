using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SmartPhoneManager : MonoBehaviour
{
	private enum ScreenEnum
	{
		MainScreen,
		ChatListScreen
	}
	public GameObject SmartPhone;
	public float Reach = 4.0F;
	[HideInInspector] public bool InReach;

	private Dictionary<ScreenEnum, GameObject> _views;
	private GameObject _activeView;

	private void Start()
	{
		KnowledgeBase.Instance.Employees.AddRange(new []{ "Alex", "Richard", "Stefanie", "Christine", "Thomas", "Daniel"});
		_views = new Dictionary<ScreenEnum, GameObject>();
		_views.Add(ScreenEnum.MainScreen, SmartPhone.transform.Find(ScreenEnum.MainScreen.ToString()).gameObject);
		_views.Add(ScreenEnum.ChatListScreen, SmartPhone.transform.Find(ScreenEnum.ChatListScreen.ToString()).gameObject);
		_activeView = _views[ScreenEnum.MainScreen];
	}

	private void Update()
	{
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0F));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Reach))
		{
			if (hit.collider.tag == "Chat")
			{
				InReach = true;
				if(Input.GetMouseButton(0))
				{
					LoadChatList();
					Show(ScreenEnum.ChatListScreen);
				}
			}
			else
			{
				InReach = false;
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
	}

	private void LoadChatList()
	{
		var chatListScreen = _views[ScreenEnum.ChatListScreen];
		foreach (var employee in KnowledgeBase.Instance.Employees)
		{

		}
	}
}