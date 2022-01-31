namespace Unity.Multiplayer.Tools
{
    struct NetworkSolutionInterfaceParameters
    {
        public INetworkObjectProvider NetworkObjectProvider;
    }
    
    static class NetworkSolutionInterface
    {
        static NetworkSolutionInterfaceParameters s_Parameters;
        
        public static void SetInterface(NetworkSolutionInterfaceParameters parameters)
        {
            parameters.NetworkObjectProvider ??= new NullNetworkObjectProvider();
            s_Parameters = parameters;
        }

        internal static INetworkObjectProvider NetworkObjectProvider => s_Parameters.NetworkObjectProvider;
    }
}