using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using Valve.VR.Extras;

/// <summary>
/// Detection_VR is a component of the hand of the VR-Player.
/// It handles the user interactions.
/// </summary>
public class Detection_VR : SteamVR_LaserPointer
{
	//isRightHand is set if the owning game object is the right hand of the player.
	//The right hand triggers the right mouse click, the left hand triggers the left mouse click.
	public bool isRightHand;

	/// <summary>
	/// OnPointerClick is called when the trigger button on the controller is clicked.
	/// </summary>
	/// <param name="e"></param>
	public override void OnPointerClick(PointerEventArgs e)
	{
		base.OnPointerClick(e);
		var currentGameObject = e.target.gameObject;
		var isKnownTag = GameObjectCollection.Tags.ContainsKey(e.collider.tag);
		//cancel user interaction when proceed animation is running
		if (!isKnownTag || KnowledgeBase.Instance.ContinueTime)
			return;

		var currentTag = GameObjectCollection.Tags[e.collider.tag];
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
				if (KnowledgeBase.Instance.LoadingCommandsFinished)
					GameObjectCollection.LoginGameObject.ShowQuitDialog();
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
			case CommandTagEnum.VRToggle:
				VRToggle.ToggleVR();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	/// <summary>
	/// OnPointerIn is called when the laser pointer collides with a game object.
	/// It is used to show a tooltip if exists.
	/// </summary>
	/// <param name="e"></param>
	public override void OnPointerIn(PointerEventArgs e)
	{
		base.OnPointerIn(e);
		var isKnownTag = GameObjectCollection.Tags.ContainsKey(e.collider.tag);
		if (!isKnownTag)
			return;

		var currentGameObject = e.target.gameObject;
		var currentToolTipGameObject = GetToolTip(currentGameObject);
		if(currentToolTipGameObject != null)
			currentToolTipGameObject.SetActive(true);
	}

	/// <summary>
	/// OnPointerOut is called when the laser pointer leaves a colliding game object.
	/// It is used to hide a tooltip if exists.
	/// </summary>
	/// <param name="e"></param>
	public override void OnPointerOut(PointerEventArgs e)
	{
		base.OnPointerOut(e);
		var isKnownTag = GameObjectCollection.Tags.ContainsKey(e.collider.tag);
		if (!isKnownTag)
			return;

		var currentGameObject = e.target.gameObject;
		var currentToolTipGameObject = GetToolTip(currentGameObject);
		if(currentToolTipGameObject != null)
			currentToolTipGameObject.SetActive(false);
	}

	/// <summary>
	/// Find a child or a sibling game object of currentGameObject with the name ToolTip.
	/// </summary>
	/// <param name="currentGameObject"></param>
	/// <returns></returns>
	private GameObject GetToolTip(GameObject currentGameObject)
	{
		var childName = "ToolTip";
		var toolTipTransform = currentGameObject.transform.Find(childName);
		if (toolTipTransform != null)
			return toolTipTransform.gameObject;
		toolTipTransform = currentGameObject.transform.parent.Find(childName);
		return toolTipTransform?.gameObject;
	}
}
