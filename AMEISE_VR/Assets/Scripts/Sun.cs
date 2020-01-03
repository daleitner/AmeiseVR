using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Sun : MonoBehaviour
{
	private Vector3 _rootPosition;

	private bool _isNight;
    // Start is called before the first frame update
    void Start()
    {
	    _rootPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
	    if (KnowledgeBase.Instance.ContinueTime)
	    {
		    transform.RotateAround(new Vector3(287.0f, 0.0f, 374.2f), Vector3.back, 40f * Time.deltaTime);
		    transform.LookAt(new Vector3(287.0f, 0.0f, 374.2f));
		    if (transform.localPosition.y < 0.0f)
			    _isNight = true;
		    if ( _isNight && Math.Abs(transform.localPosition.y - _rootPosition.y) < 0.01f)
		    {
			    _isNight = false;
			    KnowledgeBase.Instance.ContinueTime = false;
			    GameObjectCollection.Player.UnlockCursor();
			    GameObjectCollection.Player.enabled = true;
			}
	    }
    }
}
