using Entitas;

[Network]
public class ConnectionStartComponent : IComponent {

    public ServerConnectionComponent conn;

}

[Network]
public class ConnectionEndComponent : IComponent {

    public ServerConnectionComponent conn;

}

[Network]
public class ConnectionEstablishedComponent : IComponent {

    public ServerConnectionComponent conn;

}