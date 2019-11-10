using System;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
	public class Book
	{
		private readonly Animator _anim;
		private readonly MessageListener _listener;
		private readonly TextMeshPro _text;
		private readonly TextMeshPro _title;
		public Book(GameObject book, MessageListener listener)
		{
			GameObject = book;
			_anim = book.GetComponent<Animator>();
			_title = book.transform.Find("BookFront").Find("BookTitleField").Find("BookTitle").GetComponent<TextMeshPro>();
			_text = book.transform.Find("BookSheet").Find("Content").GetComponent<TextMeshPro>();
			_listener = listener;
		}

		public GameObject GameObject { get; }

		public bool IsOpen()
		{
			return _anim.GetCurrentAnimatorStateInfo(0).IsName("BookOpen");
		}

		public bool IsClosed()
		{
			return _anim.GetCurrentAnimatorStateInfo(0).IsName("BookIdle");
		}

		public void Open()
		{
			_listener.ReceivedMessage += _listener_ReceivedMessage;
			ClientConnection.GetInstance().SendCommand(KnowledgeBase.Instance.DeveloperInformationCommand,
				KnowledgeBase.Instance.Employees.First());
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
			_text.text = text;
		}

		public void SetTitle(string title)
		{
			_title.text = title;
		}

		public void MoveTo(Vector3 position)
		{
			GameObject.transform.localPosition = position;
		}

		public void Rotate(Quaternion rotation)
		{
			GameObject.transform.localRotation = rotation;
		}

		public void Scale(Vector3 scale)
		{
			GameObject.transform.localScale = scale;
		}
	}
}
