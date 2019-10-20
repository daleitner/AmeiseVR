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
		CommandControl
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
		private static readonly List<Book> Books = new List<Book>();

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
					Books.Add(new Book(gameObject, MessageListener));
					break;
				case GameObjectEnum.HistoryControl:
					HistoryDialog = new HistoryDialog(gameObject, player);
					break;
				case GameObjectEnum.CommandControl:
					CommandDialog = new CommandDialog(gameObject, player, HistoryDialog);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		public static void MovePlayer(Vector3 position)
		{
			FPSController.transform.position = position;
		}

		public static Book GetBookByGameObject(GameObject gameObject)
		{
			var book = Books.SingleOrDefault(b => b.GameObject == gameObject);
			if (book == null)
				book = Books.SingleOrDefault(b => b.GameObject == gameObject.transform.parent.gameObject);
			if (book == null)
				book = Books.SingleOrDefault(b => b.GameObject == gameObject.transform.parent.parent.gameObject);
			return book;
		}
	}
}
