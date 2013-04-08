using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARNative;
using System.Collections.Generic;
using System.Linq;

namespace ARNative.Test
{
    [TestClass]
    public class ARSession_Login_Test : ARSessionConfig
    {

        private static string TestChineseUser;
        private static string TestChineseUserPwd;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {

            //create a user which has login name with unicode character
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(101u, "周平", ARDataType.DATA_TYPE_CHAR));
                valuelist.Add(new ARFieldValue(8u, "周平", ARDataType.DATA_TYPE_CHAR));
                valuelist.Add(new ARFieldValue(102u, "123", ARDataType.DATA_TYPE_CHAR));
                session.CreateEntry("User", valuelist.ToArray());

                TestChineseUser = "周平";
                TestChineseUserPwd = "123";
            }
            finally
            {
                session.LogOut();
            }
 
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            //cleanup the user which has been created in Initialize
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add(1u);
                int total = -1;
                var entry = session.GetEntryList("User", "'101'=\"周平\"", fieldIds.ToArray(), null, null, ref total, null);
                if (entry != null || entry.Count != 0){
                    foreach (var e in entry)
                        session.DeleteEntry("User", e.EntryIds.ToArray());
                }
            }
            finally
            {
                session.LogOut();
            }
        }

        [TestMethod]
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
