using System;
using System.Collections.Generic;

public class MessageObject
{
	private Dictionary<string, string> dictionary;

	public MessageObject(MessageTypeEnum type, Dictionary<string, string> properties)
	{
		Type = type;
		dictionary = properties;
	}

	public MessageObject(string message)
	{
		if(string.IsNullOrEmpty(message))
			throw new ArgumentNullException("message is empty!");
		dictionary = new Dictionary<string, string>();
		var parts = message.Substring(0, message.Length-1).Split('|');
		Type = (MessageTypeEnum) Enum.Parse(typeof(MessageTypeEnum), parts[0]);
		for (var i = 1; i < parts.Length; i++)
		{
			if (string.IsNullOrEmpty(parts[i]))
				continue;
			var property = parts[i].Split(':');
			dictionary.Add(property[0], property[1]);
		}
	}

	public MessageTypeEnum Type { get; private set; }

	public string GetValueOf(string property)
	{
		return dictionary[property];
	}

	public override string ToString()
	{
		var str = "";
		foreach (var key in dictionary.Keys)
		{
			str += key + ":" + dictionary[key] + "|";
		}

		str = str.Substring(0, str.Length - 1);
		return Type + "|" + str + ";";
	}
}