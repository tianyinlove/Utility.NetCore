using NUnit.Framework;
using Utility.Extensions;

namespace Utility.NetCore.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("group_ticket=WSfTOWPg35xxxxxxxxxCi0TOCw-ZdFnLm4Ggvssk4suhakGwB7AD64&noncestr=Wm3WZYTPz0wzccnW&timestamp=1447334894&url=http://your.website.com", "f211c9c1b375aae79ad74685450149825b2b2353")]
        [TestCase("jsapi_ticket=sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg&noncestr=Wm3WZYTPz0wzccnW&timestamp=1414587457&url=http://mp.weixin.qq.com", "0f9de62fce790f9a083d5c99e95740ceb90c27ed")]
        public void Test1(string data, string result)
        {
            var signature = data.Sha1();
            if (signature == result)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}