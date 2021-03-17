using UnityEngine;

namespace Omnana
{
    public class HotFixAssetLoadMgr
    {
        public static HotFixAssetLoadMgr Instance { get; private set; } = new HotFixAssetLoadMgr();

        private AssetLoadMgr assetLoadMgr;

        public HotFixAssetLoadMgr()
        {
            assetLoadMgr = AssetLoadMgr.Instance;
        }

        public virtual Object LoadSync(string assetName)
        {
            return assetLoadMgr.LoadSync(assetName);
        }


        public virtual void LoadSync(string assetName, AssetsLoadCallback callFun)
        {
            assetLoadMgr.LoadAsync(assetName, callFun);
        }

        public virtual void Unload(Object obj)
        {
            assetLoadMgr.Unload(obj);
        }
    }
}

