using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Detection is a component of the 1st person player.
/// It draws the crosshair, initializes the GameObjectCollection and handles the user interaction.
/// </summary>
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
	public GameObject Canvas;
	public GameObject Office;
	public GameObject Task;
	public GameObject PlayerBoard;
	public GameObject SpeechBubble;
	public GameObject Phone;
	public GameObject VRPlayer;
	public GameObject Teleporting;
	public GameObject GameController;
	public GameObject LoginGameObject;

	private GameConfiguration config;

	private GameObject currentToolTipGameObject;

	// DEBUG SETTINGS
	[Header("Debug Settings")]
    [Tooltip("The color of the debugray that will be shown in the scene view window when playing the game.")]
    public Color DebugRayColor;
    [Tooltip("The opacity of the debugray.")]
    [Range(0.0F, 1.0F)]
    public float Opacity = 1.0F;

    void Start()
    {
		//disable vr-player
	    Screen.orientation = ScreenOrientation.AutoRotation;
	    XRSettings.enabled = false;
		gameObject.name = "Player";
        gameObject.tag = "Player";

		//instantiate crosshair
        if (CrosshairPrefab == null)
	        Debug.Log("<color=yellow><b>No CrosshairPrefab was found.</b></color>"); // Return an error if no crosshair was specified
        else
        {
            CrosshairPrefabInstance = Instantiate(CrosshairPrefab); // Display the crosshair prefab
            CrosshairPrefabInstance.transform.SetParent(transform, true); // Make the player the parent object of the crosshair prefab
        }
		
		DebugRayColor.a = Opacity; // Set the alpha value of the DebugRayColor
		//add game objects to GameObjectCollection
		GameObjectCollection.AddGameObject(FPSController, GameObjectEnum.FPSController);
		GameObjectCollection.AddGameObject(GameController, GameObjectEnum.GameController);
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
		GameObjectCollection.AddGameObject(VRPlayer, GameObjectEnum.VRPlayer);
		GameObjectCollection.AddGameObject(Teleporting, GameObjectEnum.Teleporting);
		GameObjectCollection.AddGameObject(LoginGameObject, GameObjectEnum.Login);
		GameObjectCollection.AddHistoryBook(Canvas);
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
			var isKnownTag = GameObjectCollection.Tags.ContainsKey(hit.collider.tag);

			//ignore user interaction if tag is unknown or player is not logged in and tag is door or vr toggle
			if (!isKnownTag || KnowledgeBase.Instance.ContinueTime || !KnowledgeBase.Instance.LoadingCommandsFinished &&
			    (GameObjectCollection.Tags[hit.collider.tag] == CommandTagEnum.Door || GameObjectCollection.Tags[hit.collider.tag] == CommandTagEnum.VRToggle))
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

			var currentTag = GameObjectCollection.Tags[hit.collider.tag];
			var currentGameObject = hit.transform.gameObject;
			InReach = true;
			
			currentToolTipGameObject = GetToolTip(currentGameObject);
			
			//show Tooltip
			if (TextActive == false)
			{
				if (currentToolTipGameObject != null)
				{
					currentToolTipGameObject.SetActive(true);
				}
				
				TextActive = true;
			}

			//handle user interaction
			switch (currentTag)
			{
				case CommandTagEnum.Door:
					// Get access to the 'Door' script attached to the object that was hit
					Door dooropening = currentGameObject.GetComponent<Door>();

					if (Input.GetMouseButtonDown(0))
					{
						// Open/close the door by running the 'Open' function found in the 'Door' script
						if (dooropening.RotationPending == false)
							StartCoroutine(hit.collider.GetComponent<Door>().Move());
					}
					break;
				case CommandTagEnum.Login:
					if (Input.GetMouseButtonDown(0) && !LoginControl.activeSelf && !LoginFailedControl.activeSelf && !GameSelectionControl.activeSelf)
					{
						if (config.LoggedIn && KnowledgeBase.Instance.LoadingCommandsFinished)
							GameObjectCollection.LoginGameObject.ShowQuitDialog();
						else
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
						while (!GameObjectCollection.Tags.Keys.Contains(taggedGameObject.tag) || 
						       (GameObjectCollection.Tags[taggedGameObject.tag] != CommandTagEnum.Avatar 
						        && GameObjectCollection.Tags[taggedGameObject.tag] != CommandTagEnum.Phone
						        && GameObjectCollection.Tags[taggedGameObject.tag] != CommandTagEnum.Login))
							taggedGameObject = taggedGameObject.transform.parent.gameObject;

						if (GameObjectCollection.Tags[taggedGameObject.tag] == CommandTagEnum.Phone)
						{
							GameObjectCollection.Phone.ButtonClicked(currentGameObject);
						}
						else if (GameObjectCollection.Tags[taggedGameObject.tag] == CommandTagEnum.Login)
						{
							GameObjectCollection.LoginGameObject.ButtonClicked(currentGameObject);
						}
						else
						{ 
							var avatar = GameObjectCollection.AvatarsCollection.Get(taggedGameObject);
							avatar.ButtonClicked(currentGameObject);
						}
					}
					break;
				case CommandTagEnum.Arrow:
					if (Input.GetMouseButtonDown(0))
					{
						var message = new MessageObject(MessageTypeEnum.Proceed,
							new Dictionary<string, string> { { "steps", "1" } });
						var connection = ClientConnection.GetInstance();
						connection.SendText(message);
						KnowledgeBase.Instance.ContinueTime = true;
						GameObjectCollection.Player.LockCursor();
						GameObjectCollection.Player.enabled = false;
					}
					break;
				case CommandTagEnum.VRToggle:
					if(Input.GetMouseButtonDown(0))
						VRToggle.ToggleVR();
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

	/// <summary>
	/// Find a child or a sibling game object of currentGameObject with the name ToolTip.
	/// </summary>
	/// <param name="currentGameObject"></param>
	/// <returns></returns>
	private GameObject GetToolTip(GameObject currentGameObject)
	{
		var childName = "ToolTip";
		var toolTipTransform = currentGameObject.transform.Find(childName);
		if (toolTipTransform != null)
			return toolTipTransform.gameObject;
		toolTipTransform = currentGameObject.transform.parent.Find(childName);
		return toolTipTransform?.gameObject;
	}
}
