using System.Collections.Generic;
using System.Linq;

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