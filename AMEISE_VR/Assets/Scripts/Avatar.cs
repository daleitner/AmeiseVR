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

		public bool IsDummy => string.IsNullOrEmpty(Name);

		public void AssignEmployee(string name, GameObject speechBubble)
		{
			Name = name;
			_speechBubble = new SpeechBubble(speechBubble);
			_speechBubble.SetParent(GameObject);
			_speechBubble.MoveTo(new Vector3(0.0f, 2.0f, 0.0f));
			//HideBubble();
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
	}
}
