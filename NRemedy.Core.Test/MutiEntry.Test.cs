using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using ARNative;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class MutiEntry_Test : SingleEntry_Test
    {
        //#region Test Package
        //protected static string TestServer = "172.16.121.13";
        //protected static string TestAdmin = "parkerz.admin";
        //protected static string TestAdminPwd = "admin";

        ////protected static string TestServer = "192.168.1.100";
        ////protected static string TestAdmin = "Demo";
        ////protected static string TestAdminPwd = "123";

        //private static string TestChineseUser = "周平";
        //private static string TestChineseUserPwd = "123";

        //#endregion

        public MutiEntry_Test()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                int totalMatch = -1;
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var models = proxy.GetEntryList(
                    null,
                    null,
                    null,
                    null,
                    ref totalMatch,
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
                int totalMatch = -1;
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var models = proxy.GetEntryList(
                    "'Character Field' = \"Hello Remedy\"",
                    fieldIds,
                    null,
                    null,
                    ref totalMatch,
                    null);

                Assert.AreEqual(7, models.Count);
                foreach (var model in models)
                {
                    Assert.AreEqual("Hello Remedy", model.CharacterField);
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
                int total = -1;
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetEntryList(
                    "'Character Field' LIKE \"%你好%\"",
                    fieldIds,
                    0,
                    5,
                    ref total,
                    null
                    );
                //paged count should be 5,although total matched count is 11
                Assert.AreEqual(5, retlist.Count);
                Assert.AreEqual(-1, total);
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
                int total = 0;
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                var retlist = proxy.GetEntryList(
                    "'Character Field' LIKE \"%你好%\"",
                    fieldIds,
                    0,
                    5,
                    ref total,
                    null
                    );
                Assert.AreEqual(5, retlist.Count);
                Assert.AreEqual(11, total);
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
                int total = 0;
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
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
                    null,
                    null,
                    ref total,
                    sortInfo
                    );
                Assert.AreEqual(11, retlist.Count);
                Assert.AreEqual(11, total);
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

    }
}
