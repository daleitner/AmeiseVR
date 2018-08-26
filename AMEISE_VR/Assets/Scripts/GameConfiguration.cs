using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class GameConfiguration
{
	public GameObject LoginControl;
	public GameObject GameSelectionControl;
	public GameObject LoginFailedControl;
	public GameObject FPSController;

	private InputField username;
	private InputField password;
	

	public GameConfiguration(GameObject fpsController, GameObject loginControl, GameObject gameSelectionControl, GameObject loginFailedControl)
	{
		LoginControl = loginControl;
		GameSelectionControl = gameSelectionControl;
		LoginFailedControl = loginFailedControl;
		FPSController = fpsController;

		var messageListener = FPSController.GetComponent<MessageListener>();
		messageListener.ReceivedMessage += ReceivedMessage;

		username = LoginControl.transform.Find("Username").gameObject.GetComponent<InputField>();
		password = LoginControl.transform.Find("Password").gameObject.GetComponent<InputField>();
		var loginButton = LoginControl.transform.Find("LoginButton").gameObject.GetComponent<Button>();
		loginButton.onClick.AddListener(LoginClicked);
		var cancelButton = LoginControl.transform.Find("CancelButton").gameObject.GetComponent<Button>();
		cancelButton.onClick.AddListener(CancelClicked);

		var startButton = GameSelectionControl.transform.Find("StartButton").gameObject.GetComponent<Button>();
		startButton.onClick.AddListener(StartClicked);
		var backButton = GameSelectionControl.transform.Find("BackButton").gameObject.GetComponent<Button>();
		backButton.onClick.AddListener(BackClicked);

		var okButton = LoginFailedControl.transform.Find("OkButton").gameObject.GetComponent<Button>();
		okButton.onClick.AddListener(OkClicked);
	}

	public void OpenLoginDialog()
	{
		LoginControl.SetActive(true);
		var controller = FPSController.GetComponent<FirstPersonController>();
		controller.LockCursor();
		controller.enabled = false;
	}

	public void OpenGameSelectionDialog()
	{
		GameSelectionControl.SetActive(true);
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
	}

	void CancelClicked()
	{
		LoginControl.SetActive(false);
		var controller = FPSController.GetComponent<FirstPersonController>();
		controller.enabled = true;
		controller.UnlockCursor();
	}

	void StartClicked()
	{
		//var message = new MessageObject(MessageTypeEnum.Login, new Dictionary<string, string> { { "username", username.text }, { "password", password.text } });
		//var connection = ClientConnection.GetInstance();
		//connection.SendText(message);
		//Debug.Log(message.ToString());
		GameSelectionControl.SetActive(false);
		var controller = FPSController.GetComponent<FirstPersonController>();
		controller.enabled = true;
		controller.UnlockCursor();
	}

	void BackClicked()
	{
		GameSelectionControl.SetActive(false);
		LoginControl.SetActive(true);
	}

	void OkClicked()
	{
		LoginFailedControl.SetActive(false);
		LoginControl.SetActive(true);
	}

	void ReceivedMessage(MessageObject messageObject)
	{
		if (messageObject.Type == MessageTypeEnum.Login)
		{
			LoginControl.SetActive(false);
			var loginSuccessed = bool.Parse(messageObject.GetValueOf("success"));
			if (loginSuccessed)
			{
				GameSelectionControl.SetActive(true);
			}
			else
			{
				LoginFailedControl.SetActive(true);
			}
		}
	}
}
