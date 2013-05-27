using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARNative;

namespace ARNative.Test
{
    /// <summary>
    /// Summary description for ARSession_GetARSchema_Test
    /// </summary>
    [TestClass]
    public class ARSession_GetARSchema_Test : ARSessionConfig
    {
        public ARSession_GetARSchema_Test()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<String> list = session.GetListSchema();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }


        [TestMethod]
        public void TestMethod2()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                ARForm form = session.GetSchema("ARNative_Test_Form");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void TestMethod3()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<uint> list = session.GetListField("ARNative_Test_Form");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void TestMethod4()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                uint fieldid = 536870927;
                ARField field = session.GetField("ARNative_Test_Form", fieldid);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }
    }
}
