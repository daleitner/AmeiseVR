using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

namespace Assets.Scripts
{
	/// <summary>
	/// Contains all player boards of the white board.
	/// </summary>
	public class PlayerBoardCollection : GameObjectModelBase
	{
		private readonly MessageListener _listener;
		private const float first_x = 2.6f;
		private const float diff_x = 0.8f;
		private const float y = 1.77f;
		private const float z = 0.0f;
		private bool _isBusy;
		private List<CommandInstance> _commands;
		private List<Task> _tasks;
		private Timer _timer;
		public PlayerBoardCollection(GameObject whiteBoard, MessageListener listener)
			: base(whiteBoard)
		{
			_listener = listener;
			_commands = new List<CommandInstance>();
			_tasks = new List<Task>();
			_timer= new Timer
			{
				Interval = 8000,
				AutoReset = false
			};
			PlayerBoards = new List<PlayerBoard>();
		}

		public List<PlayerBoard> PlayerBoards { get; }

		public void AddPlayerBoard(PlayerBoard newPlayerBoard)
		{
			newPlayerBoard.SetParent(GameObject);
			var vector = CalculateNextPosition();
			newPlayerBoard.MoveTo(vector);
			newPlayerBoard.Rotate(new Quaternion(0,0,0,0));
			PlayerBoards.Add(newPlayerBoard);
		}

		public void AddTask(Task newTask, GameObject playerBoardGameObject)
		{
			var playerBoard = GetPlayerBoardByGameObject(playerBoardGameObject);
			playerBoard.AddTask(newTask);
			if (newTask.Command.Parameters.Count(p => p.Type == KnowledgeBase.EmployeeType) == 1)
				return;

			newTask.SetIncomplete();
			var incompleteTasks = GetIncompleteTasks(playerBoard);
			var relatedTasks = incompleteTasks.Where(x => x.Command == newTask.Command).ToList();
			if (!relatedTasks.Any())
				return;
			
			newTask.AddRelatedTasks(relatedTasks);
			relatedTasks.ForEach(rt =>rt.AddRelatedTasks(new List<Task>{newTask}));
		}

		private List<Task> GetIncompleteTasks(PlayerBoard skippedPlayerBoard)
		{
			var tasks = new List<Task>();
			foreach (var playerBoard in PlayerBoards)
			{
				if(playerBoard == skippedPlayerBoard)
					continue;
				tasks.AddRange(playerBoard.Tasks.Where(x => x.IsIncomplete).ToList());
			}
			return tasks;
		}

		public void SendCommands()
		{
			if (_isBusy)
				return;
			_isBusy = true;
			_listener.ReceivedMessage += _listener_ReceivedMessage;
			_commands = new List<CommandInstance>();
			_tasks = new List<Task>();
			foreach (var playerBoard in PlayerBoards)
			{
				foreach (var task in playerBoard.Tasks.Where(x => !x.IsSent && !x.IsIncomplete))
				{
					//ClientConnection.GetInstance().SendCommand(task.Command, parameters.ToArray());
					var command = new CommandInstance(task.Command);
					var parameter = command.GetNextEmptyParameter();
					var employee = 0;
					while (parameter != null)
					{
						if (parameter.Parameter.Type == KnowledgeBase.EmployeeType)
						{
							if (employee == 0)
							{
								parameter.Value = playerBoard.Name;
							}
							else
							{
								var relatedPlayerBoard = PlayerBoards.Single(x =>
									x.Tasks.Contains(task.RelatedTasks[employee - 1]));
								parameter.Value = relatedPlayerBoard.Name;
								task.RelatedTasks[employee - 1].TaskSentSuccessful();
							}

							employee++;
						}
						else
							parameter.Value = task.CurrentVariableParameter;
						parameter = command.GetNextEmptyParameter();
					}
					_commands.Add(command);
					_tasks.Add(task);
				}
			}

			if (!_commands.Any())
			{
				_isBusy = false;
				_listener.ReceivedMessage -= _listener_ReceivedMessage;
				return;
			}

			var firstCommand = _commands.First();
			ClientConnection.GetInstance().SendCommand(firstCommand.Command, firstCommand.ParameterValues.Select(x => x.Value).ToArray());
		}

		private void _listener_ReceivedMessage(MessageObject messageObject)
		{
			Debug.Log("pbc: message received");
			_timer.Stop();
			_timer.Elapsed -= _timer_Elapsed;
			var currentTask = _tasks.First();
			if (messageObject.Type == MessageTypeEnum.Feedback)
			{
				if(messageObject.GetValueOf("feedback").Contains("NOT SUCCESSFUL"))
					currentTask.TaskSentError();
			}

			if (!currentTask.IsError)
				currentTask.TaskSentSuccessful();

			_timer.Elapsed += _timer_Elapsed;
			_timer.Start();
		}

		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Debug.Log("pbc: timer elapsed");
			_timer.Stop();
			_timer.Elapsed -= _timer_Elapsed;
			
			_commands.RemoveAt(0);
			_tasks.RemoveAt(0);
			if (_commands.Any())
			{
				var firstCommand = _commands.First();
				ClientConnection.GetInstance().SendCommand(firstCommand.Command, firstCommand.ParameterValues.Select(x => x.Value).ToArray());
			}
			else
			{
				_listener.ReceivedMessage -= _listener_ReceivedMessage;
				_isBusy = false;
			}
		}

		private Vector3 CalculateNextPosition()
		{
			var x = first_x + diff_x * PlayerBoards.Count;
			return new Vector3(x, y, z);
		}

		private PlayerBoard GetPlayerBoardByGameObject(GameObject gameObject)
		{
			var board = PlayerBoards.SingleOrDefault(b => b.GameObject == gameObject);
			if (board == null)
				board = PlayerBoards.SingleOrDefault(b => b.GameObject == gameObject.transform.parent.gameObject);
			return board;
		}
	}
}
