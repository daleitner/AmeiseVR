using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Scripts
{
	public class GameSelectionDialog
	{
		private readonly GameObject _gameObject;
		private readonly FirstPersonController _player;
		private ListBox _listBox;
		private GameObject toggle;
		private List<GameObject> toggles;
		public GameSelectionDialog(GameObject gameSelectionControl, FirstPersonController player)
		{
			_gameObject = gameSelectionControl;
			_player = player;

			_listBox = _gameObject.transform.Find("Listbox").GetComponent<ListBox>();
			var startButton = _gameObject.transform.Find("StartButton").gameObject.GetComponent<Button>();
			startButton.onClick.AddListener(StartClicked);
			AddBackClickedListener(BackClicked);
			toggle = _gameObject.transform.Find("Toggle").gameObject;
			toggles = new List<GameObject>();
		}

		private void StartClicked()
		{
			var selectedGame = toggles.Single(t => t.GetComponent<Toggle>().isOn).transform.Find("Label").gameObject.GetComponent<Text>().text;
			var message = new MessageObject(MessageTypeEnum.GameChoice, new Dictionary<string, string> { { "game", selectedGame } });
			var connection = ClientConnection.GetInstance();
			connection.SendText(message);

			CloseDialog();
		}

		private void BackClicked()
		{
			CloseDialog();
		}

		public void OpenDialog()
		{
			_gameObject.SetActive(true);
			_player.LockCursor();
			_player.enabled = false;
		}

		public void CloseDialog()
		{
			_gameObject.SetActive(false);
			_player.enabled = true;
			_player.UnlockCursor();
		}

		public void AddBackClickedListener(UnityEngine.Events.UnityAction action)
		{
			var backButton = _gameObject.transform.Find("BackButton").gameObject.GetComponent<Button>();
			backButton.onClick.AddListener(action);
		}

		public void AddListItems(List<string> games)
		{
			for (int i = 0; i < games.Count; i++)
			{
				var newToggle = Object.Instantiate(toggle, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
				newToggle.SetActive(true);
				newToggle.transform.SetParent(_gameObject.transform);
				newToggle.GetComponent<Toggle>().isOn = i == 0;
				newToggle.transform.Find("Label").gameObject.GetComponent<Text>().text = games[i];
				//newToggle.transform.position = new Vector3(433.0f, 301.0f - (23.0f * i), 0.0f);
				_listBox.AddItem(new ListBox.ListItem(newToggle));
				toggles.Add(newToggle);
			}
		}
	}
}
