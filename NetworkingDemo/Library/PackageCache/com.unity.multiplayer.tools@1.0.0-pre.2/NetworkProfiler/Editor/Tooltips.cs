namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    static class Tooltips
    {
        public const string SearchBar =
@"Search for a specific entry in the tree view. Additional Syntax:

Type:
  t:text (Filter by type column - ex. t:RPC)
    
Direction:
  dir:in (Only received bytes)
  dir:out (Only sent bytes)

Bytes:
  b<5 (Filter by bytes less than 5)
  b<=5 (Filter by bytes less than or equal to 5)
  b>5 (Filter by bytes greater than 5)
  b>=5 (Filter by bytes greater than or equal to 5)
  b==5 (Filter by bytes exacly 5)
  b!=5 (Filter by bytes not 5)";

        public const string DetailsViewBytes_DarkTheme =
            "Bytes sent locally are displayed in grey. Bytes sent between different instances are displayed in white.";

        public const string DetailsViewBytes_LightTheme =
            "Bytes sent locally are displayed in grey. Bytes sent between different instances are displayed in black.";
    }
}
