using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityStandardAssets.Characters.FirstPerson;
using Button = UnityEngine.UI.Button;
using Object = UnityEngine.Object;

public class GameConfiguration
{
	public GameObject LoginControl;
	public GameObject GameSelectionControl;
	public GameObject LoginFailedControl;
	public GameObject FPSController;
	public GameObject CanvasControl;
	public GameObject HistoryControl;

	private InputField username;
	private InputField password;

	private GameObject toggle;
	private List<GameObject> toggles;

	public GameConfiguration(GameObject fpsController, GameObject loginControl, GameObject gameSelectionControl, GameObject loginFailedControl, GameObject canvasControl, GameObject historyControl)
	{
		LoginControl = loginControl;
		GameSelectionControl = gameSelectionControl;
		LoginFailedControl = loginFailedControl;
		FPSController = fpsController;
		CanvasControl = canvasControl;
		HistoryControl = historyControl;

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

	public void OpenHistoryDialog()
	{
		HistoryControl.SetActive(true);
		var controller = FPSController.GetComponent<FirstPersonController>();
		controller.LockCursor();
		controller.enabled = false;
	}

	public void CloseHistoryDialog()
	{
		HistoryControl.SetActive(false);
		var controller = FPSController.GetComponent<FirstPersonController>();
		controller.UnlockCursor();
		controller.enabled = true;
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
			var listbox = GameSelectionControl.transform.Find("Listbox").GetComponent<ListBox>();
			var count = int.Parse(messageObject.GetValueOf("count"));
			for (var i = 0; i < count; i++)
			{
				var game = messageObject.GetValueOf("game" + i);
				var newToggle = Object.Instantiate(toggle, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
				newToggle.SetActive(true);
				newToggle.transform.SetParent(GameSelectionControl.transform);
				newToggle.GetComponent<Toggle>().isOn = i == 0;
				newToggle.transform.Find("Label").gameObject.GetComponent<Text>().text = game;
				//newToggle.transform.position = new Vector3(433.0f, 301.0f - (23.0f * i), 0.0f);
				listbox.AddItem(new ListBox.ListItem(newToggle));
				toggles.Add(newToggle);
			}
		}
		else if (messageObject.Type == MessageTypeEnum.ContinueGame)
		{
			var today = DateTime.Parse(messageObject.GetValueOf("current"));
			CanvasControl.transform.Find("Today").gameObject.GetComponent<Text>().text = today.ToString(CultureInfo.InvariantCulture);
			FPSController.transform.position = new Vector3(230.0f, 21.0f, 163.0f);
			HistoryControl.SetActive(true);
			var listbox = HistoryControl.transform.Find("Listbox").GetComponent<ListBox>();
			var feedback = messageObject.GetValueOf("feedback");
			var feedbacks = feedback.Split('\n');
			for (var i = 0; i < feedbacks.Length; i++)
			{
				var newItem = new ListBox.ListItem(feedbacks[i]);
				listbox.AddItem(newItem);
			}
			listbox.AddItem(new ListBox.ListItem("----------------------------------------------------------------------------------------------------"));
			HistoryControl.SetActive(false);
		}
		else if (messageObject.Type == MessageTypeEnum.DictionaryAndParameter)
		{
			var commandCount = int.Parse(messageObject.GetValueOf("count"));
			var commands = new List<Command>();
			var paramTypes = new List<string>();
			for (var i = 0; i < commandCount; i++)
			{
				var command = new Command();

				command.Name = messageObject.GetValueOf("command" + i);
				int paramcnt = int.Parse(messageObject.GetValueOf("command" + i + "_paramcnt"));
				for(var j = 0; j<paramcnt; j++)
				{
					var paramName = messageObject.GetValueOf("command" + i + "_param" + j);
					var paramType = messageObject.GetValueOf("command" + i + "_paramtype" + j);
					command.ParameterTypes.Add(paramName, paramType);
					command.ParameterValues.Add(paramName, null);

					if (!paramTypes.Contains(paramType))
						paramTypes.Add(paramType);
				}
				commands.Add(command);
			}

			var paramDict = new Dictionary<string, List<string>>();
			foreach(var type in paramTypes)
			{
				var parameters = new List<string>();
				var cnt = int.Parse(messageObject.GetValueOf(type + "_cnt"));
				for(var i = 0; i<cnt; i++)
				{
					parameters.Add(messageObject.GetValueOf(type + i));
				}
				paramDict.Add(type, parameters);
			}
			var str = "";
			commands.ForEach(x => str += x + "\r\n");
			Debug.Log("commands:\r\n" + str);
		}
	}
}
