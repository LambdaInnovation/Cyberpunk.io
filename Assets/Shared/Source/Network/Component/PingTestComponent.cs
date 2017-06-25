using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using Entitas;

[Network]
public class PingTestComponent : IComponent {
	public IPEndPoint target;

	public int testIntervalMsec = 1000;
	public float cooldown = 1.0f;


}
