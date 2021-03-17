using System.Threading.Tasks;

namespace Omnana
{
    public class TestServicer : ServiceBase
    {
        public RestMgr RestService;


        public override void Loaded()
        {
            base.Loaded();

            RestService = RestMgr.Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> Test()
        {
            var data = await RestService.HttpGet<object>("www.baidu.com");

            return "Test";
        }
    }
}
