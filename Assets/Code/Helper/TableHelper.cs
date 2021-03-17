using System;

namespace Omnana
{
    public class TableHelper
    {
        /// <summary>
        /// Csv配置管理器组
        /// </summary>
        private static readonly Type[] MgrTypeArr = new Type[]
        {
        typeof(TestModelMgr),
        };

        public static Action DownloadFinishCallabck;

        private static int LoadNum { get; set; }

        public static void StartLoad()
        {
            LoadNum = MgrTypeArr.Length;

            MessageAggregator<object>.Instance.Subscribe(MessageType.TableMgrLoadFinish, DownloadFinish);

            foreach (var m in MgrTypeArr)
            {
                TableMgrContainer.RegisterSingleton(m);
            }
        }

        private static void DownloadFinish(object sender, MessageArgs<object> args)
        {
            if (--LoadNum == 0)
            {
                DownloadFinishCallabck?.Invoke();

                MessageAggregator<object>.Instance.Remove(MessageType.TableMgrLoadFinish);
            }
        }
    }
}