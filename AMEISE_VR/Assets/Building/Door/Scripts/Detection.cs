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
	public GameObject HistoryControl;
	public GameObject CommandControl;
	public GameObject Book;
	public GameObject Office;
	public GameObject Avatar;

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
		GameObjectCollection.AddGameObject(HistoryControl, GameObjectEnum.HistoryControl);
		GameObjectCollection.AddGameObject(CommandControl, GameObjectEnum.CommandControl);
		GameObjectCollection.AddGameObject(Book, GameObjectEnum.Book);
		GameObjectCollection.AddGameObject(Avatar, GameObjectEnum.Avatar);
		GameObjectCollection.AddGameObject(Office, GameObjectEnum.Office);
		config = new GameConfiguration();
	}

	void Update()
    {
        // Set origin of ray to 'center of screen' and direction of ray to 'cameraview'
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0F));

        RaycastHit hit; // Variable reading information about the collider hit
		if (Input.GetKey("7"))
		{
			config.OpenHistoryDialog();
		}
		else if (Input.GetKey(KeyCode.Escape))
			config.CloseHistoryDialog();
		else if (Input.GetKey("8"))
			config.OpenCommandDialog();
		else if (Input.GetKey("9"))
			config.CloseCommandDialog();

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

                if (Input.GetMouseButton(0))
                {
                    // Open/close the door by running the 'Open' function found in the 'Door' script
                    if (dooropening.RotationPending == false) StartCoroutine(hit.collider.GetComponent<Door>().Move());
                }
            }
	        else if (hit.collider.tag == "Login")
	        {
		        InReach = true;

		        // Display the UI element when the player is in reach of the door
		        if (TextLoginActive == false && TextPrefabLogin != null)
		        {
			        TextPrefabLoginInstance = Instantiate(TextPrefabLogin);
			        TextLoginActive = true;
			        TextPrefabLoginInstance.transform.SetParent(transform, true); // Make the player the parent object of the text element
		        }

		        if (Input.GetMouseButton(0) && !LoginControl.activeSelf && !config.LoggedIn)
		        {
					config.OpenLoginDialog();
		        }
	        }
            else if (hit.collider.tag == "Book")
            {
	            InReach = true;

	            if (Input.GetMouseButton(0))
	            {
		            if (!_mouseClicked)
		            {
			            _mouseClicked = true;
			            var book = GameObjectCollection.GetBookByGameObject(hit.transform.gameObject);
			            if (book.IsOpen())
				            book.Close();
			            else if (book.IsClosed())
				            book.Open();
		            }
	            }
	            else if (Input.GetMouseButton(1))
	            {
		            var book = GameObjectCollection.GetBookByGameObject(hit.transform.gameObject);
		            if (!_mouseClicked && book.BelongsToAShelf)
		            {
			            _mouseClicked = true;
			            book.TriggerShelfMove();
					}
	            }
	            else
	            {
		            _mouseClicked = false;
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
