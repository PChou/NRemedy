using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using ARNative;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class MutiEntry_Test_Get : RegularConfig
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
            catch(Exception ex)
            {
                throw new Exception("env init in Construct error.", ex);
            }
            finally
            {
                context.Dispose();
            }
        }


        [TestMethod]
        public void ARProxy_GetEntryList_qulifier_fieldIds_nopage_nocount_noorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add(TestCharacterFieldId);
                //int totalMatch = -1;
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var models = proxy.GetEntryList(
                    "'Character Field' = \"Hello Remedy\"",
                    fieldIds,
                    0,
                    null,
                    null,
                    null);

                Assert.AreEqual(7, models.Count);
                foreach (var model in models)
                {
                    Assert.AreEqual(TestCharacterFieldValue, model.CharacterField);
                }

            }
            catch(Exception ex)
            {
                Assert.AreEqual(null, ex);
            }
            finally
            {
                context.Dispose();
            }
        }

        [TestMethod]
        public void ARProxy_GetEntryList_qulifier_fieldIds_haspage_nocount_noorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                //int total = -1;
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
                fieldIds.Add(1u);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetEntryList(
                    "'Character Field' LIKE \"%你好%\"",
                    fieldIds,
                    0,
                    5,
                    null,
                    null
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
        public void ARProxy_GetEntryList_qulifier_fieldIds_haspage_hascount_noorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                TotalMatch total = new TotalMatch();
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
                fieldIds.Add(1u);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetEntryList(
                    "'Character Field' LIKE \"%你好%\"",
                    fieldIds,
                    0,
                    5,
                    total,
                    null
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
        public void ARProxy_GetEntryList_qulifier_fieldIds_nopage_hascount_hasorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                TotalMatch total = new TotalMatch();
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
                fieldIds.Add(1u);
                List<ARSortInfo> sortInfo = new List<ARSortInfo>();
                sortInfo.Add(new ARSortInfo
                {
                    FieldId = TestCharacterFieldId,
                    Order = SortOrder.SORT_DESCENDING
                });

                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);

                var retlist = proxy.GetEntryList(
                    "'Character Field' LIKE \"%你好%\"",
                    fieldIds,
                    0,
                    null,
                    total,
                    sortInfo
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


        [TestMethod]
        public void ARProxy_GetEntryList_by_filter_qulifier_fieldIds_nopage_nocount_noorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var models = proxy.GetEntryList(
                    "'Character Field' = \"Hello Remedy\"",
                    0,
                    delegate(System.Reflection.PropertyInfo pi){
                        return pi.Name == "CharacterField";
                    },
                    null,
                    null,
                    null);

                Assert.AreEqual(7, models.Count);
                foreach (var model in models)
                {
                    Assert.AreEqual(TestCharacterFieldValue, model.CharacterField);
                    Assert.AreEqual(null, model.IntegerField);
                }

                var models2 = proxy.GetEntryList(
                    "'Character Field' = \"Hello Remedy\"",
                    0,
                    delegate(System.Reflection.PropertyInfo pi)
                    {
                        return pi.Name == "CharacterField" || pi.Name == "IntegerField";
                    },
                    null,
                    null,
                    null);

                Assert.AreEqual(7, models2.Count);
                foreach (var model in models2)
                {
                    Assert.AreEqual(TestCharacterFieldValue, model.CharacterField);
                    Assert.AreEqual(1, model.IntegerField);
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
        public void ARProxy_GetEntryList_by_Properties_Str_qulifier_fieldIds_nopage_nocount_noorder_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var models = proxy.GetEntryList(
                    "'Character Field' = \"Hello Remedy\"",
                    new string[]{ "CharacterField" },
                    null,
                    0,
                    null,
                    null);

                Assert.AreEqual(7, models.Count);
                foreach (var model in models)
                {
                    Assert.AreEqual(TestCharacterFieldValue, model.CharacterField);
                    Assert.AreEqual(null, model.IntegerField);
                }

                var models2 = proxy.GetEntryList(
                    "'Character Field' = \"Hello Remedy\"",
                    new string[] { "CharacterField", "IntegerField" },
                    null,
                    0,
                    null,
                    null);

                Assert.AreEqual(7, models2.Count);
                foreach (var model in models2)
                {
                    Assert.AreEqual(TestCharacterFieldValue, model.CharacterField);
                    Assert.AreEqual(1, model.IntegerField);
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

        //statictisc test
        //GetListEntryStatictisc
        [TestMethod]
        public void ARProxy_GetEntryListStatictisc_count_noqulifier_nogroup_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                var retlist = proxy.GetListEntryStatictisc(
                    null,//"'Character Field' = \"Hello Remedy\"",
                    ARStatictisc.STAT_OP_COUNT,
                    null,
                    null
                    );
                Assert.AreEqual(1, retlist.Count);
                Assert.AreEqual(18,Convert.ToInt32(retlist[0].Statictisc));
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
        public void ARProxy_GetEntryListStatictisc_count_qulifier_nogroup_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                List<UInt32> groupIds = new List<UInt32>();
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                groupIds.Add(TestCharacterFieldId);
                var retlist = proxy.GetListEntryStatictisc(
                    "'Character Field' = \"Hello Remedy\"",
                    ARStatictisc.STAT_OP_COUNT,
                    null,
                    null
                    );
                Assert.AreEqual(1, retlist.Count);
                Assert.AreEqual(7, Convert.ToInt32(retlist[0].Statictisc));
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
        public void ARProxy_GetEntryListStatictisc_count_noqulifier_group_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetListEntryStatictisc(
                    null,
                    ARStatictisc.STAT_OP_COUNT,
                    null,
                    groupIds
                    );
                Assert.AreEqual(3, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 2);
                assertTable.Add("你好 Remedy", 9);
                Assert.AreEqual(assertTable[retlist[0].CharacterField],Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].CharacterField], Convert.ToInt32(retlist[1].Statictisc));
                Assert.AreEqual(assertTable[retlist[2].CharacterField], Convert.ToInt32(retlist[2].Statictisc));
                
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
        public void ARProxy_GetEntryListStatictisc_count_qulifier_group_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetListEntryStatictisc(
                    "'Character Field' LIKE \"%你好%\"",
                    ARStatictisc.STAT_OP_COUNT,
                    null,
                    groupIds
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 2);
                assertTable.Add("你好 Remedy", 9);

                Assert.AreEqual(assertTable[retlist[0].CharacterField], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].CharacterField], Convert.ToInt32(retlist[1].Statictisc));

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
        public void ARProxy_GetEntryListStatictisc_sum_qulifier_group_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetListEntryStatictisc(
                    "'Character Field' LIKE \"%你好%\"",
                    ARStatictisc.STAT_OP_SUM,
                    TestIntFieldId,
                    groupIds
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 6);
                assertTable.Add("你好 Remedy", 18);

                Assert.AreEqual(assertTable[retlist[0].CharacterField], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].CharacterField], Convert.ToInt32(retlist[1].Statictisc));

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
        public void ARProxy_GetEntryListStatictisc_max_qulifier_group_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetListEntryStatictisc(
                    "'Character Field' LIKE \"%你好%\"",
                    ARStatictisc.STAT_OP_MAXIMUM,
                    TestIntFieldId,
                    groupIds
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 3);
                assertTable.Add("你好 Remedy", 2);

                Assert.AreEqual(assertTable[retlist[0].CharacterField], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].CharacterField], Convert.ToInt32(retlist[1].Statictisc));

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
        public void ARProxy_GetEntryListStatictisc_min_qulifier_group_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetListEntryStatictisc(
                    "'Character Field' LIKE \"%你好%\"",
                    ARStatictisc.STAT_OP_MINIMUM,
                    TestIntFieldId,
                    groupIds
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 3);
                assertTable.Add("你好 Remedy", 2);

                Assert.AreEqual(assertTable[retlist[0].CharacterField], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].CharacterField], Convert.ToInt32(retlist[1].Statictisc));

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
        public void ARProxy_GetEntryListStatictisc_avg_qulifier_group_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetListEntryStatictisc(
                    "'Character Field' LIKE \"%你好%\"",
                    ARStatictisc.STAT_OP_AVERAGE,
                    TestIntFieldId,
                    groupIds
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 3);
                assertTable.Add("你好 Remedy", 2);

                Assert.AreEqual(assertTable[retlist[0].CharacterField], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].CharacterField], Convert.ToInt32(retlist[1].Statictisc));

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
        public void ARProxy_GetEntryListStatictisc_by_Properties_Str_avg_qulifier_group_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetListEntryStatictisc(
                    "'Character Field' LIKE \"%你好%\"",
                    
                    TestIntFieldId,
                    new string[] { "CharacterField" },
                    ARStatictisc.STAT_OP_AVERAGE
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 3);
                assertTable.Add("你好 Remedy", 2);

                Assert.AreEqual(assertTable[retlist[0].CharacterField], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].CharacterField], Convert.ToInt32(retlist[1].Statictisc));

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
