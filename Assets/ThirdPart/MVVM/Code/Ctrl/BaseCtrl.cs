using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omnana
{
    public class BaseCtrl
    {
        protected ViewModelBase viewModel;


        public BaseCtrl()
        {
        }

        public virtual void Loaded(ViewModelBase vm)
        {
            viewModel = vm;
        }

        public virtual void DispoWith()
        {
        }
    }
}