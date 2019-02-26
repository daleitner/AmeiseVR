using UnityEngine;
using UnityEditor;

public abstract class ControllerBase
{
	protected ControllerBase(SmartPhoneManager manager)
	{
		Manager = manager;
	}
	protected SmartPhoneManager Manager { get; private set; }
	public abstract bool Accepts(string tag);
	public abstract void Execute(GameObject gameObject);
	public abstract void Activate(object payload);
	public abstract void Back();

	public void Home()
	{
		Manager.Show(ScreenEnum.MainScreen);
	}
}