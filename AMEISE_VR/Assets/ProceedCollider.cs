using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceedCollider : MonoBehaviour {
	public GameObject FPSController;

	private void OnTriggerEnter(Collider other)
	{
		var message = new MessageObject(MessageTypeEnum.Proceed, new Dictionary<string, string> { { "steps", "1" } });
		var connection = ClientConnection.GetInstance();
		connection.SendText(message);
		FPSController.transform.position = new Vector3(230.0f, 21.0f, 163.0f);
	}
}
