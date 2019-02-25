using UnityEngine;

public class ControllerFactory
{
	public static ControllerBase GetController(ScreenEnum controller, GameObject view, SmartPhoneManager manager)
	{
		switch (controller)
		{
			case ScreenEnum.MainScreen:
				return new MainController(view, manager);
			case ScreenEnum.ChatListScreen:
				return new ChatListController(view, manager);
			case ScreenEnum.ChatScreen:
				return new ChatController(view, manager);
		}
		return null;
	}
}