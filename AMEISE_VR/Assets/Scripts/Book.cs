using System.Linq;
using Boo.Lang;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
	public class Book : GameObjectModelBase
	{
		private const int LineLength = 26;
		private const int PageSize = 286;
		private readonly Animator _anim;
		private readonly MessageListener _listener;
		private readonly TextMeshPro _text;
		private readonly TextMeshPro _title;
		private readonly GameObject _nextButton;
		private readonly GameObject _backButton;
		private Command _command;
		private string[] _parameters;
		private List<string> _pages;
		private string _fullText = string.Empty;
		private int _currentPage = 0;
		private bool _isHistoryBook;
		public Book(GameObject book, MessageListener listener, bool isHistoryBook = false)
			:base(book)
		{
			_isHistoryBook = isHistoryBook;
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
			if (_command != null)
			{
				_listener.ReceivedMessage += _listener_ReceivedMessage;
				ClientConnection.GetInstance().SendCommand(_command, _parameters);
				SetText("Loading...");
			}

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
			_fullText = text;
			FillPages(text);
			if (_isHistoryBook)
			{
				_nextButton.SetActive(false);
				_backButton.SetActive(_pages.Count > 1);
			}
			else
			{
				_nextButton.SetActive(_pages.Count > 1);
				_backButton.SetActive(false);
			}

			SetText();
		}

		public void AppendText(string text)
		{
			SetText(_fullText + " " + text);
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
			
			var currentPage = "";
			var count = 0;
			for (int i = 0; i < text.Length; i++)
			{
				currentPage += text[i];
				if (text[i] == '\n')
					count += LineLength;
				else
					count++;

				if (count >= PageSize)
				{
					var lastWord = currentPage.Split(' ').Last();
					
					_pages.Add(currentPage.Substring(0, currentPage.Length - lastWord.Length - 1).Trim());
					currentPage = lastWord;
					count = lastWord.Length;
				}
			}
			if(count > 0)
				_pages.Add(currentPage);
			if (_isHistoryBook)
				_currentPage = _pages.Count - 1;
			else
				_currentPage = 0;
		}

		private void SetText()
		{
			_text.text = _pages[_currentPage];
		}
	}
}
