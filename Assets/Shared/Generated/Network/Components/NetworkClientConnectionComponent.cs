//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class NetworkEntity {

    public ClientConnectionComponent clientConnection { get { return (ClientConnectionComponent)GetComponent(NetworkComponentsLookup.ClientConnection); } }
    public bool hasClientConnection { get { return HasComponent(NetworkComponentsLookup.ClientConnection); } }

    public void AddClientConnection(float newStateTime, float newPacketUnreceivedTime, System.Net.IPEndPoint newServerEP, PlayerMetadata newPlayerMetadata, float newTimeoutCounter, float newKeepAliveCounter, int newRetryTime, ClientConnectionComponent.State newState) {
        var index = NetworkComponentsLookup.ClientConnection;
        var component = CreateComponent<ClientConnectionComponent>(index);
        component.stateTime = newStateTime;
        component.packetUnreceivedTime = newPacketUnreceivedTime;
        component.serverEP = newServerEP;
        component.playerMetadata = newPlayerMetadata;
        component.timeoutCounter = newTimeoutCounter;
        component.keepAliveCounter = newKeepAliveCounter;
        component.retryTime = newRetryTime;
        component.state = newState;
        AddComponent(index, component);
    }

    public void ReplaceClientConnection(float newStateTime, float newPacketUnreceivedTime, System.Net.IPEndPoint newServerEP, PlayerMetadata newPlayerMetadata, float newTimeoutCounter, float newKeepAliveCounter, int newRetryTime, ClientConnectionComponent.State newState) {
        var index = NetworkComponentsLookup.ClientConnection;
        var component = CreateComponent<ClientConnectionComponent>(index);
        component.stateTime = newStateTime;
        component.packetUnreceivedTime = newPacketUnreceivedTime;
        component.serverEP = newServerEP;
        component.playerMetadata = newPlayerMetadata;
        component.timeoutCounter = newTimeoutCounter;
        component.keepAliveCounter = newKeepAliveCounter;
        component.retryTime = newRetryTime;
        component.state = newState;
        ReplaceComponent(index, component);
    }

    public void RemoveClientConnection() {
        RemoveComponent(NetworkComponentsLookup.ClientConnection);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class NetworkMatcher {

    static Entitas.IMatcher<NetworkEntity> _matcherClientConnection;

    public static Entitas.IMatcher<NetworkEntity> ClientConnection {
        get {
            if (_matcherClientConnection == null) {
                var matcher = (Entitas.Matcher<NetworkEntity>)Entitas.Matcher<NetworkEntity>.AllOf(NetworkComponentsLookup.ClientConnection);
                matcher.componentNames = NetworkComponentsLookup.componentNames;
                _matcherClientConnection = matcher;
            }

            return _matcherClientConnection;
        }
    }
}
