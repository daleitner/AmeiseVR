/// <summary>
/// Contains a value to the given parameter.
/// </summary>
public class ParameterInstance
{
	public ParameterInstance(Parameter parameter)
	{
		Parameter = parameter;
	}

	public Parameter Parameter { get; private set; }
	public string Value { get; set; }
}