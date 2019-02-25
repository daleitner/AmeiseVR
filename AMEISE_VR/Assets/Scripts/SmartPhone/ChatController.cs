using UnityEngine;

public class ChatController : ControllerBase
{
	private GameObject _view;
	public ChatController(GameObject view, SmartPhoneManager manager)
		: base(manager)
	{
		_view = view;
	}

	public override bool Accepts(string tag)
	{
		return false;
	}

	public override void Execute(string tag)
	{
		
	}
}