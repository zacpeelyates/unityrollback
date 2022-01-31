using NUnit.Framework;

namespace Unity.Multiplayer.Tools.Tests.Editor
{
    class NetworkSolutionInterfaceTests
    {
        [Test]
        public void GivenWhenSetInterfaceCalledWithNullNetworkObjectProvider_NetworkObjectProviderAccessed_ReturnsValidInstance()
        {
            NetworkSolutionInterface.SetInterface(new NetworkSolutionInterfaceParameters()
            {
                NetworkObjectProvider = null
            });

            var networkObjectProvider = NetworkSolutionInterface.NetworkObjectProvider;
            
            Assert.NotNull(networkObjectProvider);
            Assert.DoesNotThrow(() => networkObjectProvider.GetNetworkObject(0));
        }
    }
}
