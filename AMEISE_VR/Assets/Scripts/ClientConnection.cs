using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class ClientConnection
{
	private readonly Socket _socket;
	public ClientConnection()
	{
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	}

	public void Connect(string ipAdress, int port)
	{
		var adress = IPAddress.Parse(ipAdress);
		var endPoint = new IPEndPoint(adress, port);
		_socket.Connect(endPoint);
	}

	public void CloseConnection()
	{
		if(_socket.Connected)
			_socket.Close();
	}
}
