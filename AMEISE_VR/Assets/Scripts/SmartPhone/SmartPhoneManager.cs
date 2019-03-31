using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class SmartPhoneManager : MonoBehaviour
{
	private ControllerBase _activeController;
	private Dictionary<ScreenEnum, ControllerBase> _controllers;
	private Dictionary<ScreenEnum, GameObject> _views;

	public GameObject SmartPhone;
	public float Reach = 4.0F;
	[HideInInspector] public bool InReach;
	
	private bool alreadyClicked = false;
	private void Start()
	{
		_views = new Dictionary<ScreenEnum, GameObject>();
		_controllers = new Dictionary<ScreenEnum, ControllerBase>();
		//KnowledgeBase.Instance.AddParameterType("E", new List<string>{ "Alex", "Richard", "Stefanie", "Christine", "Thomas", "Daniel" });
		//KnowledgeBase.Instance.AddParameterType("D", new List<string> { "Document1", "Code", "Specification", "Something", "Design", "Manual", "Another document" });
		//KnowledgeBase.Instance.Commands.AddRange(new[]
		//{
		//	new Command { Name = "Hire", Parameters = new List<Parameter>{new Parameter("Employee", "E")}},
		//	new Command { Name = "Release", Parameters = new List<Parameter>{new Parameter("Employee", "E")}},
		//	new Command { Name = "Code", Parameters = new List<Parameter>{new Parameter("Employee", "E"), new Parameter("Document", "D")} },
		//	new Command { Name = "Write Doc" },
		//	new Command { Name = "Do Sth" },
		//	new Command { Name = "Stop" },
		//	new Command { Name = "Go" }
		//});
		var values = Enum.GetValues(typeof(ScreenEnum)).Cast<ScreenEnum>().ToList();
		values.ForEach(x => 
		{
			_views.Add(x, SmartPhone.transform.Find(x.ToString()).gameObject);
			_controllers.Add(x, ControllerFactory.GetController(x, _views[x], this));
		});
		Show(ScreenEnum.MainScreen);
	}

	private void Update()
	{
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0F));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Reach))
		{
			if (Input.GetMouseButton(0))
			{
				if (alreadyClicked)
					return;
				alreadyClicked = true;
				if (_activeController.Accepts(hit.collider.tag))
				{
					InReach = true;
					_activeController.Execute(hit.transform.gameObject);
				}
				else if (hit.collider.tag == Tags.BackTag)
				{
					InReach = true;
					_activeController.Back();
				}
				else if(hit.collider.tag == Tags.HomeTag)
				{
					InReach = true;
					_activeController.Home();
				}
				else
				{
					InReach = false;
				}
			}
			else
			{
				alreadyClicked = false;
			}
		}
		else
		{
			InReach = false;
		}
	}

	#region public methods
	public void Show(ScreenEnum screenName, object payload = null)
	{
		foreach (var key in _views.Keys)
		{
			_views[key].SetActive(screenName == key);
		}
		_activeController = _controllers[screenName];
		_activeController.Activate(payload);
	}

	public GameObject Create(GameObject gameObject)
	{
		return Instantiate(gameObject);
	}
	#endregion

}