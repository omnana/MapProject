using UnityEngine;

namespace Omnana
{
    public class UiTest : MonoBehaviour
    {
        public GameObject redPoint;

        public RedPoint RedPointType;

        public bool isRedPointActive;

        private void Awake()
        {
            RedPointSystem.Inst.AddObserver(RedPointType, RefreshRedPoint);
        }

        private void Start()
        {
            RedPointSystem.Inst.SetActive(RedPointType, isRedPointActive);
        }

        private void RefreshRedPoint(bool active)
        {
            redPoint.SetActive(active);

            Debug.Log(RedPointType + " = " + active);
        }

        public void OnClick()
        {
            isRedPointActive = !isRedPointActive;

            RedPointSystem.Inst.SetActive(RedPointType, isRedPointActive);
        }
    }

}