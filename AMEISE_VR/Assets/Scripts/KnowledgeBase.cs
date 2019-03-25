
using System.Collections.Generic;

public class KnowledgeBase
{
	private readonly Dictionary<string, List<string>> _parameterTypes;
	private static KnowledgeBase _instance;
	private KnowledgeBase()
	{
		Employees = new List<string>();
		Commands = new List<Command>();
		_parameterTypes = new Dictionary<string, List<string>>();
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

	public List<string> Employees { get; private set; }
	public List<Command> Commands { get; set; }
	public const string EmployeeType = "E";

	public void AddParameterType(string parameterType, List<string> values)
	{
		_parameterTypes.Add(parameterType, values);

		if (parameterType == EmployeeType)
			Employees = values;
	}

	public List<string> GetValuesOfParameterType(string parameterType)
	{
		return _parameterTypes[parameterType];
	}
}
