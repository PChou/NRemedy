using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ARNative;

namespace ARNative.Test
{
    [TestClass]
    public class ARSession_Group_Test : ARSessionConfig
    {
        [TestMethod]
        public void ARSession_ServerInfo_Get_SERVER_INFO_MAX_ENTRIES()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                var list = session.GetUserGroupList("luy");

                
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
