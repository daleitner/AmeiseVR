using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class Detection : MonoBehaviour
{
    // GENERAL SETTINGS
    [Header("General Settings")]
    [Tooltip("How close the player has to be in order to be able to open/close the door.")]
    public float Reach = 4.0F;
    [HideInInspector] public bool InReach;

    // UI SETTINGS
    [Header("UI Settings")]
    [Tooltip("The image or text that will be shown whenever the player is in reach of the door.")]
    public GameObject TextPrefab; // A text element to display when the player is in reach of the door
    [HideInInspector] public GameObject TextPrefabInstance; // A copy of the text prefab to prevent data corruption
    [HideInInspector] public bool TextActive;

	[Tooltip("The image or text that is shown in the middle of the the screen.")]
    public GameObject CrosshairPrefab;
    [HideInInspector] public GameObject CrosshairPrefabInstance; // A copy of the crosshair prefab to prevent data corruption

	public GameObject LoginControl;
	public GameObject GameSelectionControl;
	public GameObject LoginFailedControl;
	public GameObject FPSController;
	public GameObject CommandControl;
	public GameObject LoginText;
	public GameObject Book;
	public GameObject Office;
	public GameObject Avatar;
	public GameObject Task;
	public GameObject PlayerBoard;
	public GameObject EmployeeCommandControl;
	public GameObject SecretaryCommandControl;
	public GameObject SpeechBubble;

	private GameConfiguration config;

	// DEBUG SETTINGS
	[Header("Debug Settings")]
    [Tooltip("The color of the debugray that will be shown in the scene view window when playing the game.")]
    public Color DebugRayColor;
    [Tooltip("The opacity of the debugray.")]
    [Range(0.0F, 1.0F)]
    public float Opacity = 1.0F;

    private bool _mouseClicked = false;

    void Start()
    {
        gameObject.name = "Player";
        gameObject.tag = "Player";

        if (CrosshairPrefab == null) Debug.Log("<color=yellow><b>No CrosshairPrefab was found.</b></color>"); // Return an error if no crosshair was specified

        else
        {
            CrosshairPrefabInstance = Instantiate(CrosshairPrefab); // Display the crosshair prefab
            CrosshairPrefabInstance.transform.SetParent(transform, true); // Make the player the parent object of the crosshair prefab
        }

        if (TextPrefab == null) Debug.Log("<color=yellow><b>No TextPrefab was found.</b></color>"); // Return an error if no text element was specified

		DebugRayColor.a = Opacity; // Set the alpha value of the DebugRayColor
		GameObjectCollection.AddGameObject(FPSController, GameObjectEnum.FPSController);
		GameObjectCollection.AddGameObject(LoginControl, GameObjectEnum.LoginControl);
		GameObjectCollection.AddGameObject(GameSelectionControl, GameObjectEnum.GameSelectionControl);
		GameObjectCollection.AddGameObject(LoginFailedControl, GameObjectEnum.LoginFailedControl);
		GameObjectCollection.AddGameObject(CommandControl, GameObjectEnum.CommandControl);
		GameObjectCollection.AddGameObject(EmployeeCommandControl, GameObjectEnum.EmployeeCommandControl);
		GameObjectCollection.AddGameObject(SecretaryCommandControl, GameObjectEnum.SecretaryCommandControl);
		GameObjectCollection.AddGameObject(Book, GameObjectEnum.Book);
		GameObjectCollection.AddGameObject(Avatar, GameObjectEnum.Avatar);
		GameObjectCollection.AddGameObject(Office, GameObjectEnum.Office);
		GameObjectCollection.AddGameObject(Task, GameObjectEnum.Task);
		GameObjectCollection.AddGameObject(PlayerBoard, GameObjectEnum.PlayerBoard);
		GameObjectCollection.AddGameObject(LoginText, GameObjectEnum.LoginText);
		GameObjectCollection.AddGameObject(SpeechBubble, GameObjectEnum.SpeechBubble);
		var historyBook = Instantiate(Book);
		GameObjectCollection.AddHistoryBook(historyBook);
		config = new GameConfiguration();
	}

	void Update()
    {
        // Set origin of ray to 'center of screen' and direction of ray to 'cameraview'
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0F));

        RaycastHit hit; // Variable reading information about the collider hit

		if (Input.GetKey("8"))
			config.OpenCommandDialog();
		else if (Input.GetKey("9"))
			config.CloseCommandDialog();
		// Cast ray from center of the screen towards where the player is looking
		if (Physics.Raycast(ray, out hit, Reach))
		{
			var isKnownTag = Tags.ContainsKey(hit.collider.tag);

			if (!isKnownTag || Tags[hit.collider.tag] == CommandTagEnum.Door && !KnowledgeBase.Instance.LoadingCommandsFinished)
			{
				InReach = false;

				// Destroy the UI element when Player is no longer in reach of the door
				if (TextActive)
				{
					DestroyImmediate(TextPrefabInstance);
					TextActive = false;
				}
				//Draw the ray as a colored line for debugging purposes.
				Debug.DrawRay(ray.origin, ray.direction * Reach, DebugRayColor);
				return;
			}

			var currentTag = Tags[hit.collider.tag];
			InReach = true;

			// Display the UI element when the player is in reach of the door
			if (TextActive == false && TextPrefab != null)
			{
				TextPrefabInstance = Instantiate(TextPrefab);
				TextActive = true;
				TextPrefabInstance.transform.SetParent(transform, true); // Make the player the parent object of the text element
				TextPrefabInstance.transform.Find("Text 1").GetComponent<Text>().text = ToolTips[currentTag];
			}

			switch (currentTag)
			{
				case CommandTagEnum.Door:
					

					// Give the object that was hit the name 'Door'
					GameObject Door = hit.transform.gameObject;

					// Get access to the 'Door' script attached to the object that was hit
					Door dooropening = Door.GetComponent<Door>();

					if (Input.GetMouseButtonDown(0))
					{
						// Open/close the door by running the 'Open' function found in the 'Door' script
						if (dooropening.RotationPending == false) StartCoroutine(hit.collider.GetComponent<Door>().Move());
					}
					break;
				case CommandTagEnum.Login:
					if (config.LoggedIn)
						break;
					
					if (Input.GetMouseButtonDown(0) && !LoginControl.activeSelf && !LoginFailedControl.activeSelf)
					{
						config.OpenLoginDialog();
					}
					break;
				case CommandTagEnum.Book:
					if (Input.GetMouseButtonDown(0))
					{
						var book = GameObjectCollection.GetBookByGameObject(hit.transform.gameObject);
						if (book.IsOpen())
							book.Close();
						else if (book.IsClosed())
							book.Open();
					}
					else if (Input.GetMouseButtonDown(1))
					{
						var book = GameObjectCollection.GetBookByGameObject(hit.transform.gameObject);
						if (book.BelongsToAShelf)
						{
							book.TriggerShelfMove();
						}
						else
						{
							book.TriggerRotation();
						}
					}
					break;
				case CommandTagEnum.BookNext:
					if (Input.GetMouseButtonDown(0))
					{
						var book = GameObjectCollection.GetBookByGameObject(hit.transform.gameObject);
						book.NextPage();
					}
					break;
				case CommandTagEnum.BookPrevious:
					if (Input.GetMouseButtonDown(0))
					{
						var book = GameObjectCollection.GetBookByGameObject(hit.transform.gameObject);
						book.PreviousPage();
					}
					break;
				case CommandTagEnum.Task:
					if (Input.GetMouseButtonDown(0))
					{
						var task = GameObjectCollection.GetTaskByGameObject(hit.transform.gameObject);
						var playerBoards = GameObjectCollection.PlayerBoardCollection;
						var playerBoard = playerBoards.PlayerBoards.SingleOrDefault(p => p.Tasks.Contains(task));
						if (playerBoard != null)
						{
							playerBoard.RemoveTask(task);
							task.RelatedTasks.ForEach(t => t.RemoveRelatedTask(task));
							Destroy(task.GameObject);
						}
						else
						{
							GameObjectCollection.TaskBoard.SelectTask(task);
						}
					}
					else if (Input.GetMouseButtonDown(1))
					{
						var task = GameObjectCollection.GetTaskByGameObject(hit.transform.gameObject);
						task.ChangeParameter();
						task.RelatedTasks.ForEach(t => t.ChangeParameter());
					}
					break;
				case CommandTagEnum.PlayerBoard:
					if (Input.GetMouseButtonDown(0))
					{
						var selectedTask = GameObjectCollection.TaskBoard.SelectedTask;
						if (selectedTask != null)
						{
							var playerBoards = GameObjectCollection.PlayerBoardCollection;
							var newTaskGameObject = Instantiate(GameObjectCollection.Task);
							var newTask = selectedTask.Clone(newTaskGameObject);
							playerBoards.AddTask(newTask, hit.transform.gameObject);
						}
					}
					break;
				case CommandTagEnum.SendCommand:
					if (Input.GetMouseButtonDown(0))
					{
						GameObjectCollection.PlayerBoardCollection.SendCommands();
					}
					break;
				case CommandTagEnum.Phone:
					if (Input.GetMouseButtonDown(0))
					{
						ClientConnection.GetInstance().SendCommand(KnowledgeBase.Instance.CustomerAcceptanceTestCommand);
						GameObjectCollection.HistoryBook.Open();
					}
					break;
				case CommandTagEnum.PostBox:
					if (Input.GetMouseButtonDown(0))
					{
						ClientConnection.GetInstance().SendCommand(KnowledgeBase.Instance.FinishProjectCommand);
					}
					break;
				case CommandTagEnum.WasteBin:
					if (Input.GetMouseButtonDown(0))
					{
						ClientConnection.GetInstance().SendCommand(KnowledgeBase.Instance.CancelProjectCommand);
					}
					break;
				case CommandTagEnum.Avatar:
					if (Input.GetMouseButtonDown(0))
					{
						var avatar = GameObjectCollection.AvatarsCollection.Get(hit.transform.gameObject);
						if (!avatar.IsDummy)
						{
							if (avatar.IsSecretary)
							{
								var secretaryDialog = GameObjectCollection.SecretaryCommandDialog;
								secretaryDialog.SetAvatar(avatar);
								secretaryDialog.OpenDialog();
							}
							else
							{
								var employeeDialog = GameObjectCollection.EmployeeCommandDialog;
								employeeDialog.SetAvatar(avatar);
								employeeDialog.OpenDialog();
							}
						}
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
        }
        else
        {
            InReach = false;

            // Destroy the UI element when Player is no longer in reach of the door
            if (TextActive == true)
            {
                DestroyImmediate(TextPrefabInstance);
                TextActive = false;
            }
		}

        //Draw the ray as a colored line for debugging purposes.
        Debug.DrawRay(ray.origin, ray.direction * Reach, DebugRayColor);
    }

	public enum CommandTagEnum
	{
		Door,
		Login,
		Book,
		BookNext,
		BookPrevious,
		Task,
		PlayerBoard,
		SendCommand,
		Phone,
		PostBox,
		WasteBin,
		Avatar
	}

	private static readonly Dictionary<string, CommandTagEnum> Tags = new Dictionary<string, CommandTagEnum>
	{
		{"Door", CommandTagEnum.Door},
		{"Login", CommandTagEnum.Login},
		{"Book", CommandTagEnum.Book},
		{"BookNext", CommandTagEnum.BookNext},
		{"BookBack", CommandTagEnum.BookPrevious},
		{"Task", CommandTagEnum.Task},
		{"PlayerBoard", CommandTagEnum.PlayerBoard},
		{"SendCommand", CommandTagEnum.SendCommand},
		{"Phone", CommandTagEnum.Phone},
		{"PostBox", CommandTagEnum.PostBox},
		{"WasteBin", CommandTagEnum.WasteBin},
		{"Avatar", CommandTagEnum.Avatar}
	};

	private static readonly Dictionary<CommandTagEnum, string> ToolTips = new Dictionary<CommandTagEnum, string>
	{
		{CommandTagEnum.Door, "Open Door" },
		{CommandTagEnum.Login, "" },
		{CommandTagEnum.Book, "" },
		{CommandTagEnum.BookNext, "" },
		{CommandTagEnum.BookPrevious, "" },
		{CommandTagEnum.Task, "" },
		{CommandTagEnum.PlayerBoard, "" },
		{CommandTagEnum.SendCommand, "Assign Tasks from Whiteboard" },
		{CommandTagEnum.Phone, "Call customer to perform acceptance tests" },
		{CommandTagEnum.PostBox, "Deliver System" },
		{CommandTagEnum.WasteBin, "Cancel Project" },
		{CommandTagEnum.Avatar, "" }
	};
}
