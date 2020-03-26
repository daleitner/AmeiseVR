using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Keeps track of the queue of received Objects.
/// </summary>
public class MessageListener : MonoBehaviour
{
	private List<MessageObject> _receivedObjects;
	public delegate void MessageReceivedEventHandler(MessageObject messageObject);
	private bool sendDict = true;
	public event MessageReceivedEventHandler ReceivedMessage = null;
	// Use this for initialization
	void Start () {
		_receivedObjects = new List<MessageObject>();
		var client = ClientConnection.GetInstance();
		client.RegisterList(_receivedObjects);
	}
	
	// Update is called once per frame
	void Update () {
		if (_receivedObjects.Count > 0)
		{
			ReceivedMessage?.Invoke(_receivedObjects.First());
			_receivedObjects.RemoveAt(0);
		}
	}
}
