using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRemedy.Core;
using System.Linq;
using NRemedy.Linq;
using ARNative;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class ARProxyLambdaExtension_Delete_Test : RegularConfig
    {
        [ClassInitialize]
        public static void Initialize(TestContext context2)
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                //int totalMatch = -1;
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var models = proxy.GetEntryList(
                    null,
                    new string[] { "RequestID" },
                    null,
                    0,
                    null,
                    null);
                foreach (var model in models)
                {
                    proxy.DeleteEntry(model);
                }

                NRemedy_Test_Regular_Form newentry = new NRemedy_Test_Regular_Form();
                newentry.CharacterField = TestCharacterFieldValue;
                newentry.Radio_Button_Field = null;
                newentry.IntegerField = 1;
                for (int i = 0; i < 7; i++)
                {
                    proxy.CreateEntry(newentry);
                }

                newentry.CharacterField = TestCharacterFieldValueChinese;
                newentry.IntegerField = 2;
                for (int i = 0; i < 9; i++)
                {
                    proxy.CreateEntry(newentry);
                }

                newentry.CharacterField = TestCharacterFieldValueChinese + "Set Something";
                newentry.IntegerField = 3;
                for (int i = 0; i < 2; i++)
                {
                    proxy.CreateEntry(newentry);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("env init in Construct error.", ex);
            }
            finally
            {
                context.Dispose();
            }
        }

        [TestMethod]
        public void DeleteEntries_single_condition()
        {
            ARLoginContext context = null;
            try
            {
                context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField == TestCharacterFieldValue
                        select s.RequestID;
                Assert.AreEqual(7, q.Count());

                proxy.DeleteEntryList(m => m.CharacterField == TestCharacterFieldValue);

                //after delete the count should be 0
                Assert.AreEqual(0, q.Count());
            }
            finally {
                if (context != null)
                    context.Dispose();
            }
        }

        [TestMethod]
        public void DeleteEntries_string_contains_condition()
        {
            ARLoginContext context = null;
            try
            {
                context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField.Contains(TestCharacterFieldValueChinese + "%") && s.IntegerField == 3
                        select s.RequestID;
                Assert.AreEqual(2, q.Count());

                proxy.DeleteEntryList(m => m.CharacterField.Contains(TestCharacterFieldValueChinese + "%") && m.IntegerField == 3);

                //after delete the count should be 0
                Assert.AreEqual(0, q.Count());
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        [TestMethod]
        public void DeleteEntries_constant_condition_true()
        {
            ARLoginContext context = null;
            try
            {
                context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);

                NRemedy_Test_Regular_Form newentry = new NRemedy_Test_Regular_Form();
                newentry.CharacterField = TestCharacterFieldValue;
                newentry.Radio_Button_Field = null;
                newentry.IntegerField = 1;
                for (int i = 0; i < 7; i++)
                {
                    proxy.CreateEntry(newentry);
                }

                newentry.CharacterField = TestCharacterFieldValueChinese;
                newentry.IntegerField = 2;
                for (int i = 0; i < 9; i++)
                {
                    proxy.CreateEntry(newentry);
                }

                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        select s.RequestID;
                int count = q.Count();
                Assert.IsTrue(count > 0);

                proxy.DeleteEntryList(m => true);

                //after delete the count should be 0
                Assert.AreEqual(0, q.Count());
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


    }
}
