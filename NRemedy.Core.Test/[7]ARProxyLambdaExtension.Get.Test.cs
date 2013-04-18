using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRemedy.Core;
using System.Linq;
using NRemedy.Linq;
using ARNative;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class ARProxyLambdaExtension_Get_Test : RegularConfig
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
        public void GetEntry_by_Expression_single_condition()
        {
            ARLoginContext context = null;
            try
            {
                context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form
                {
                    CharacterField = "should not changed",
                    Status = NRemedy_Test_Regular_Form.Status_Enum.Fixed,
                    DecimalNumberField = 3.13m,
                    IntegerField = 10
                }
                );

                Assert.AreNotEqual(null, entryId);

                NRemedy_Test_Regular_Form entry = proxy.GetEntry(entryId,
                    m => m.CharacterField,
                    m => m.Status,
                    m => m.DecimalNumberField
                    );

                Assert.AreEqual("should not changed", entry.CharacterField);
                Assert.AreEqual(NRemedy_Test_Regular_Form.Status_Enum.Fixed, entry.Status);
                Assert.AreEqual(3.13m, entry.DecimalNumberField);
                Assert.AreEqual(null, entry.IntegerField);

            }
            finally {
                if (context != null)
                    context.Dispose();
            }
        }


    }
}
