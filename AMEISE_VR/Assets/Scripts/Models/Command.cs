using System.Collections.Generic;

/// <summary>
/// The Class represents a command which consists of a name and a list of parameters
/// </summary>
public class Command
{
	public Command()
	{
		Parameters = new List<Parameter>();
	}
	public string Name { get; set; }

	public List<Parameter> Parameters { get; set; }

}