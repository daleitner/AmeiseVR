using UnityEngine;
using UnityEditor;

public abstract class ControllerBase
{
	protected ControllerBase(SmartPhoneManager manager)
	{
		Manager = manager;
	}
	protected SmartPhoneManager Manager { get; }
	public abstract bool Accepts(string tag);
	public abstract void Execute(string tag);
}