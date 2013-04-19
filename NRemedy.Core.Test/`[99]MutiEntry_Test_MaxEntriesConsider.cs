using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NRemedy.Core.Test
{
    //!!@@Max Entries Returned By GetList must 100
    [TestClass]
    public class MutiEntry_Test_MaxEntriesConsider : RegularConfig
    {
        private static void Clean()
        {
            ARLoginContext context = null;
            try
            {
                context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
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
        public void ARProxy_GetListEntry_MaxEntries_Cover_notCover()
        {
            Clean();
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                NRemedy_Test_Regular_Form newentry = new NRemedy_Test_Regular_Form();
                newentry.CharacterField = TestCharacterFieldValue;
                newentry.IntegerField = 1;
                for (int i = 0; i < 588; i++)
                {
                    proxy.CreateEntry(newentry);
                }

                var results = proxy.GetEntryList(
                    null,
                    new string[] { "RequestID" },
                    null,
                    0,
                    null,
                    null);
                Assert.AreEqual(588, results.Count);


                var results2 = proxy.GetEntryList(
                    null,
                    new string[] { "RequestID" },
                    300,
                    10,
                    null,
                    null);
                Assert.AreEqual(300, results2.Count);

                var results3 = proxy.GetEntryList(
                    null,
                    new string[] { "RequestID" },
                    100,
                    10,
                    null,
                    null);
                Assert.AreEqual(100, results3.Count);

                for (int i = 0; i < 100; i++)
                {
                    Assert.AreEqual(results3[i].RequestID, results2[i].RequestID);
 
                }


                var results4 = proxy.GetEntryList(
                    null,
                    new string[] { "RequestID" },
                    200,
                    400,
                    null,
                    null);
                Assert.AreEqual(188, results4.Count);

                var results5 = proxy.GetEntryList(
                    null,
                    new string[] { "RequestID" },
                    null,
                    400,
                    null,
                    null);
                Assert.AreEqual(188, results5.Count);

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

    }
}
