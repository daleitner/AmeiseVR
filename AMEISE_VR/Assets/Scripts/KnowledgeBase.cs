using System;
using System.Collections.Generic;
using System.Linq;

public class KnowledgeBase
{
	private readonly Dictionary<string, List<string>> _parameterTypes;
	private static KnowledgeBase instance;
	private KnowledgeBase()
	{
		Employees = new List<string>();
		Commands = new List<Command>();
		History = new List<string>();
		_parameterTypes = new Dictionary<string, List<string>>();
	}

	public static KnowledgeBase Instance => instance ?? (instance = new KnowledgeBase());

	public List<string> History { get; private set; }
	public List<string> Employees { get; private set; }
	public List<Command> Commands { get; set; }
	public DateTime Date { get; set; }
	public bool LoadingCommandsFinished { get; private set; }
	public bool ContinueTime { get; set; }
	public bool ProjectDelivered { get; set; }

	public List<Command> WhiteBoardCommands
	{
		get
		{
			return Commands?.Where(cmd => cmd.Parameters.Any(param => param.Type == EmployeeType)).ToList();
		}
	}

	public List<Command> SystemCommands
	{
		get
		{
			return Commands?.Where(cmd => !WhiteBoardCommands.Contains(cmd)).ToList();
		}
	}

	public Command DeveloperInformationCommand => WhiteBoardCommands.Single(x => x.Name == "information about developer");
	public Command ResourceCommand => SystemCommands.Single(x => x.Name == "information about spent resources");
	public Command CancelProjectCommand => SystemCommands.Single(x => x.Name == "finish project");
	public Command FinishProjectCommand => SystemCommands.Single(x => x.Name == "deliver system");
	public Command CustomerAcceptanceTestCommand => SystemCommands.Single(x => x.Name == "customer perform acceptance test");

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
		if (message.Length >= 10 && DateTime.TryParse(message.Substring(0, 10), out var newDate))
		{
			Date = newDate;
		}
		History.Add(message);
	}

	public void SetLoadingCommandsFinished()
	{
		LoadingCommandsFinished = true;
	}
}
