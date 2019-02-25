using UnityEngine;

public class MainController : ControllerBase
{
	private GameObject _view;
	public MainController(GameObject view, SmartPhoneManager manager)
		:base(manager)
	{
		_view = view;
	}

	public override bool Accepts(string tag)
	{
		return tag == Tags.ChatTag;
	}

	public override void Execute(GameObject gameObject)
	{
		if (gameObject.tag == Tags.ChatTag)
			OpenChat();
	}

	private void OpenChat()
	{
		Manager.Show(ScreenEnum.ChatListScreen);
	}
}