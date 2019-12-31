using Boo.Lang;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
	public class Book : GameObjectModelBase
	{
		private const int PageSize = 100;
		private readonly Animator _anim;
		private readonly MessageListener _listener;
		private readonly TextMeshPro _text;
		private readonly TextMeshPro _title;
		private readonly GameObject _nextButton;
		private readonly GameObject _backButton;
		private Command _command;
		private string[] _parameters;
		private List<string> _pages;
		private int _currentPage = 0;
		public Book(GameObject book, MessageListener listener)
			:base(book)
		{
			_anim = book.GetComponent<Animator>();
			_title = book.transform.Find("BookFront").Find("BookTitleField").Find("BookTitle").GetComponent<TextMeshPro>();
			_text = book.transform.Find("BookSheet").Find("Content").GetComponent<TextMeshPro>();
			_nextButton = book.transform.Find("NextField").gameObject;
			_backButton = book.transform.Find("BackField").gameObject;
			_nextButton.SetActive(false);
			_backButton.SetActive(false);
			_listener = listener;
			_anim.SetTrigger("Take");
			_pages = new List<string>();
		}
		
		public bool IsOpen()
		{
			return _anim.GetCurrentAnimatorStateInfo(0).IsName("BookOpen");
		}

		public bool IsClosed()
		{
			return _anim.GetCurrentAnimatorStateInfo(0).IsName("BookIdle");
		}

		public bool IsInShelf { get; private set; }
		public bool BelongsToAShelf { get; private set; }

		public void Open()
		{
			_listener.ReceivedMessage += _listener_ReceivedMessage;
			ClientConnection.GetInstance().SendCommand(_command, _parameters);
			SetText("Loading...");

			_anim.SetTrigger("Open");
		}

		private void _listener_ReceivedMessage(MessageObject messageObject)
		{
			if (messageObject.Type == MessageTypeEnum.Feedback)
			{
				_listener.ReceivedMessage -= _listener_ReceivedMessage;
				SetText(messageObject.GetValueOf("feedback"));
			}
		}

		public void Close()
		{
			_anim.SetTrigger("Close");
		}

		public void SetText(string text)
		{
			FillPages(text);
			_nextButton.SetActive(_pages.Count > 1);
			_backButton.SetActive(false);
			SetText();
		}

		public void NextPage()
		{
			if (_currentPage < _pages.Count - 1)
			{
				_currentPage++;
				_backButton.SetActive(true);
				_nextButton.SetActive(_currentPage < _pages.Count - 1);
				SetText();
			}
		}

		public void PreviousPage()
		{
			if (_currentPage > 0)
			{
				_currentPage--;
				_backButton.SetActive(_currentPage > 0);
				_nextButton.SetActive(true);
				SetText();
			}
		}

		public void SetTitle(string title)
		{
			_title.text = title;
		}

		public void SetCommand(Command command, string[] parameters)
		{
			_command = command;
			_parameters = parameters;
		}

		private void PullOutFromShelf()
		{
			if (!IsInShelf)
				return;
			GameObject.transform.localPosition = new Vector3(GameObject.transform.localPosition.x, GameObject.transform.localPosition.y, GameObject.transform.localPosition.z - 0.5f);
			IsInShelf = false;
			_anim.SetTrigger("Take");
		}

		private void PushInToShelf()
		{
			if (IsInShelf)
				return;
			GameObject.transform.localPosition = new Vector3(GameObject.transform.localPosition.x, GameObject.transform.localPosition.y, GameObject.transform.localPosition.z + 0.5f);
			IsInShelf = true;
			_anim.SetTrigger("Put");
		}

		public void TriggerShelfMove()
		{
			if(IsInShelf)
				PullOutFromShelf();
			else if(IsClosed())
			{
				PushInToShelf();
			}
		}

		public void SetShelf(GameObject shelf)
		{
			SetParent(shelf);
			IsInShelf = true;
			BelongsToAShelf = true;
			_anim.SetTrigger("Put");
		}

		private void FillPages(string text)
		{
			_pages = new List<string>();
			var pageCount = text.Length / PageSize;
			for (int i = 0; i < pageCount; i++)
			{
				_pages.Add(text.Substring(i * PageSize, PageSize));
			}

			var lastPageSize = text.Length % PageSize;
			if (lastPageSize > 0)
				_pages.Add(text.Substring(pageCount * PageSize));
			_currentPage = 0;
		}

		private void SetText()
		{
			_text.text = _pages[_currentPage];
		}
	}
}
