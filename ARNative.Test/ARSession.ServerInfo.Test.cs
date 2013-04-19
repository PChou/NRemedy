using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ARNative;

namespace ARNative.Test
{
    [TestClass]
    public class ARSession_ServerInfo_Test : ARSessionConfig
    {
        [TestMethod]
        public void ARSession_ServerInfo_Get_SERVER_INFO_MAX_ENTRIES()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<uint> tar = new List<uint>();
                tar.Add((uint)ServerInfoType.SERVER_INFO_MAX_ENTRIES);
                var result = session.GetServerInfo(tar.ToArray());
                Assert.AreEqual(1,result.Count);
                Assert.AreEqual(ServerInfoType.SERVER_INFO_MAX_ENTRIES, result[0].Type);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            finally
            {
                session.LogOut();
            }


        }
    }
}
