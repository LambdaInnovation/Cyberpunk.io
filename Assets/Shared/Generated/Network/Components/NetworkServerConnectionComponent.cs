//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class NetworkEntity {

    public ServerConnectionComponent serverConnection { get { return (ServerConnectionComponent)GetComponent(NetworkComponentsLookup.ServerConnection); } }
    public bool hasServerConnection { get { return HasComponent(NetworkComponentsLookup.ServerConnection); } }

    public void AddServerConnection(float newStateTime, float newPacketUnreceivedTime, float newTimeoutCounter, int newRetryTime, float newKeepAliveCounter, PlayerMetadata newPlayerMetadata, System.Net.IPEndPoint newClientEP, ServerConnectionComponent.State newState) {
        var index = NetworkComponentsLookup.ServerConnection;
        var component = CreateComponent<ServerConnectionComponent>(index);
        component.stateTime = newStateTime;
        component.packetUnreceivedTime = newPacketUnreceivedTime;
        component.timeoutCounter = newTimeoutCounter;
        component.retryTime = newRetryTime;
        component.keepAliveCounter = newKeepAliveCounter;
        component.playerMetadata = newPlayerMetadata;
        component.clientEP = newClientEP;
        component.state = newState;
        AddComponent(index, component);
    }

    public void ReplaceServerConnection(float newStateTime, float newPacketUnreceivedTime, float newTimeoutCounter, int newRetryTime, float newKeepAliveCounter, PlayerMetadata newPlayerMetadata, System.Net.IPEndPoint newClientEP, ServerConnectionComponent.State newState) {
        var index = NetworkComponentsLookup.ServerConnection;
        var component = CreateComponent<ServerConnectionComponent>(index);
        component.stateTime = newStateTime;
        component.packetUnreceivedTime = newPacketUnreceivedTime;
        component.timeoutCounter = newTimeoutCounter;
        component.retryTime = newRetryTime;
        component.keepAliveCounter = newKeepAliveCounter;
        component.playerMetadata = newPlayerMetadata;
        component.clientEP = newClientEP;
        component.state = newState;
        ReplaceComponent(index, component);
    }

    public void RemoveServerConnection() {
        RemoveComponent(NetworkComponentsLookup.ServerConnection);
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

    static Entitas.IMatcher<NetworkEntity> _matcherServerConnection;

    public static Entitas.IMatcher<NetworkEntity> ServerConnection {
        get {
            if (_matcherServerConnection == null) {
                var matcher = (Entitas.Matcher<NetworkEntity>)Entitas.Matcher<NetworkEntity>.AllOf(NetworkComponentsLookup.ServerConnection);
                matcher.componentNames = NetworkComponentsLookup.componentNames;
                _matcherServerConnection = matcher;
            }

            return _matcherServerConnection;
        }
    }
}
