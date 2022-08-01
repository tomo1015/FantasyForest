using UnityScreenNavigator.Runtime.Core.Page;

namespace ScreenNavigator.Scripts
{
    public class LoadingPage : Page
    {
        public override void DidPushEnter()
        {
            PageContainer.Of(transform).Push(ResourceKey.HomePagePrefab(),true);
        }
    }
}

