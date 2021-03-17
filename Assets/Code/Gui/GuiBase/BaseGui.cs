using UnityEngine.UI;
using UnityEngine;

namespace Omnana
{
    public class BaseGui : MonoBehaviour, IGui
    {
        public HotFixBaseGui HotFixContent;

        protected bool NeedUpdate = true;

        private Canvas canvas;

        public int SortingOrder
        {
            get
            {
                return canvas.sortingOrder;
            }
            set
            {
                canvas.sortingOrder = value;
            }
        }

        private void Awake()
        {
            canvas = gameObject.GetComponent<Canvas>();

            if (canvas == null) canvas = gameObject.AddComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var raycaster = gameObject.GetComponent<GraphicRaycaster>();

            if (raycaster == null) gameObject.AddComponent<GraphicRaycaster>();

            var canvasScale = gameObject.GetComponent<CanvasScaler>();

            if (canvasScale == null) gameObject.AddComponent<CanvasScaler>();
        }

        private void Update()
        {
            if (!NeedUpdate) return;

            if (HotFixContent != null)
            {
                HotFixContent.DoUpdate();
            }
            else
            {
                DoUpdate();
            }
        }

        private void OnDestroy()
        {
            if (HotFixContent != null)
            {
                HotFixContent.DoDestroy();
            }
            else
            {
                DoDestroy();
            }
        }

        public void Init()
        {
            if (HotFixContent != null)
            {
                HotFixContent.DoInit();
            }
            else
            {
                DoInit();
            }
        }

        public void Open()
        {
            if (HotFixContent != null)
            {
                HotFixContent.DoOpen();
            }
            else
            {
                DoOpen();
            }
        }

        public void Close()
        {
            if (HotFixContent != null)
            {
                HotFixContent.DoClose();
            }
            else
            {
                DoClose();
            }
        }

        public virtual void DoInit()
        {
        }

        public virtual void DoOpen()
        {
        }

        public virtual void DoUpdate()
        {
        }

        public virtual void DoClose()
        {
        }

        public virtual void DoDestroy()
        {
        }
    }

}