using System.Collections.Generic;
using System.Linq;

/// <summary>
/// CommandInstance is used to instantiate a command, which means that values are assigned to its parameters.
/// </summary>
public class CommandInstance
{
	public CommandInstance(Command command)
	{
		Command = command;
		ParameterValues = new List<ParameterInstance>();
		foreach (var parameter in command.Parameters)
		{
			ParameterValues.Add(new ParameterInstance(parameter));
		}
	}
	public Command Command { get; private set; }

	public List<ParameterInstance> ParameterValues { get; set; }

	public ParameterInstance GetNextEmptyParameter()
	{
		return ParameterValues.FirstOrDefault(x => string.IsNullOrEmpty(x.Value));
	}

	/// <summary>
	/// returns the command in that format where it is sent to the old client.
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		var cmd = Command.Name + " ";
		foreach (var parameter in ParameterValues)
		{
			if (string.IsNullOrEmpty(parameter.Value))
				cmd += "@" + parameter.Parameter.Name + " ";
			else
				cmd += parameter.Value + " ";
		}

		return cmd;
	}
}