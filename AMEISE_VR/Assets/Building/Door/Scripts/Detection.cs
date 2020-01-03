using System.Linq;
using Assets.Scripts;
using UnityEngine;

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

	[Tooltip("The image or text that will be shown whenever the player is in reach of the login.")]
	public GameObject TextPrefabLogin; // A text element to display when the player is in reach of the login
	[HideInInspector] public GameObject TextPrefabLoginInstance; // A copy of the text prefab to prevent data corruption
	[HideInInspector] public bool TextLoginActive;

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
	    if (TextPrefabLogin == null) Debug.Log("<color=yellow><b>No TextPrefabLogin was found.</b></color>"); // Return an error if no text element was specified

		DebugRayColor.a = Opacity; // Set the alpha value of the DebugRayColor
		GameObjectCollection.AddGameObject(FPSController, GameObjectEnum.FPSController);
		GameObjectCollection.AddGameObject(LoginControl, GameObjectEnum.LoginControl);
		GameObjectCollection.AddGameObject(GameSelectionControl, GameObjectEnum.GameSelectionControl);
		GameObjectCollection.AddGameObject(LoginFailedControl, GameObjectEnum.LoginFailedControl);
		GameObjectCollection.AddGameObject(CommandControl, GameObjectEnum.CommandControl);
		GameObjectCollection.AddGameObject(Book, GameObjectEnum.Book);
		GameObjectCollection.AddGameObject(Avatar, GameObjectEnum.Avatar);
		GameObjectCollection.AddGameObject(Office, GameObjectEnum.Office);
		GameObjectCollection.AddGameObject(Task, GameObjectEnum.Task);
		GameObjectCollection.AddGameObject(PlayerBoard, GameObjectEnum.PlayerBoard);
		GameObjectCollection.AddGameObject(LoginText, GameObjectEnum.LoginText);
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
		else if (Input.GetKey(KeyCode.X))
			KnowledgeBase.Instance.ContinueTime = true;
		// Cast ray from center of the screen towards where the player is looking
		if (Physics.Raycast(ray, out hit, Reach))
        {
            if (hit.collider.tag == "Door")
            {
                InReach = true;

                // Display the UI element when the player is in reach of the door
                if (TextActive == false && TextPrefab != null)
                {
                    TextPrefabInstance = Instantiate(TextPrefab);
                    TextActive = true;
                    TextPrefabInstance.transform.SetParent(transform, true); // Make the player the parent object of the text element
                }

                // Give the object that was hit the name 'Door'
                GameObject Door = hit.transform.gameObject;

                // Get access to the 'Door' script attached to the object that was hit
                Door dooropening = Door.GetComponent<Door>();

                if (Input.GetMouseButtonDown(0))
                {
                    // Open/close the door by running the 'Open' function found in the 'Door' script
                    if (dooropening.RotationPending == false) StartCoroutine(hit.collider.GetComponent<Door>().Move());
                }
            }
	        else if (hit.collider.tag == "Login" && !config.LoggedIn)
	        {
		        InReach = true;

		        // Display the UI element when the player is in reach of the door
		        if (TextLoginActive == false && TextPrefabLogin != null)
		        {
			        TextPrefabLoginInstance = Instantiate(TextPrefabLogin);
			        TextLoginActive = true;
			        TextPrefabLoginInstance.transform.SetParent(transform, true); // Make the player the parent object of the text element
		        }

		        if (Input.GetMouseButtonDown(0) && !LoginControl.activeSelf && !LoginFailedControl.activeSelf)
		        {
					config.OpenLoginDialog();
		        }
	        }
            else if (hit.collider.tag == "Book")
            {
	            InReach = true;

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
	            }
            }
            else if (hit.collider.tag == "BookNext")
            {
	            InReach = true;

	            if (Input.GetMouseButtonDown(0))
	            {
		            var book = GameObjectCollection.GetBookByGameObject(hit.transform.gameObject);
			        book.NextPage();
	            }
	           
            }
            else if (hit.collider.tag == "BookBack")
            {
	            InReach = true;

	            if (Input.GetMouseButtonDown(0))
	            {
		            var book = GameObjectCollection.GetBookByGameObject(hit.transform.gameObject);
		            book.PreviousPage();
	            }

            }
			else if (hit.collider.tag == "Task")
            {
	            InReach = true;
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
            }
            else if(hit.collider.tag == "PlayerBoard")
            {
	            InReach = true;
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
            }
			else if (hit.collider.tag == "SendCommand")
            {
	            InReach = true;
	            if (Input.GetMouseButtonDown(0))
	            {
		            GameObjectCollection.PlayerBoardCollection.SendCommands();
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

	            // Destroy the UI element when Player is no longer in reach of the login
	            if (TextLoginActive == true)
	            {
		            DestroyImmediate(TextPrefabLoginInstance);
		            TextLoginActive = false;
	            }
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

	        // Destroy the UI element when Player is no longer in reach of the login
	        if (TextLoginActive == true)
	        {
		        DestroyImmediate(TextPrefabLoginInstance);
		        TextLoginActive = false;
	        }
		}

        //Draw the ray as a colored line for debugging purposes.
        Debug.DrawRay(ray.origin, ray.direction * Reach, DebugRayColor);
    }
}
