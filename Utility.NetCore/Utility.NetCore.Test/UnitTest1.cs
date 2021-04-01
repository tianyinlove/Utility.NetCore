using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using System;
using Utility.Extensions;
using Utility.NetLocker;

namespace Utility.NetCore.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        //[TestCase("group_ticket=WSfTOWPg35xxxxxxxxxCi0TOCw-ZdFnLm4Ggvssk4suhakGwB7AD64&noncestr=Wm3WZYTPz0wzccnW&timestamp=1447334894&url=http://your.website.com", "f211c9c1b375aae79ad74685450149825b2b2353")]
        [TestCase("jsapi_ticket=sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg&noncestr=Wm3WZYTPz0wzccnW&timestamp=1414587457&url=https://mobiletest.emoney.cn/appstatic/matches/2020/new-user-share-mp/?IsServiceApi=&activityCode=20200901&acountId=2&passKey=&appId=4&code=0315Bh0001H4iK1TGQ1004FR6j25Bh0X&state=STATE", "2ce4a2bfe8abd625a8f29a0ab886aeef86712f75")]
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