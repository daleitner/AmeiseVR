using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Scripts
{
	public enum GameObjectEnum
	{
		LoginControl,
		GameSelectionControl,
		LoginFailedControl,
		FPSController,
		Book,
		HistoryControl,
		CommandControl,
		Office,
		Avatar,
		Task,
		PlayerBoard
	}

	public static class GameObjectCollection
	{
		private static GameObject FPSController;
		private static FirstPersonController player;
		
		public static LoginDialog LoginDialog { get; private set; }
		public static LoginFailedDialog LoginFailedDialog { get; private set; }
		public static GameSelectionDialog GameSelectionDialog { get; private set; }
		public static HistoryDialog HistoryDialog { get; private set; }
		public static CommandDialog CommandDialog { get; private set; }
		public static MessageListener MessageListener { get; private set; }
		public static GameObject Office { get; set; }
		public static GameObject Book { get; private set; }
		public static GameObject Task { get; private set; }
		public static GameObject Avatar { get; private set; }
		public static GameObject PlayerBoard { get; private set; }
		private static BookCollection Shelf;
		private static GameObject MyOfficeDesk;
		private static List<Book> AllBooks = new List<Book>();
		public static AvatarsCollection AvatarsCollection { get; private set; }
		public static TaskBoard TaskBoard { get; private set; }
		public static PlayerBoardCollection PlayerBoardCollection { get; private set; }

		public static void AddGameObject(GameObject gameObject, GameObjectEnum type)
		{
			switch (type)
			{
				case GameObjectEnum.LoginControl:
					LoginDialog = new LoginDialog(gameObject, player);
					break;
				case GameObjectEnum.GameSelectionControl:
					GameSelectionDialog = new GameSelectionDialog(gameObject, player);
					break;
				case GameObjectEnum.LoginFailedControl:
					LoginFailedDialog = new LoginFailedDialog(gameObject, player);
					break;
				case GameObjectEnum.FPSController:
					FPSController = gameObject;
					player = FPSController.GetComponent<FirstPersonController>();
					MessageListener = FPSController.GetComponent<MessageListener>();
					break;
				case GameObjectEnum.Book:
					Book = gameObject;
					break;
				case GameObjectEnum.Avatar:
					Avatar = gameObject;
					break;
				case GameObjectEnum.HistoryControl:
					HistoryDialog = new HistoryDialog(gameObject, player);
					break;
				case GameObjectEnum.CommandControl:
					CommandDialog = new CommandDialog(gameObject, player, HistoryDialog);
					break;
				case GameObjectEnum.Office:
					Office = gameObject;
					MyOfficeDesk = Office.transform.Find("MyOffice").Find("Desk").gameObject;
					AvatarsCollection = new AvatarsCollection(Office.transform.Find("Avatars").gameObject);
					var shelf = Office.transform.Find("MyOffice").Find("Shelf_05").Find("Shelf").gameObject;
					Shelf = new BookCollection(shelf);
					var whiteBoard = Office.transform.Find("MyOffice").Find("WhiteBoard");
					PlayerBoardCollection = new PlayerBoardCollection(whiteBoard.gameObject, MessageListener);
					TaskBoard = new TaskBoard(whiteBoard.Find("TaskBoard").gameObject);
					break;
				case GameObjectEnum.Task:
					Task = gameObject;
					break;
				case GameObjectEnum.PlayerBoard:
					PlayerBoard = gameObject;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		public static void MovePlayer(Vector3 position)
		{
			FPSController.transform.position = position;
		}

		public static void AddBookToShelf(GameObject newBookObject, string title, Command command, string[] parameters)
		{
			var newBook = new Book(newBookObject, MessageListener);
			newBook.SetTitle(title);
			newBook.SetCommand(command, parameters);
			Shelf.AddBook(newBook);
			AllBooks.Add(newBook);
		}

		public static void AddTaskToWhiteBoard(GameObject newTask, Command command)
		{
			var task = new Task(newTask, command);
			TaskBoard.AddTask(task);
		}

		public static void AddPlayerBoardToWhiteBoard(GameObject newPlayerboard, string employee)
		{
			var playerBoard = new PlayerBoard(newPlayerboard, employee);
			PlayerBoardCollection.AddPlayerBoard(playerBoard);
		}

		public static void AddResourceBook(GameObject newBookObject)
		{
			var newBook = new Book(newBookObject, MessageListener);
			newBook.SetTitle("Resources");
			newBook.SetCommand(KnowledgeBase.Instance.ResourceCommand, new string[]{});
			newBookObject.transform.parent = MyOfficeDesk.transform;
			newBook.MoveTo(new Vector3(0.7f, 0.814f, 0.0f));
			newBook.Rotate(Quaternion.Euler(0.0f, 0.0f, 0.0f));
			AllBooks.Add(newBook);
		}

		public static Book GetBookByGameObject(GameObject gameObject)
		{
			var book = AllBooks.SingleOrDefault(b => b.GameObject == gameObject);
			if (book == null)
				book = AllBooks.SingleOrDefault(b => b.GameObject == gameObject.transform.parent.gameObject);
			if (book == null)
				book = AllBooks.SingleOrDefault(b => b.GameObject == gameObject.transform.parent.parent.gameObject);
			return book;
		}

		public static Task GetTaskByGameObject(GameObject gameObject)
		{
			var allTasks = new List<Task>();
			foreach (var currentTask in TaskBoard.Tasks)
			{
				allTasks.Add(currentTask);
			}

			foreach (var playerBoard in PlayerBoardCollection.PlayerBoards)
			{
				foreach (var currentTask in playerBoard.Tasks)
				{
					allTasks.Add(currentTask);
				}
			}

			var task = allTasks.SingleOrDefault(t => t.GameObject == gameObject);
			if (task == null)
				task = allTasks.SingleOrDefault(t => t.GameObject == gameObject.transform.parent.gameObject);
			return task;
		}

		{
		}
	}
}
