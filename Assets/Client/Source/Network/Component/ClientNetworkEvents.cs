﻿using System.Collections;
using System.Collections.Generic;
using Entitas;
using System.Net;

// Used to issue a connection start.
[Network]
public class ClientStartConnectionComponent : IComponent {

	public IPEndPoint serverEP;
	public PlayerMetadata playerMetadata;

}

// Used to issue a disconnect.
[Network]
public class ClientDisconnectComponent : IComponent {

}