using Entitas;
using System.Net;

[Network]
public class SendPacketComponent : IComponent {

	public IPEndPoint target;
	public Packet packet;

}

[Network]
public class RecvPacketComponent : IComponent {

	public IPEndPoint source;
	public Packet packet;

}