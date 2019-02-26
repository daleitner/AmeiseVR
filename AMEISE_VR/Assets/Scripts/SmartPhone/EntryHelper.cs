using System;
using TMPro;
using UnityEngine;

public class EntryHelper
{
	private readonly GameObject _entry;
	public EntryHelper(GameObject entry)
	{
		_entry = entry;
	}

	public string GetName()
	{
		GameObject lblName = null;
		if (_entry.name.StartsWith("Entry"))
		{
			lblName = _entry.transform.GetChild(1).gameObject;
		}
		else if (_entry.name == "LblName")
		{
			lblName = _entry;
		}
		else if(_entry.name == "ProfileImage")
		{
			lblName = _entry.transform.parent.GetChild(1).gameObject;
		}

		if(lblName == null)
			throw new ArgumentException("LblName was not found!");

		return lblName.GetComponent<TextMeshPro>().text;
	}
}