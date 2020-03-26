using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains the Date and all Commands.
/// </summary>
public class KnowledgeBase
{
	private readonly Dictionary<string, List<string>> _parameterTypes;
	private static KnowledgeBase instance;
	private KnowledgeBase()
	{
		Employees = new List<string>();
		Commands = new List<Command>();
		_parameterTypes = new Dictionary<string, List<string>>();
	}

	public static KnowledgeBase Instance => instance ?? (instance = new KnowledgeBase());
	
	public List<string> Employees { get; private set; }
	public List<Command> Commands { get; set; }
	public DateTime Date { get; set; }
	public bool LoadingCommandsFinished { get; private set; }
	public bool ContinueTime { get; set; }
	public bool ProjectDelivered { get; set; }

	public List<Command> EmployeeCommands
	{
		get { return Commands?.Where(cmd => cmd.GetFeedBackImmediately && !SpecialCommands.Contains(cmd) || cmd.Name == "show me tasks of developer").ToList(); }
	}

	public List<Command> SecretaryCommands
	{
		get
		{
			return new List<Command>
			{
				Commands.Single(x => x.Name == "show me available developers"),
				Commands.Single(x => x.Name == "show me my team members")
			};
		}
	}

	public List<Command> WhiteBoardCommands
	{
		get
		{
			return Commands?.Where(cmd => cmd.Parameters.Any(param => param.Type == EmployeeType) && !EmployeeCommands.Contains(cmd) && !cmd.GetFeedBackImmediately).ToList();
		}
	}

	public Command DeveloperInformationCommand => Commands.Single(x => x.Name == "information about developer");
	public Command ResourceCommand => Commands.Single(x => x.Name == "information about spent resources");
	public Command CancelProjectCommand => Commands.Single(x => x.Name == "finish project");
	public Command FinishProjectCommand => Commands.Single(x => x.Name == "deliver system");
	public Command CustomerAcceptanceTestCommand => Commands.Single(x => x.Name == "customer perform acceptance test");
	public List<Command> CustomerCommands => new List<Command>{CustomerAcceptanceTestCommand, FinishProjectCommand };
	private List<Command> SpecialCommands => new List<Command>{DeveloperInformationCommand, ResourceCommand, CancelProjectCommand, FinishProjectCommand, CustomerAcceptanceTestCommand};

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
	}

	public void SetLoadingCommandsFinished()
	{
		LoadingCommandsFinished = true;
	}
}
