using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
	private bool _booksCreated;
    // Start is called before the first frame update
    void Start()
    {
	    _booksCreated = false;
    }

    // Update is called once per frame
    void Update()
    {
	    if (!_booksCreated && KnowledgeBase.Instance.LoadingCommandsFinished)
	    {
		    _booksCreated = true;
		    var bookTemplate = GameObjectCollection.Book;
		    foreach (var employee in KnowledgeBase.Instance.Employees)
		    {
			    var gameObject = Instantiate(bookTemplate);
			    GameObjectCollection.AddBook(gameObject, employee, KnowledgeBase.Instance.DeveloperInformationCommand, new []{employee});
		    }
	    }
    }
}
