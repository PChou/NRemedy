using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARNative;

namespace ARNative.Test
{
    [TestClass]    
    public class ARSession_Login_Test
    {
        #region Test Package
        protected static string TestServer = "192.168.1.101";
        protected static string TestAdmin = "Demo";
        protected static string TestAdminPwd = "123abc";

        //protected static string TestServer = "192.168.1.102";
        //protected static string TestAdmin = "Demo";
        //protected static string TestAdminPwd = "123";

        private static string TestChineseUser = "周平";
        private static string TestChineseUserPwd = "123";

        #endregion


        [TestMethod]
        [TestCategory("Login")]
        public void Login_with_admin_success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void Login_with_admin_failed_cause_bad_pwd()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd + "something");
            }
            catch (ARException ex)
            {
                Assert.AreEqual(623, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  72

            }
            session.LogOut();
        }

        [TestMethod]
        public void Login_with_admin_failed_cause_bad_user()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin + "something", TestAdminPwd + "something");
            }
            catch (ARException ex)
            {
                Assert.AreEqual(623, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  72
            }
            session.LogOut();
        }

        [TestMethod]
        public void Login_with_admin_failed_cause_bad_server()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer + "something", TestAdmin , TestAdminPwd);
            }
            catch (ARException ex)
            {
                Assert.AreEqual(90, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  28
            }
            session.LogOut();
        }

        [TestMethod]
        public void Login_with_chinese_admin_success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestChineseUser, TestChineseUserPwd);
            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void Login_with_admin_failed_and_try_a_success_again()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd + "something");
            }
            catch (ARException ex)
            {
                Assert.AreEqual(623, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  72
                try
                {
                    session.Login(TestServer, TestAdmin, TestAdminPwd);
                }
                catch
                {
                    Assert.AreEqual(null, ex);
                }
            }
            session.LogOut();
        }
    }
}
