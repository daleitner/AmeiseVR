using System.Collections.Generic;

public class Command
{
	public Command()
	{
		ParameterTypes = new Dictionary<string, string>();
		ParameterValues = new Dictionary<string, string>();
	}
	public string Name { get; set; }
	public Dictionary<string, string> ParameterTypes { get; set; }
	public Dictionary<string, string> ParameterValues { get; set; }
}