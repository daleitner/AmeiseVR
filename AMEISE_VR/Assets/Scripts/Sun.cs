using Assets.Scripts;
using UnityEngine;

/// <summary>
/// Sun is a component of the Sun and the Moon game object.
/// Handles the day/night animation
/// </summary>
public class Sun : MonoBehaviour
{
	private bool _sunGoesDown;
	private bool _previousSunGoesDown;
    // Start is called before the first frame update
    void Start()
    {
	    _sunGoesDown = true;
    }

    // Update is called once per frame
    void Update()
    {
	    if (KnowledgeBase.Instance.ContinueTime)
	    {
		    _previousSunGoesDown = _sunGoesDown;
		    var previousY = transform.localPosition.y;
		    transform.RotateAround(new Vector3(287.0f, 0.0f, 374.2f), Vector3.back, 80f * Time.deltaTime);
		    transform.LookAt(new Vector3(287.0f, 0.0f, 374.2f));
		    _sunGoesDown = transform.localPosition.y - previousY <= 0;
		    if (transform.name == "sun" && !_previousSunGoesDown && _sunGoesDown)
		    {
			    KnowledgeBase.Instance.ContinueTime = false;
			    GameObjectCollection.Player.UnlockCursor();
			    GameObjectCollection.Player.enabled = true;
			}
	    }
    }
}
