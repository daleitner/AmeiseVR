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
		KnowledgeBase.Instance.Employees.AddRange(new[] { "Alex", "Richard", "Stefanie", "Christine", "Thomas", "Daniel" });
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
					_activeController.Execute(hit.collider.tag);
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
	public void Show(ScreenEnum screenName)
	{
		foreach (var key in _views.Keys)
		{
			_views[key].SetActive(screenName == key);
		}
		_activeController = _controllers[screenName];
	}

	public GameObject Create(GameObject gameObject)
	{
		return Instantiate(gameObject);
	}
	#endregion

}