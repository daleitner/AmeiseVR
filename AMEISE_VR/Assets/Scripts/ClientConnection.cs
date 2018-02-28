using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEditorInternal;

public class ClientConnection
{
	private readonly Socket _socket;
	private static ClientConnection client;
	private ClientConnection()
	{
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
		Debug.WriteLine("Client Connected!");
		SendText("hello server");
	}

	public void CloseConnection()
	{
		if(_socket.Connected)
			_socket.Close();
	}

	public void SendText(string text)
	{
		using (var stream = new NetworkStream(_socket))
		{
			var bytes = System.Text.Encoding.ASCII.GetBytes(text);
			stream.Write(bytes, 0, bytes.Length);
		}
	}
}
