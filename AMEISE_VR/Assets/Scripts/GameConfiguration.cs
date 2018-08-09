using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class GameConfiguration
{
	public GameObject LoginControl;
	public GameObject GameSelectionControl;
	public GameObject FPSController;

	private InputField username;
	private InputField password;

	public GameConfiguration(GameObject fpsController, GameObject loginControl, GameObject gameSelectionControl)
	{
		LoginControl = loginControl;
		GameSelectionControl = gameSelectionControl;
		FPSController = fpsController;

		username = LoginControl.transform.Find("Username").gameObject.GetComponent<InputField>();
		password = LoginControl.transform.Find("Password").gameObject.GetComponent<InputField>();
		var button = LoginControl.transform.Find("LoginButton").gameObject.GetComponent<Button>();
		button.onClick.AddListener(LoginClicked);
		var cancelButton = LoginControl.transform.Find("CancelButton").gameObject.GetComponent<Button>();
		cancelButton.onClick.AddListener(CancelClicked);
	}

	public void OpenLoginDialog()
	{
		LoginControl.SetActive(true);
		var controller = FPSController.GetComponent<FirstPersonController>();
		controller.LockCursor();
		controller.enabled = false;
	}

	void LoginClicked()
	{
		var message = new MessageObject(MessageTypeEnum.Login, new Dictionary<string, string> { { "username", username.text }, { "password", password.text } });
		var connection = ClientConnection.GetInstance();
		connection.SendText(message);
		Debug.Log(message.ToString());
		LoginControl.SetActive(false);
		var controller = FPSController.GetComponent<FirstPersonController>();
		controller.enabled = true;
		controller.UnlockCursor();
	}

	void CancelClicked()
	{
		LoginControl.SetActive(false);
		var controller = FPSController.GetComponent<FirstPersonController>();
		controller.enabled = true;
		controller.UnlockCursor();
	}
}
