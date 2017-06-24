using Entitas;
using System.Net;

[Game]
public class SendPacketComponent : IComponent {

	public IPEndPoint target;
	public Packet packet;

}

[Game]
public class RecvPacketComponent : IComponent {

	public IPEndPoint source;
	public Packet packet;

}