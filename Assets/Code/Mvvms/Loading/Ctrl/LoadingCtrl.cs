using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCtrl : BaseCtrl
{
    private HotfixMgr hotfixMgr;

    public LoadingViewModel LoadingViewModel
    {
        get
        {
            return viewModel as LoadingViewModel;
        }
    }

    public override void Loaded(ViewModelBase vm)
    {
        base.Loaded(vm);

        hotfixMgr = ServiceLocator.Resolve<HotfixMgr>();

        hotfixMgr.RegisterRequsetCallback((state, sgm, isComplete) =>
        {

        });

        hotfixMgr.Start();
    }

}
