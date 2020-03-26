using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
	/// <summary>
	/// Creates a Command Dialog.
	/// </summary>
	public class DialogCreator : MonoBehaviour
	{
		private static readonly Color BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);
		private static readonly Vector3 TitlePosition = new Vector3(1.0f, 0.45f, 0.0f);
		private static readonly Vector3 TitleScale = new Vector3(0.012f, 0.007f, 1.0f);
		private static readonly Vector3 ButtonScale = new Vector3(10.0f, 0.05f, 0.7f);
		private static readonly Vector3 FirstButtonPosition = new Vector3(0.0f, 0.4f, 0.0f);
		private static readonly Vector3 ButtonTextPosition = new Vector3(1.0f, 0.0f, 0.0f);
		private static readonly Quaternion ButtonTextRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
		private static readonly Vector3 ButtonTextScale = new Vector3(0.02f, 0.15f, 1.0f);
		private static readonly Vector2 ButtonTextSize = new Vector2(50, 5);
		private static readonly float ButtonYOffset = 0.06f;
		private static readonly string ButtonTag = "Button";
		private GameObject _dialog;
		private Dictionary<GameObject, object> _items;

		public void ShowSelectionDialog(string title, Dictionary<string, object> items, Vector3 dialogPosition, Vector3 dialogScale)
		{
			if (IsOpen)
				return;
			CreateDialog(title, dialogPosition, dialogScale);
			_items = new Dictionary<GameObject, object>();
			foreach (var key in items.Keys)
			{
				var button = CreateButton(key);
				_items.Add(button, items[key]);
			}

			var cancelButton = CreateButton("Cancel");
			_items.Add(cancelButton, null);
			IsOpen = true;
		}

		public void CloseDialog()
		{
			IsOpen = false;
			Destroy(_dialog);
		}

		public Dictionary<GameObject, object> GetButtons()
		{
			return _items;
		}

		private void CreateDialog(string title, Vector3 dialogPosition, Vector3 dialogScale)
		{
			_dialog = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_dialog.GetComponent<Renderer>().material.color = BackgroundColor;
			_dialog.transform.parent = transform;
			_dialog.transform.localPosition = dialogPosition;
			_dialog.transform.localScale = dialogScale;

			var titleTextObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			titleTextObject.transform.parent = _dialog.transform;
			titleTextObject.transform.localPosition = TitlePosition;

			var text = titleTextObject.AddComponent<TextMeshPro>();
			text.text = title;
			text.color = Color.black;
			text.alignment = TextAlignmentOptions.Center;

			titleTextObject.transform.rotation = ButtonTextRotation;
			titleTextObject.transform.localScale = TitleScale;
			titleTextObject.GetComponent<RectTransform>().sizeDelta = ButtonTextSize;

			var titleTextObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
			titleTextObject2.transform.parent = _dialog.transform;
			titleTextObject2.transform.localPosition = new Vector3(-TitlePosition.x, TitlePosition.y, TitlePosition.z);

			var text2 = titleTextObject2.AddComponent<TextMeshPro>();
			text2.text = title;
			text2.color = Color.black;
			text2.alignment = TextAlignmentOptions.Center;

			titleTextObject2.transform.rotation = Quaternion.Euler(ButtonTextRotation.x, 90.0f, ButtonTextRotation.z);
			titleTextObject2.transform.localScale = TitleScale;
			titleTextObject2.GetComponent<RectTransform>().sizeDelta = ButtonTextSize;
		}

		private GameObject CreateButton(string buttonText)
		{
			var count = _items.Count;
			var button = GameObject.CreatePrimitive(PrimitiveType.Cube);
			button.transform.parent = _dialog.transform;
			button.transform.localPosition = new Vector3(FirstButtonPosition.x, FirstButtonPosition.y - ButtonYOffset*count, FirstButtonPosition.z);
			button.transform.localScale = ButtonScale;
			button.tag = ButtonTag;

			var buttonTextObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			buttonTextObject.transform.parent = button.transform;
			buttonTextObject.transform.localPosition = ButtonTextPosition;
			buttonTextObject.tag = ButtonTag;

			var text = buttonTextObject.AddComponent<TextMeshPro>();
			text.text = buttonText;
			text.color = Color.black;
			text.alignment = TextAlignmentOptions.Center;

			buttonTextObject.transform.rotation = ButtonTextRotation;
			buttonTextObject.transform.localScale = ButtonTextScale;
			buttonTextObject.GetComponent<RectTransform>().sizeDelta = ButtonTextSize;

			var buttonTextObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
			buttonTextObject2.transform.parent = button.transform;
			buttonTextObject2.transform.localPosition = new Vector3(-ButtonTextPosition.x, ButtonTextPosition.y, ButtonTextPosition.z);
			buttonTextObject2.tag = ButtonTag;

			var text2 = buttonTextObject2.AddComponent<TextMeshPro>();
			text2.text = buttonText;
			text2.color = Color.black;
			text2.alignment = TextAlignmentOptions.Center;

			buttonTextObject2.transform.rotation = Quaternion.Euler(ButtonTextRotation.x, 90.0f, ButtonTextRotation.z);
			buttonTextObject2.transform.localScale = ButtonTextScale;
			buttonTextObject2.GetComponent<RectTransform>().sizeDelta = ButtonTextSize;
			return button;
		}

		public bool IsOpen { get; private set; }


	}
}
