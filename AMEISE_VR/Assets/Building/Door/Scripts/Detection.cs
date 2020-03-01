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
    [HideInInspector] public bool TextActive;

	[Tooltip("The image or text that is shown in the middle of the the screen.")]
    public GameObject CrosshairPrefab;
    [HideInInspector] public GameObject CrosshairPrefabInstance; // A copy of the crosshair prefab to prevent data corruption

	public GameObject LoginControl;
	public GameObject GameSelectionControl;
	public GameObject LoginFailedControl;
	public GameObject FPSController;
	public GameObject LoginText;
	public GameObject Book;
	public GameObject Office;
	public GameObject Task;
	public GameObject PlayerBoard;
	public GameObject SpeechBubble;
	public GameObject Phone;

	private GameConfiguration config;

	private GameObject currentToolTipGameObject;

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
		
		DebugRayColor.a = Opacity; // Set the alpha value of the DebugRayColor
		GameObjectCollection.AddGameObject(FPSController, GameObjectEnum.FPSController);
		GameObjectCollection.AddGameObject(LoginControl, GameObjectEnum.LoginControl);
		GameObjectCollection.AddGameObject(GameSelectionControl, GameObjectEnum.GameSelectionControl);
		GameObjectCollection.AddGameObject(LoginFailedControl, GameObjectEnum.LoginFailedControl);
		GameObjectCollection.AddGameObject(Book, GameObjectEnum.Book);
		GameObjectCollection.AddGameObject(Office, GameObjectEnum.Office);
		GameObjectCollection.AddGameObject(Task, GameObjectEnum.Task);
		GameObjectCollection.AddGameObject(PlayerBoard, GameObjectEnum.PlayerBoard);
		GameObjectCollection.AddGameObject(LoginText, GameObjectEnum.LoginText);
		GameObjectCollection.AddGameObject(SpeechBubble, GameObjectEnum.SpeechBubble);
		GameObjectCollection.AddGameObject(Phone, GameObjectEnum.Phone);
		var historyBook = Instantiate(Book);
		GameObjectCollection.AddHistoryBook(historyBook);
		config = new GameConfiguration();
	}

	void Update()
    {
        // Set origin of ray to 'center of screen' and direction of ray to 'cameraview'
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0F));

        RaycastHit hit; // Variable reading information about the collider hit

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
					if(currentToolTipGameObject != null)
					{
						currentToolTipGameObject.SetActive(false);
					}
					TextActive = false;
				}
				//Draw the ray as a colored line for debugging purposes.
				Debug.DrawRay(ray.origin, ray.direction * Reach, DebugRayColor);
				return;
			}

			var currentTag = Tags[hit.collider.tag];
			var currentGameObject = hit.transform.gameObject;
			InReach = true;

			// Display the UI element when the player is in reach of the door
			currentToolTipGameObject = GetToolTip(currentGameObject);
			
			if (TextActive == false)
			{
				if (currentToolTipGameObject != null)
				{
					currentToolTipGameObject.SetActive(true);
				}
				
				TextActive = true;
			}

			switch (currentTag)
			{
				case CommandTagEnum.Door:
					// Get access to the 'Door' script attached to the object that was hit
					Door dooropening = currentGameObject.GetComponent<Door>();

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
						var book = GameObjectCollection.GetBookByGameObject(currentGameObject);
						if (book.IsOpen())
							book.Close();
						else if (book.IsClosed())
							book.Open();
					}
					else if (Input.GetMouseButtonDown(1))
					{
						var book = GameObjectCollection.GetBookByGameObject(currentGameObject);
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
						var book = GameObjectCollection.GetBookByGameObject(currentGameObject);
						book.NextPage();
					}
					break;
				case CommandTagEnum.BookPrevious:
					if (Input.GetMouseButtonDown(0))
					{
						var book = GameObjectCollection.GetBookByGameObject(currentGameObject);
						book.PreviousPage();
					}
					break;
				case CommandTagEnum.Task:
					if (Input.GetMouseButtonDown(0))
					{
						var task = GameObjectCollection.GetTaskByGameObject(currentGameObject);
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
						var task = GameObjectCollection.GetTaskByGameObject(currentGameObject);
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
							playerBoards.AddTask(newTask, currentGameObject);
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
						GameObjectCollection.Phone.ShowDialog();
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
						var avatar = GameObjectCollection.AvatarsCollection.Get(currentGameObject);
						if (!avatar.IsDummy)
							avatar.ShowDialog();
					}
					break;
				case CommandTagEnum.Button:
					if (Input.GetMouseButtonDown(0))
					{
						var taggedGameObject = currentGameObject;
						while (!Tags.Keys.Contains(taggedGameObject.tag) || (Tags[taggedGameObject.tag] != CommandTagEnum.Avatar && Tags[taggedGameObject.tag] != CommandTagEnum.Phone))
							taggedGameObject = taggedGameObject.transform.parent.gameObject;

						if (Tags[taggedGameObject.tag] == CommandTagEnum.Phone)
						{
							GameObjectCollection.Phone.ButtonClicked(currentGameObject);
						}
						else
						{ 
							var avatar = GameObjectCollection.AvatarsCollection.Get(taggedGameObject);
							avatar.ButtonClicked(currentGameObject);
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
				if(currentToolTipGameObject != null)
					currentToolTipGameObject.SetActive(false);
                TextActive = false;
            }
		}

        //Draw the ray as a colored line for debugging purposes.
        Debug.DrawRay(ray.origin, ray.direction * Reach, DebugRayColor);
    }

	private GameObject GetToolTip(GameObject currentGameObject)
	{
		var childName = "ToolTip";
		var toolTipTransform = currentGameObject.transform.Find(childName);
		if (toolTipTransform != null)
			return toolTipTransform.gameObject;
		toolTipTransform = currentGameObject.transform.parent.Find(childName);
		return toolTipTransform?.gameObject;
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
		WasteBin,
		Avatar,
		Button
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
		{"WasteBin", CommandTagEnum.WasteBin},
		{"Avatar", CommandTagEnum.Avatar},
		{"Button", CommandTagEnum.Button}
	};
}
