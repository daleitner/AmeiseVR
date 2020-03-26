using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Scripts
{
	/// <summary>
	/// Represents the 2D login dialog.
	/// </summary>
	public class LoginDialog
	{
		private readonly GameObject _loginControl;
		private readonly InputField _username;
		private readonly InputField _password;
		private readonly FirstPersonController _player;
		public LoginDialog(GameObject loginControl, FirstPersonController player)
		{
			_loginControl = loginControl;
			_player = player;
			_username = loginControl.transform.Find("Username").gameObject.GetComponent<InputField>();
			_password = loginControl.transform.Find("Password").gameObject.GetComponent<InputField>();
			var loginButton = loginControl.transform.Find("LoginButton").gameObject.GetComponent<Button>();
			loginButton.onClick.AddListener(LoginClicked);
			var cancelButton = loginControl.transform.Find("CancelButton").gameObject.GetComponent<Button>();
			cancelButton.onClick.AddListener(CancelClicked);
		}

		private void LoginClicked()
		{
			var message = new MessageObject(MessageTypeEnum.Login, new Dictionary<string, string> { { "username", _username.text }, { "password", _password.text } });
			var connection = ClientConnection.GetInstance();
			connection.SendText(message);
		}

		private void CancelClicked()
		{
			CloseDialog();
		}

		public void OpenDialog()
		{
			_loginControl.SetActive(true);
			_player.LockCursor();
			_player.enabled = false;
		}

		public void CloseDialog()
		{
			_loginControl.SetActive(false);
			_player.enabled = true;
			_player.UnlockCursor();
		}
	}
}
