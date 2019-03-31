
using System.Collections.Generic;
using System.Linq;

public class KnowledgeBase
{
	private readonly Dictionary<string, List<string>> _parameterTypes;
	private static KnowledgeBase _instance;
	private KnowledgeBase()
	{
		Employees = new List<string>();
		Commands = new List<Command>();
		History = new List<string>();
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

	public List<string> History { get; private set; }
	public List<string> Employees { get; private set; }
	public List<Command> Commands { get; set; }

	public List<Command> EmployeeCommands
	{
		get
		{
			return Commands != null
				? Commands.Where(cmd => cmd.Parameters.Any(param => param.Type == EmployeeType)).ToList()
				: null;
		}
	}

	public List<Command> SystemCommands
	{
		get
		{
			return Commands != null
				? Commands.Where(cmd => !EmployeeCommands.Contains(cmd)).ToList()
				: null;
		}
	}

	public const string EmployeeType = "Entwickler";

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

	public void AddMessage(string message)
	{
		History.Add(message);
	}
}
