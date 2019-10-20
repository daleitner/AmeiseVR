using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Scripts
{
	public class LoginFailedDialog
	{
		private readonly GameObject _gameObject;
		private readonly FirstPersonController _player;
		public LoginFailedDialog(GameObject loginFailedControl, FirstPersonController player)
		{
			_gameObject = loginFailedControl;
			_player = player;

			AddOkClickedListener(OkClicked);
		}

		private void OkClicked()
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

		public void AddOkClickedListener(UnityEngine.Events.UnityAction action)
		{
			var okButton = _gameObject.transform.Find("OkButton").gameObject.GetComponent<Button>();
			okButton.onClick.AddListener(action);
		}
	}
}
