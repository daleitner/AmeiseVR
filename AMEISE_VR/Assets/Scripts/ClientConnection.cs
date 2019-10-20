using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ClientConnection
{
	private readonly Socket _socket;
	private static ClientConnection client;
	private readonly BackgroundWorker _worker;
	private bool _connectionClosed = false;
	private readonly List<List<MessageObject>> _messageLists;

	private ClientConnection()
	{
		_messageLists = new List<List<MessageObject>>();
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_worker = new BackgroundWorker()
		{
			WorkerSupportsCancellation = true
		};
		_worker.DoWork += _worker_DoWork;
		_worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
	}

	public static ClientConnection GetInstance()
	{
		return client ?? (client = new ClientConnection());
	}

	public void Connect(string ipAdress, int port)
	{
		var adress = IPAddress.Parse(ipAdress);
		var endPoint = new IPEndPoint(adress, port);
		_socket.Connect(endPoint);
		Debug.Log("Client Connected!");
		if (!_worker.IsBusy)
		{
			_worker.RunWorkerAsync();
		}
	}

	private void _worker_DoWork(object sender, DoWorkEventArgs e)
	{
		var worker = (BackgroundWorker)sender;
		ReceiveText();
	}

	private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		Debug.Log("Stopped listening!");
	}

	public void CloseConnection()
	{
		if(_socket.Connected)
			_socket.Close();
		_connectionClosed = true;
	}

	public void SendText(MessageObject message)
	{
		var text = message.ToString();
		using (var stream = new NetworkStream(_socket))
		{
			var bytes = System.Text.Encoding.ASCII.GetBytes(text);
			stream.Write(bytes, 0, bytes.Length);
		}
	}

	public void SendCommand(Command command, params string[] parameters)
	{
		if (command.Parameters.Count != parameters.Length)
			throw new ArgumentException("number of parameters is wrong!");
		var dict = new Dictionary<string, string>();
		dict.Add("command", command.Name);
		var i = 0;
		foreach (var param in parameters)
		{
			if (string.IsNullOrEmpty(param))
				return;
			dict.Add("param" + (i + 1), param);
			i++;
		}
		var message = new MessageObject(MessageTypeEnum.Command, dict);
		SendText(message);
	}

	public void ReceiveText()
	{
		using (var stream = new NetworkStream(_socket))
		{
			// Examples for CanRead, Read, and DataAvailable.

			// Check to see if this NetworkStream is readable.
			while (!_connectionClosed)
			{
				if (stream.CanRead)
				{
					byte[] myReadBuffer = new byte[1024];
					StringBuilder message = new StringBuilder();
					int numberOfBytesRead = 0;

					// Incoming message may be larger than the buffer size.
					do
					{
						numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

						message.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

					} while (!message.ToString().EndsWith(";"));
					Debug.Log("You received the following message : " + message);
					var msgobj = new MessageObject(message.ToString());
					foreach (var messageList in _messageLists)
					{
						messageList.Add(msgobj);
					}
					// Print out the received message to the console.
				}
				else
				{
					Debug.Log("Sorry.  You cannot read from this NetworkStream.");
				}
			}
		}
	}

	public void RegisterList(List<MessageObject> messageObjects)
	{
		_messageLists.Add(messageObjects);
	}
}
