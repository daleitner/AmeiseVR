﻿
using System.Collections.Generic;

public class KnowledgeBase
{
	private static KnowledgeBase _instance;
	private KnowledgeBase()
	{
		Employees = new List<string>();
	}

	public static KnowledgeBase Instance
	{
		get
		{
			if (_instance == null)
				_instance = new KnowledgeBase();
			return _instance;
		}
	}

	public List<string> Employees { get; set; }
}
