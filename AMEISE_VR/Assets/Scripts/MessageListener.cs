using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
			if (ReceivedMessage != null)
				ReceivedMessage(_receivedObjects.First());
			_receivedObjects.RemoveAt(0);
		}
		if(Input.GetKey(KeyCode.Return) && sendDict)
		{
			sendDict = false;
			var msg = "DictionaryAndParameter|command11:deliver system|command33:specify|command10:customer perform acceptance test|command32:show me tasks of developer|command31:show me system design|command30:show me specification|command15:finish project|command37:test system|command14:finish activity|command36:test modules|command13:design system|command35:test integration|command12:design modules|command34:talk customer|command1:correct all documents|command2:correct documentation|command0:code|command9:customer participate in review specification|command19:information about spent resources|command18:information about developer|command7:correct tested code|command17:hire|command39:two developers review|command8:customer participate in review documentation|command16:finish review document|command38:three developers review|command5:correct specification|command6:correct system design|command3:correct module design|command4:correct reviewed code|command22:release|command21:quit activity|command20:integrate|command41:write documentation|command26:show me documentation|command25:show me code|command24:show me available developers|command23:show me activity|count:42|command40:use notation|command29:show me review report|command28:show me my team members|command27:show me module design;";
			var obj = new MessageObject(msg);
			_receivedObjects.Add(obj);
		}
	}
}
