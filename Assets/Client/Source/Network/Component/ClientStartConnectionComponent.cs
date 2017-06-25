using System.Collections;
using System.Collections.Generic;
using Entitas;
using System.Net;

// Used to issue a connection start.
[Network]
public class ClientStartConnectionComponent : IComponent {

	public IPEndPoint serverEP;

}
