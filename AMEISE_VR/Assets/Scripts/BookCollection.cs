using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public class BookCollection
	{
		private readonly GameObject _shelf;
		private readonly Vector3 DefaultPosition = new Vector3(0.01f, 0.35f, -0.25f);
		private readonly Vector3 DefaultScale = new Vector3(0.85f, 1.0f, 1.7f);
		private readonly Quaternion DefaultRotation = Quaternion.Euler(90.0f, 180.0f, 90.0f);
		private const float x_offset = 0.2f;
		public BookCollection(GameObject shelf)
		{
			_shelf = shelf;
			Books = new List<Book>();
		}

		public void AddBook(Book book)
		{
			book.SetShelf(_shelf);
			var position = DefaultPosition;
			position.x += x_offset * Books.Count;
			book.MoveTo(position);
			book.Rotate(DefaultRotation);
			book.Scale(DefaultScale);
			Books.Add(book);
		}

		public List<Book> Books { get; }
	}
}
