using UnityEngine;

namespace Assets.Scripts
{
	public class Avatar : GameObjectModelBase
	{
		private const string DefaultText = "Loading...";
		private SpeechBubble _speechBubble;
		public Avatar(GameObject avatar)
			:base(avatar)
		{
		}

		public string Name { get; private set; }

		public bool IsDummy => string.IsNullOrEmpty(Name) && !IsSecretary;
		public bool IsSecretary { get; private set; }

		public void AssignEmployee(string name, GameObject speechBubble)
		{
			Name = name;
			CreateSpeechBubble(speechBubble);
			HideBubble();
		}

		private void CreateSpeechBubble(GameObject speechBubble)
		{
			_speechBubble = new SpeechBubble(speechBubble);
			_speechBubble.SetParent(GameObject);
			_speechBubble.MoveTo(new Vector3(0.0f, 2.0f, 0.0f));
		}

		public void SetText(string text)
		{
			_speechBubble.SetText(text);
			_speechBubble.Show();
		}

		public void HideBubble()
		{
			_speechBubble.Hide();
			_speechBubble.SetText(DefaultText);
		}

		public void AssignSecretary(GameObject speechBubble)
		{
			IsSecretary = true;
			CreateSpeechBubble(speechBubble);
			HideBubble();
		}

		public void ShowBubble()
		{
			SetText(DefaultText);
		}
	}
}
