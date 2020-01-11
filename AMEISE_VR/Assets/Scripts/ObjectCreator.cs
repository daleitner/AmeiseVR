using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
	private bool _initialCreationDone;
	// Start is called before the first frame update
	void Start()
	{
		_initialCreationDone = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (_initialCreationDone)
			return;

		if (!KnowledgeBase.Instance.LoadingCommandsFinished)
			return;

		_initialCreationDone = true;

		CreateBooks();

		CreateAvatars();

		CreateTasks();

		CreateButtons();
	}

	private static void CreateButtons()
	{
		var buttonTemplate = GameObjectCollection.Button;

	}

	private static void CreateTasks()
	{
		var taskTemplate = GameObjectCollection.Task;
		foreach (var command in KnowledgeBase.Instance.WhiteBoardCommands)
		{
			var gameObject = Instantiate(taskTemplate);
			GameObjectCollection.AddTaskToWhiteBoard(gameObject, command);
		}
	}

	private static void CreateAvatars()
	{
		var collection = GameObjectCollection.AvatarsCollection;
		var avatarTemplate = GameObjectCollection.Avatar;
		for (int i = 0; i < AvatarsCollection.MaxAvatars; i++)
		{
			var gameObject = Instantiate(avatarTemplate);
			collection.AddAvatar(gameObject);
		}
	}

	private static void CreateBooks()
	{
		var bookTemplate = GameObjectCollection.Book;
		var playerBoardTemplate = GameObjectCollection.PlayerBoard;
		foreach (var employee in KnowledgeBase.Instance.Employees)
		{
			var gameObject = Instantiate(bookTemplate);
			GameObjectCollection.AddBookToShelf(gameObject, employee, KnowledgeBase.Instance.DeveloperInformationCommand,
				new[] {employee});

			var newPlayerBoard = Instantiate(playerBoardTemplate);
			GameObjectCollection.AddPlayerBoardToWhiteBoard(newPlayerBoard, employee);
		}

		var resourceBook = Instantiate(bookTemplate);
		GameObjectCollection.AddResourceBook(resourceBook);
	}
}
