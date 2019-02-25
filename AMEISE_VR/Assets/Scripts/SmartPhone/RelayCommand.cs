using UnityEngine;
using UnityEditor;
using System;

public class RelayCommand
{
	readonly Action<object> _execute;
	readonly Predicate<object> _canExecute;

	public RelayCommand(Action<object> execute)
		: this(execute, null) { }

	public RelayCommand(Action<object> execute, Predicate<object> canExecute)
	{
		if (execute == null)
			throw new ArgumentNullException("execute");
		_execute = execute;
		_canExecute = canExecute;
	}

	public bool CanExecute(object parameter)
	{
		try
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}
		catch (Exception e)
		{
			System.Console.WriteLine(e.Message);
		}

		return false;
	}


	public void Execute(object parameter)
	{
		try
		{
			if (!CanExecute(parameter))
				throw new Exception("Execute Command Denied!");
			_execute(parameter);
		}
		catch (Exception e)
		{
			System.Console.WriteLine(e.Message);
		}
	}

}