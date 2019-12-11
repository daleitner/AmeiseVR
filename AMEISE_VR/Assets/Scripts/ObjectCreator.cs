using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
	private bool _booksCreated;
	private bool _avatarsCreated;
	private bool _tasksCreated;
    // Start is called before the first frame update
    void Start()
    {
	    _booksCreated = false;
	    _avatarsCreated = false;
	    _tasksCreated = false;
    }

    // Update is called once per frame
    void Update()
    {
	    if (!_booksCreated && KnowledgeBase.Instance.LoadingCommandsFinished)
	    {
		    _booksCreated = true;
		    var bookTemplate = GameObjectCollection.Book;
		    var playerBoardTemplate = GameObjectCollection.PlayerBoard;
		    foreach (var employee in KnowledgeBase.Instance.Employees)
		    {
			    var gameObject = Instantiate(bookTemplate);
			    GameObjectCollection.AddBookToShelf(gameObject, employee, KnowledgeBase.Instance.DeveloperInformationCommand, new []{employee});

			    var newPlayerBoard = Instantiate(playerBoardTemplate);
				GameObjectCollection.AddPlayerBoardToWhiteBoard(newPlayerBoard, employee);
		    }

		    var resourceBook = Instantiate(bookTemplate);
			GameObjectCollection.AddResourceBook(resourceBook);
	    }

	    if (!_avatarsCreated && GameObjectCollection.AvatarsCollection != null)
	    {
		    _avatarsCreated = true;
		    var collection = GameObjectCollection.AvatarsCollection;
		    var avatarTemplate = GameObjectCollection.Avatar;
		    for (int i = 0; i < AvatarsCollection.MaxAvatars; i++)
		    {
			    var gameObject = Instantiate(avatarTemplate);
				collection.AddAvatar(gameObject);
		    }
	    }

	    if (!_tasksCreated && KnowledgeBase.Instance.LoadingCommandsFinished)
	    {
		    _tasksCreated = true;
		    var taskTemplate = GameObjectCollection.Task;
		    foreach (var command in KnowledgeBase.Instance.EmployeeCommands)
		    {
			    var gameObject = Instantiate(taskTemplate);
			    GameObjectCollection.AddTaskToWhiteBoard(gameObject, command);
		    }
	    }
    }
}
