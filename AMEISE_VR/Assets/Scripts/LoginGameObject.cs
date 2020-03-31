using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	/// <summary>
	/// Represents the Game Object where user can login and quit.
	/// </summary>
	public class LoginGameObject : GameObjectModelBase
	{
		private static readonly Vector3 DialogPosition = new Vector3(0.0f, 0.5f, -0.5f);
		private static readonly Vector3 DialogScale = new Vector3(0.001f, 1.0f, 0.5f);
		private DialogCreator _dialogCreator;
		public LoginGameObject(GameObject gameObject) : base(gameObject)
		{
			_dialogCreator = gameObject.GetComponent<DialogCreator>();
		}

		/// <summary>
		/// Show Dialog to quit Simulation
		/// </summary>
		public void ShowQuitDialog()
		{
			_dialogCreator.ShowSelectionDialog("Leave Simulation?", new Dictionary<string, object>{{"Quit", "Quit"}}, DialogPosition, DialogScale);
		}

		/// <summary>
		/// Is called when a button was clicked in QuitDialog
		/// </summary>
		public void ButtonClicked(GameObject button)
		{
			var buttons = _dialogCreator.GetButtons();

			if (buttons[button] == null)
			{
				_dialogCreator.CloseDialog();
				return;
			}

			var cmd = buttons[button] as string;

			if (cmd == "Quit")
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif 
			}

			_dialogCreator.CloseDialog();
		}
	}
}
