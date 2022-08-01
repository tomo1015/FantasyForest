namespace ScreenNavigator.Scripts
{
    public static class ResourceKey
    {
        private const string PrefabFormat = "Prefabs/prefab_{0}";
        private const string TopPagePrefabName = "page_top";
        private const string HomePagePrefabName = "page_home";
        private const string LoadingPagePrefabName = "page_loading";
        private const string ShopPagePrefabName = "page_shop";
        private const string StageSelectPrefabName = "page_stageselect";
        private const string SettingsModalPrefabName = "modal_settings";

        public static string TopPagePrefab()
        {
            return string.Format(PrefabFormat, TopPagePrefabName);
        }
        public static string HomePagePrefab()
        {
            return string.Format(PrefabFormat, HomePagePrefabName);
        }
        public static string LoadingPagePrefab()
        {
            return string.Format(PrefabFormat, LoadingPagePrefabName);
        }

        public static string ShopPagePrefab()
        {
            return string.Format(PrefabFormat, ShopPagePrefabName);
        }
        public static string StageSelectPrefab()
        {
            return string.Format(PrefabFormat, StageSelectPrefabName);
        }

        public static string SettingModalPrefab()
        {
            return string.Format(PrefabFormat, SettingsModalPrefabName);
        }
    }

}
