namespace ScreenNavigator.Scripts
{
    public static class ResourceKey
    {
        private const string PrefabFormat = "Prefabs/prefab_{0}";
        private const string TopPagePrefabName = "page_top";


        public static string TopPagePrefab()
        {
            return string.Format(PrefabFormat, TopPagePrefabName);
        }
    }

}
