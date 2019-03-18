using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListControl
{
	private readonly SmartPhoneManager _manager;
	private readonly GameObject _listControlObject;
	private List<GameObject> _listItems;
	private List<object> _items;
	private int _startIndex;
	public ListControl(GameObject listControlObject, int items, SmartPhoneManager manager)
	{
		_manager = manager;
		_listControlObject = listControlObject;
		CreateListItems(items);
	}

	public List<object> Items
	{
		get
		{
			return _items;
		}
		set
		{
			_items = value;
			_startIndex = 0;
			UpdateList();
		}
	}

	public void Up()
	{
		if (_startIndex <= 0)
			return;
		_startIndex--;
		UpdateList();
	}

	public void Down()
	{
		if (_startIndex + _listItems.Count >= Items.Count)
			return;
		_startIndex++;
		UpdateList();
	}

	public void SetActive(bool isActive)
	{
		_listControlObject.SetActive(isActive);
	}

	private void CreateListItems(int count)
	{
		_listItems = new List<GameObject>();
		var entry = _listControlObject.transform.Find("Entry").gameObject;
		var yOffset = entry.transform.localPosition.y;
		var entryHeight = 0.15f;
		for (var i = 0; i < count; i++)
		{
			var newEntry = _manager.Create(entry);
			newEntry.transform.SetParent(_listControlObject.transform);
			newEntry.transform.localScale = new Vector3(1, entryHeight, 1);
			newEntry.transform.localPosition = new Vector3(0.06f, yOffset, 0);
			yOffset -= entryHeight;
			newEntry.transform.rotation = new Quaternion(0, 0, 0, 0);
			newEntry.SetActive(true);
			_listItems.Add(newEntry);
		}
	}

	private void UpdateList()
	{
		for (var i = 0; i < _listItems.Count; i++)
		{
			var nameObject = _listItems[i].transform.Find("LblEntry").gameObject;
			nameObject.GetComponent<TextMeshPro>().text = i + _startIndex < Items.Count ? Items[i + _startIndex].ToString() : string.Empty;
		}
	}
}