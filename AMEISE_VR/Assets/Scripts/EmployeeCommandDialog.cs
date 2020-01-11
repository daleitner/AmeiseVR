using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Scripts
{
	public class EmployeeCommandDialog
	{
		private readonly GameObject _employeeCommandControl;
		private readonly FirstPersonController _player;

		public EmployeeCommandDialog(GameObject employeeCommandControl, FirstPersonController player)
		{
			_employeeCommandControl = employeeCommandControl;
			_player = player;
		}

		public void CloseDialog()
		{
			_employeeCommandControl.SetActive(false);
			_player.UnlockCursor();
			_player.enabled = true;
		}

		public void OpenDialog()
		{
			_employeeCommandControl.SetActive(true);
			_player.LockCursor();
			_player.enabled = false;
		}
	}
}
