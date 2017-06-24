using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Entitas;

public class NetworkingFeature : Feature {
	public UdpClient udp {
		get; private set;
	}

	public NetworkingFeature(Contexts ctxs) : base("Networking") {
		Add(new SendPacketSystem(ctxs, this));
		Add(new RecvPacketSystem(ctxs, this));
	}

	public override void Initialize() {
		udp = new UdpClient(NetworkConfig.port);
		base.Initialize();
	}

	public override void TearDown() {
		base.TearDown();
		udp.Close();
	}

}

public class RecvPacketSystem : IInitializeSystem, ITearDownSystem, IExecuteSystem, ICleanupSystem {
	struct IncomingPacket {
		public IPEndPoint source;
		public Packet packet;
	}

	Thread thread;
	NetworkingFeature feature;

	Queue<IncomingPacket> incomingPackets = new Queue<IncomingPacket>();

	IGroup<GameEntity> recvGroup;

	public RecvPacketSystem(Contexts ctxs, NetworkingFeature feature) {
		this.feature = feature;
		recvGroup = ctxs.game.GetGroup(GameMatcher.RecvPacket);
	}

	public void Initialize() {
		thread = new Thread(new ThreadStart(ListenThreadFunc));
		thread.Start();
	}

	public void TearDown() {
		thread.Abort();
	}

	public void Execute() {
		IncomingPacket[] arr;
		lock (incomingPackets) {
			arr = incomingPackets.ToArray();
			incomingPackets.Clear();
		}

		var game = Contexts.sharedInstance.game;
		foreach (var inc in arr) {
			Debug.Log("Recv packet " + inc.packet.packetType);
			game.CreateEntity()
				.AddRecvPacket(inc.source, inc.packet);
		}
	}

	public void Cleanup() {
		foreach (var entity in recvGroup.GetEntities()) {
			entity.Destroy();
		}
	}

	void ListenThreadFunc() {
		while (true) {
			var ep = new IPEndPoint(IPAddress.Any, NetworkConfig.port);

			var data = feature.udp.Receive(ref ep);

			var incoming = new IncomingPacket {
				source = ep,
				packet = Packet.FromBytes(data)
			};

			lock (incomingPackets) {
				incomingPackets.Enqueue(incoming);
				Debug.Log("IncomingPackets size " + incomingPackets.Count);
			}
		}
	}

}


public class SendPacketSystem : ReactiveSystem<GameEntity>, ICleanupSystem {
	
	NetworkingFeature feature;
	IGroup<GameEntity> sendGroup;

	public SendPacketSystem(Contexts ctxs, NetworkingFeature feature) : base(ctxs.game) {
		this.feature = feature;
		sendGroup = ctxs.game.GetGroup(GameMatcher.PingTest);
	}

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
        return context.CreateCollector(GameMatcher.SendPacket);
    }

    protected override bool Filter(GameEntity entity) {
        return entity.hasSendPacket;
    }

    protected override void Execute(List<GameEntity> entities) {
        foreach (var ent in entities) {
			var comp = ent.sendPacket;
			feature.udp.Send(comp.packet.ToBytes(), comp.packet.data.Length, comp.target);
			Debug.Log("Send Packet " + comp.packet);
		}
    }

	public void Cleanup() {
		foreach (var ent in sendGroup.GetEntities()) {
			ent.Destroy();
		}
	}
}
