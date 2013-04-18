using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using NRemedy.Linq;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class MutiEntry_Test_Set : RegularConfig
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
        public void SetEntries_single_condition()
        {
            ARLoginContext context = null;
            try
            {
                context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField == TestCharacterFieldValue
                        select s.IntegerField;
                Assert.AreEqual(7, q.Count());

                NRemedy_Test_Regular_Form model = new NRemedy_Test_Regular_Form();
                model.IntegerField = 10;//original is 1

                proxy.SetEntryList("'Character Field' = \"" + TestCharacterFieldValue + "\"", model, new string[] { "IntegerField" });

                foreach (var i in q)
                {
                    Assert.AreEqual(10, i);
                }
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        [TestMethod]
        public void SetEntries_All()
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
                        select s.IntegerField;

                int count = q.Count();
                Assert.IsTrue(count > 0);

                NRemedy_Test_Regular_Form model = new NRemedy_Test_Regular_Form();
                model.IntegerField = 110;//original is 1

                proxy.SetEntryList(null,model,new string[] { "IntegerField" });

                //after delete the count should be 0
                foreach(var i in q)
                {
                    Assert.AreEqual(110, i);
                }
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }
    }
}
