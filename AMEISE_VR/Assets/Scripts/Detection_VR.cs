using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Valve.VR.Extras;

public class Detection_VR : SteamVR_LaserPointer
{
	public bool isRightHand;
	public override void OnPointerClick(PointerEventArgs e)
	{
		base.OnPointerClick(e);
		var currentGameObject = e.target.gameObject;
		var isKnownTag = GameObjectCollection.Tags.ContainsKey(currentGameObject.tag);
		if (!isKnownTag)
			return;

		var currentTag = GameObjectCollection.Tags[currentGameObject.tag];
		switch (currentTag)
		{
			case CommandTagEnum.Door:
				// Get access to the 'Door' script attached to the object that was hit
				Door dooropening = currentGameObject.GetComponent<Door>();
				
				// Open/close the door by running the 'Open' function found in the 'Door' script
				if (dooropening.RotationPending == false)
					StartCoroutine(dooropening.Move());
				
				break;
			case CommandTagEnum.Login:
			//	if (config.LoggedIn)
			//		break;

			//	if (Input.GetMouseButtonDown(0) && !LoginControl.activeSelf && !LoginFailedControl.activeSelf)
			//	{
			//		config.OpenLoginDialog();
			//	}
				break;
			case CommandTagEnum.Book:

				if (!isRightHand)
				{
					var book = GameObjectCollection.GetBookByGameObject(currentGameObject);
					if (book.IsOpen())
						book.Close();
					else if (book.IsClosed())
						book.Open();
				}
				else
				{
					var book = GameObjectCollection.GetBookByGameObject(currentGameObject);
					if (book.BelongsToAShelf)
					{
						book.TriggerShelfMove();
					}
					else
					{
						book.TriggerRotation();
					}
				}
				break;
			case CommandTagEnum.BookNext:
				
					var book1 = GameObjectCollection.GetBookByGameObject(currentGameObject);
					book1.NextPage();
				
				break;
			case CommandTagEnum.BookPrevious:
				
					var book2 = GameObjectCollection.GetBookByGameObject(currentGameObject);
					book2.PreviousPage();
				
				break;
			case CommandTagEnum.Task:
				if (!isRightHand)
				{
					var task = GameObjectCollection.GetTaskByGameObject(currentGameObject);
					var playerBoards = GameObjectCollection.PlayerBoardCollection;
					var playerBoard = playerBoards.PlayerBoards.SingleOrDefault(p => p.Tasks.Contains(task));
					if (playerBoard != null)
					{
						playerBoard.RemoveTask(task);
						task.RelatedTasks.ForEach(t => t.RemoveRelatedTask(task));
						Destroy(task.GameObject);
					}
					else
					{
						GameObjectCollection.TaskBoard.SelectTask(task);
					}
				}
				else
				{
					var task = GameObjectCollection.GetTaskByGameObject(currentGameObject);
					task.ChangeParameter();
					task.RelatedTasks.ForEach(t => t.ChangeParameter());
				}
				break;
			case CommandTagEnum.PlayerBoard:
				
					var selectedTask = GameObjectCollection.TaskBoard.SelectedTask;
					if (selectedTask != null)
					{
						var playerBoards1 = GameObjectCollection.PlayerBoardCollection;
						var newTaskGameObject = Instantiate(GameObjectCollection.Task);
						var newTask = selectedTask.Clone(newTaskGameObject);
						playerBoards1.AddTask(newTask, currentGameObject);
					}
				
				break;
			case CommandTagEnum.SendCommand:
				
					GameObjectCollection.PlayerBoardCollection.SendCommands();
				
				break;
			case CommandTagEnum.Phone:
				
					GameObjectCollection.Phone.ShowDialog();
				
				break;
			case CommandTagEnum.WasteBin:
				
					ClientConnection.GetInstance().SendCommand(KnowledgeBase.Instance.CancelProjectCommand);
				
				break;
			case CommandTagEnum.Avatar:
				
					var avatar = GameObjectCollection.AvatarsCollection.Get(currentGameObject);
					if (!avatar.IsDummy)
						avatar.ShowDialog();
				
				break;
			case CommandTagEnum.Button:
				
					var taggedGameObject = currentGameObject;
					while (!GameObjectCollection.Tags.Keys.Contains(taggedGameObject.tag) || (GameObjectCollection.Tags[taggedGameObject.tag] != CommandTagEnum.Avatar && GameObjectCollection.Tags[taggedGameObject.tag] != CommandTagEnum.Phone))
						taggedGameObject = taggedGameObject.transform.parent.gameObject;

					if (GameObjectCollection.Tags[taggedGameObject.tag] == CommandTagEnum.Phone)
					{
						GameObjectCollection.Phone.ButtonClicked(currentGameObject);
					}
					else
					{
						var avatar1 = GameObjectCollection.AvatarsCollection.Get(taggedGameObject);
						avatar1.ButtonClicked(currentGameObject);
					}
				
				break;
			case CommandTagEnum.Arrow:
					var message = new MessageObject(MessageTypeEnum.Proceed,
						new Dictionary<string, string> { { "steps", "1" } });
					var connection = ClientConnection.GetInstance();
					connection.SendText(message);
					KnowledgeBase.Instance.ContinueTime = true;
					GameObjectCollection.Player.LockCursor();
					GameObjectCollection.Player.enabled = false;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}
