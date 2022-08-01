using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityScreenNavigator.Runtime.Core.Modal;

namespace ScreenNavigator.Scripts
{
    public class HomePage : Page
    {
        [SerializeField] private Button _settingButton;//設定
        [SerializeField] private Button _stageSelectButton;//ステージセレクト
        [SerializeField] private Button _shopButton;//ショップ

        public override IEnumerator Initialize()
        {
            //クリック時のイベントセット
            _settingButton.onClick.AddListener(OnSettingButtonClicked);
            _stageSelectButton.onClick.AddListener(OnStageSelectButtonClicked);
            _shopButton.onClick.AddListener(OnShopButtonClicked);

            // preload shop page prefab
            yield return PageContainer.Of(transform).Preload(ResourceKey.ShopPagePrefab());
            // preload stage select page prefab
            yield return PageContainer.Of(transform).Preload(ResourceKey.StageSelectPrefab());
            // Simulate loading time
            yield return new WaitForSeconds(1.0f);
        }

        public override IEnumerator Cleanup()
        {
            _settingButton.onClick.RemoveListener(OnSettingButtonClicked);
            _shopButton.onClick.RemoveListener(OnShopButtonClicked);
            _stageSelectButton.onClick.RemoveListener(OnStageSelectButtonClicked);

            PageContainer.Of(transform).ReleasePreloaded(ResourceKey.ShopPagePrefab());
            PageContainer.Of(transform).ReleasePreloaded(ResourceKey.StageSelectPrefab());

            yield break;
        }


        private void OnSettingButtonClicked()
        {
            ModalContainer.Find(ContainerKey.MainModalContainer).Push(ResourceKey.SettingModalPrefab(),true);
        }

        private void OnStageSelectButtonClicked()
        {
            PageContainer.Of(transform).Push(ResourceKey.StageSelectPrefab(),true);
        }

        private void OnShopButtonClicked()
        {
            PageContainer.Of(transform).Push(ResourceKey.ShopPagePrefab(),true);
        }
    }

}
