using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRemedy.Core;
using System.Linq;
using NRemedy.Linq;
using ARNative;
using System.Collections.Generic;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class ARProxyLambdaExtension_GetList_Test : RegularConfig
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
        public void GetEntryList_by_expression_qulifier_fieldIds_nopage_nocount_noorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var models = proxy.GetEntryList(
                    m => m.CharacterField == TestCharacterFieldValue,
                    0,
                    null,
                    null,
                    null,
                    m => m.CharacterField
                    );

                Assert.AreEqual(7, models.Count);
                foreach (var model in models)
                {
                    Assert.AreEqual(TestCharacterFieldValue, model.CharacterField);
                }

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
        public void GetEntryList_by_expression_qulifier_fieldIds_haspage_nocount_noorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetEntryList(
                    m => m.CharacterField.Contains("%你好%"),
                    0,
                    5,
                    null,
                    null,
                     m => m.CharacterField,
                     m => m.RequestID
                    );
                //paged count should be 5,although total matched count is 11
                Assert.AreEqual(5, retlist.Count);
                //Assert.AreEqual(-1, total);
                foreach (var entry in retlist)
                {
                    Assert.AreEqual(true, entry.CharacterField.Contains("你好"));
                }

                //assert the max id is greater 4 than min id
                //which means return the continous id list
                Assert.AreEqual(4, Convert.ToInt32(retlist.Max(m => m.RequestID)) - Convert.ToInt32(retlist.Min(m => m.RequestID)));

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
        public void GetEntryList_by_expression_qulifier_fieldIds_haspage_hascount_noorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                TotalMatch total = new TotalMatch();
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetEntryList(
                    m => m.CharacterField.Contains("%你好%"),
                    0,
                    5,
                    total,
                    null,
                     m => m.CharacterField,
                     m => m.RequestID
                    );
                Assert.AreEqual(5, retlist.Count);
                Assert.AreEqual(11, total.Value);
                foreach (var entry in retlist)
                {
                    Assert.AreEqual(true, entry.CharacterField.Contains("你好"));
                }

                Assert.AreEqual(4, Convert.ToInt32(retlist.Max(m => m.RequestID)) - Convert.ToInt32(retlist.Min(m => m.RequestID)));

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
        public void GetEntryList_by_expression_qulifier_fieldIds_nopage_hascount_hasorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                TotalMatch total = new TotalMatch();
                List<ARSortInfo> sortInfo = new List<ARSortInfo>();
                sortInfo.Add(new ARSortInfo
                {
                    FieldId = TestCharacterFieldId,
                    Order = SortOrder.SORT_DESCENDING
                });

                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);

                var retlist = proxy.GetEntryList(
                    m => m.CharacterField.Contains("%你好%"),
                    0,
                    null,
                    total,
                    sortInfo,
                     m => m.CharacterField,
                     m => m.RequestID
                    );
                Assert.AreEqual(11, retlist.Count);
                Assert.AreEqual(11, total.Value);
                foreach (var entry in retlist)
                {
                    Assert.AreEqual(true, entry.CharacterField.Contains("你好"));
                }

                Assert.AreEqual(10, Convert.ToInt32(retlist.Max(m => m.RequestID)) - Convert.ToInt32(retlist.Min(m => m.RequestID)));

                Assert.AreEqual(TestCharacterFieldValueChinese + "Set Something", retlist[0].CharacterField);
                Assert.AreEqual(TestCharacterFieldValueChinese + "Set Something", retlist[1].CharacterField);
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


    }
}
