﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityStandardAssets.Characters.FirstPerson;
using Button = UnityEngine.UI.Button;

public class GameConfiguration
{
	public GameObject LoginControl;
	public GameObject GameSelectionControl;
	public GameObject LoginFailedControl;
	public GameObject FPSController;

	private InputField username;
	private InputField password;

	private GameObject toggle;
	private List<GameObject> toggles;

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
		toggle = GameSelectionControl.transform.Find("Toggle").gameObject;

		var okButton = LoginFailedControl.transform.Find("OkButton").gameObject.GetComponent<Button>();
		okButton.onClick.AddListener(OkClicked);

		toggles = new List<GameObject>();
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
		var selectedGame = toggles.Single(t => t.GetComponent<Toggle>().isOn).transform.Find("Label").gameObject.GetComponent<Text>().text;
		var message = new MessageObject(MessageTypeEnum.GameChoice, new Dictionary<string, string> { { "game", selectedGame } });
		var connection = ClientConnection.GetInstance();
		connection.SendText(message);
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
		else if (messageObject.Type == MessageTypeEnum.GameChoice)
		{
			var count = int.Parse(messageObject.GetValueOf("count"));
			for (var i = 0; i < count; i++)
			{
				var game = messageObject.GetValueOf("game" + i);
				var newToggle = Object.Instantiate(toggle, new Vector3(0.0f,0.0f,0.0f), Quaternion.identity);
				newToggle.SetActive(true);
				newToggle.transform.SetParent(GameSelectionControl.transform);
				newToggle.GetComponent<Toggle>().isOn = i == 0;
				newToggle.transform.Find("Label").gameObject.GetComponent<Text>().text = game;
				newToggle.transform.position = new Vector3(433.0f, 301.0f-(23.0f*i), 0.0f);
				toggles.Add(newToggle);
			}
		}
	}
}