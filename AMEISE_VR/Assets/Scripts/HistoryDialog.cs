using UnityEngine;
using UnityEditor;
using UnityStandardAssets.Characters.FirstPerson;

public class HistoryDialog
{
	private GameObject _historyControl;
	private FirstPersonController _player;

	private ListBox _lbHistory;
	public HistoryDialog(GameObject historyControl, FirstPersonController player)
	{
		_historyControl = historyControl;
		_player = player;

		_lbHistory = _historyControl.transform.Find("Listbox").GetComponent<ListBox>();
	}

	public void CloseDialog()
	{
		_historyControl.SetActive(false);
		_player.UnlockCursor();
		_player.enabled = true;
	}

	public void OpenDialog()
	{
		_historyControl.SetActive(true);
		_player.LockCursor();
		_player.enabled = false;
	}

	public void Add(string item)
	{
		_historyControl.SetActive(true);
		var newItem = new ListBox.ListItem(item);
		_lbHistory.AddItem(newItem);
		_historyControl.SetActive(false);
	}
}