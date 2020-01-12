using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class CommandButton : GameObjectModelBase
	{
		public delegate void ButtonClickedEventHandler(CommandButton button);

		public event ButtonClickedEventHandler ButtonClicked = null;
		public CommandButton(GameObject gameObject, Command command)
			:base(gameObject)
		{
			Command = command;
			var buttonText = GameObject.transform.Find("Text").GetComponent<Text>();
			buttonText.text = Command?.Name ?? "Cancel";
			var button = GameObject.GetComponent<Button>();
			button.onClick.AddListener(OnButtonClicked);
		}

		public Command Command { get; }

		public override void SetParent(GameObject parent)
		{
			base.SetParent(parent);
			GameObject.SetActive(true);
		}

		private void OnButtonClicked()
		{
			ButtonClicked?.Invoke(this);
		}
	}
}
