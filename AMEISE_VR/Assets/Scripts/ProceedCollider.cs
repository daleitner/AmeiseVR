using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ProceedCollider : MonoBehaviour {

	private void OnTriggerEnter(Collider other)
	{
		var message = new MessageObject(MessageTypeEnum.Proceed, new Dictionary<string, string> { { "steps", "1" } });
		var connection = ClientConnection.GetInstance();
		connection.SendText(message);
		KnowledgeBase.Instance.ContinueTime = true;
		GameObjectCollection.Player.LockCursor();
		GameObjectCollection.Player.enabled = false;
	}
}
