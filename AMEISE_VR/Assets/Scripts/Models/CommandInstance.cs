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
}