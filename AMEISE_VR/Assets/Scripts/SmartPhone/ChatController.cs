using TMPro;
using UnityEngine;

public class ChatController : ControllerBase
{
	private GameObject _view;
	private TextMeshPro _lblName;
	public ChatController(GameObject view, SmartPhoneManager manager)
		: base(manager)
	{
		_view = view;
		_lblName = _view.transform.Find("LblName").gameObject.GetComponent<TextMeshPro>();
	}

	public override bool Accepts(string tag)
	{
		return false;
	}

	public override void Execute(GameObject gameObject)
	{
		
	}

	public override void Activate(object payload)
	{
		var gameObject = payload as GameObject;
		if (gameObject == null)
			return;
		var helper = new EntryHelper(gameObject);
		_lblName.text = helper.GetName();
	}

	public override void Back()
	{
		Manager.Show(ScreenEnum.ChatListScreen);
	}
}