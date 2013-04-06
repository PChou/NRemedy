using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRemedy;
using ARNative;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class Login_Test
    {
        #region Test Package
        protected static string TestServer = "192.168.1.101";
        protected static string TestAdmin = "Demo";
        protected static string TestAdminPwd = "123abc";

        //protected static string TestServer = "192.168.1.100";
        //protected static string TestAdmin = "Demo";
        //protected static string TestAdminPwd = "123";

        private static string TestChineseUser = "周平";
        private static string TestChineseUserPwd = "123";

        #endregion


        [TestMethod]
        public void Login_with_admin_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);
            }
            finally
            {
                context.Dispose();
            }
        }

        [TestMethod]
        public void Login_with_admin_failed_cause_bad_pwd()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd + "something");
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
            }
            catch (ARException ex)
            {
                Assert.AreEqual(623, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  72

            }
            finally
            {
                context.Dispose();
            }
        }

        [TestMethod]
        public void Login_with_admin_failed_cause_bad_user()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin + "something", TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
            }
            catch (ARException ex)
            {
                Assert.AreEqual(623, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  72
            }
            finally
            {
                context.Dispose();
            }
        }

        [TestMethod]
        public void Login_with_admin_failed_cause_bad_server()
        {
            ARLoginContext context = new ARLoginContext(TestServer + "something", TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
            }
            catch (ARException ex)
            {
                Assert.AreEqual(90, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  72
            }
            finally
            {
                context.Dispose();
            }
        }

        [TestMethod]
        public void Login_with_chinese_admin_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestChineseUser, TestChineseUserPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            finally
            {
                context.Dispose();
            }
        }

        [TestMethod]
        public void Login_with_admin_failed_and_try_a_success_again()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestChineseUser, TestChineseUserPwd + "something");
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
            }
            catch (ARException ex)
            {
                Assert.AreEqual(623, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  72
                try
                {
                    context.Password = TestChineseUserPwd;
                    ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                }
                catch
                {
                    Assert.AreEqual(null, ex);
                }
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}
