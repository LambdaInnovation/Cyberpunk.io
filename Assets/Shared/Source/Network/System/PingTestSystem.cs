using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System;

public class PingTestSystem : IExecuteSystem {
    NetworkContext ctx;
    IGroup<NetworkEntity> groupPings;

	public PingTestSystem(Contexts ctxs) {
        ctx = ctxs.network;
        groupPings = ctx.GetGroup(NetworkMatcher.PingTest);
    }

    public void Execute() {
        foreach (var ent in groupPings.GetEntities()) {
			var data = ent.pingTest;

            data.cooldown -= Time.deltaTime;
            if (data.cooldown <= 0) {
                data.cooldown = data.testIntervalMsec / 1000.0f;

                ctx.CreateEntity()
                    .AddSendPacket(data.target, new Packet(PacketType.Ping, new byte[128]));
                Debug.Log("Ping Test to " + data.target);
            }
		}
    }

}
