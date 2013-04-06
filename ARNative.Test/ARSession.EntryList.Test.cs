using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARNative;
using System.Collections.Generic;
using System.Linq;

namespace ARNative.Test
{
    //this test is for GetListEntryStatictisc/GetEntryList
    [TestClass]
    
    public class ARSession_EntryList_Test : ARSession_CreateEntry_Test
    {
        //#region Test Package
        //protected static string TestRegularFormName = "NRemedy_Test_Regular_Form";
        //protected static uint TestCharacterFieldId = 20000001;
        //protected static uint TestIntFieldId = 20000002;
        //protected static uint TestCharacterFieldIdNotExist = 745678897;

        //protected static string TestCharacterFieldValue = "Hello Remedy";
        //protected static string TestCharacterFieldValueChinese = "你好 Remedy";


        //#endregion


        //the following test need enviroment below
        //Dictionary<String,int> assertTable = new Dictionary<string,int>();
        //assertTable.Add("Hello Remedy",7);
        //assertTable.Add("你好 RemedySet Something",2);
        //assertTable.Add("你好 Remedy",9);
        //all integer filed is 1

        /// <summary>
        /// perform env ready
        /// include clear all data in TestRegularFormName and add init data for statictisc
        /// </summary>
        public ARSession_EntryList_Test()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                int totalCount = -1;
                List<AREntry> allentries = session.GetEntryList(
                    TestRegularFormName,
                    null,
                    null,
                    null,
                    null,
                    ref totalCount,
                    null);

                foreach (AREntry entry in allentries)
                {
                    session.DeleteEntry(TestRegularFormName, entry.EntryIds.ToArray());
                }

                //add test data
                List<ARFieldValue> fv1 = new List<ARFieldValue>();
                fv1.Add(new ARFieldValue(TestCharacterFieldId,(object)TestCharacterFieldValue,ARDataType.DATA_TYPE_CHAR));
                fv1.Add(new ARFieldValue(TestIntFieldId,1,ARDataType.DATA_TYPE_INTEGER));
                List<ARFieldValue> fv2 = new List<ARFieldValue>();
                fv2.Add(new ARFieldValue(TestCharacterFieldId,(object)TestCharacterFieldValueChinese,ARDataType.DATA_TYPE_CHAR));
                fv2.Add(new ARFieldValue(TestIntFieldId,1,ARDataType.DATA_TYPE_INTEGER));
                List<ARFieldValue> fv3 = new List<ARFieldValue>();
                fv3.Add(new ARFieldValue(TestCharacterFieldId,(object)(TestCharacterFieldValueChinese + "Set Something"),ARDataType.DATA_TYPE_CHAR));
                fv3.Add(new ARFieldValue(TestIntFieldId,1,ARDataType.DATA_TYPE_INTEGER));

                for (int i = 0; i < 7; i++)
                    session.CreateEntry(TestRegularFormName, fv1.ToArray());
                for (int i = 0; i < 9; i++)
                    session.CreateEntry(TestRegularFormName, fv2.ToArray());
                for (int i = 0; i < 2; i++)
                    session.CreateEntry(TestRegularFormName, fv3.ToArray());

            }
            catch (ARException ex)
            {

            }
            finally
            {
                session.LogOut();
            }
        }


        [TestMethod]
        [TestCategory("MultiEntry")]
        public void GetEntryListStatictisc_count_noqulifier_nogroup_success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                var retlist = session.GetListEntryStatictisc(
                    TestRegularFormName,
                    null,//"'Character Field' = \"Hello Remedy\"",
                    ARStatictisc.STAT_OP_COUNT,
                    null,
                    null
                    );
                Assert.AreEqual(1, retlist.Count);
                Assert.AreEqual(18, Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(null, retlist[0].GroupByValues);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void GetEntryListStatictisc_count_qulifier_nogroup_success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                var retlist = session.GetListEntryStatictisc(
                    TestRegularFormName,
                    "'Character Field' = \"Hello Remedy\"",
                    ARStatictisc.STAT_OP_COUNT,
                    null,
                    null
                    );
                Assert.AreEqual(1, retlist.Count);
                Assert.AreEqual(7, Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(null, retlist[0].GroupByValues);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void GetEntryListStatictisc_count_noqulifier_group_success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                var retlist = session.GetListEntryStatictisc(
                    TestRegularFormName,
                    null,
                    ARStatictisc.STAT_OP_COUNT,
                    null,
                    groupIds.ToArray()
                    );
                Assert.AreEqual(3, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 2);
                assertTable.Add("你好 Remedy", 9);

                Assert.AreEqual(assertTable[retlist[0].GroupByValues[0].ToString()], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].GroupByValues[0].ToString()], Convert.ToInt32(retlist[1].Statictisc));
                Assert.AreEqual(assertTable[retlist[2].GroupByValues[0].ToString()], Convert.ToInt32(retlist[2].Statictisc));
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void GetEntryListStatictisc_count_qulifier_group_success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                var retlist = session.GetListEntryStatictisc(
                    TestRegularFormName,
                    "'Character Field' LIKE \"%你好%\"",
                    ARStatictisc.STAT_OP_COUNT,
                    null,
                    groupIds.ToArray()
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 2);
                assertTable.Add("你好 Remedy", 9);

                Assert.AreEqual(assertTable[retlist[0].GroupByValues[0].ToString()], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].GroupByValues[0].ToString()], Convert.ToInt32(retlist[1].Statictisc));

            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void GetEntryListStatictisc_sum_qulifier_group_success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                var retlist = session.GetListEntryStatictisc(
                    TestRegularFormName,
                    "'Character Field' LIKE \"%你好%\"",
                    ARStatictisc.STAT_OP_SUM,
                    TestIntFieldId,
                    groupIds.ToArray()
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 2);
                assertTable.Add("你好 Remedy", 9);

                Assert.AreEqual(assertTable[retlist[0].GroupByValues[0].ToString()], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].GroupByValues[0].ToString()], Convert.ToInt32(retlist[1].Statictisc));

            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void GetEntryListStatictisc_max_qulifier_group_success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                var retlist = session.GetListEntryStatictisc(
                    TestRegularFormName,
                    "'Character Field' LIKE \"%你好%\"",
                    ARStatictisc.STAT_OP_MAXIMUM,
                    TestIntFieldId,
                    groupIds.ToArray()
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 1);
                assertTable.Add("你好 Remedy", 1);

                Assert.AreEqual(assertTable[retlist[0].GroupByValues[0].ToString()], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].GroupByValues[0].ToString()], Convert.ToInt32(retlist[1].Statictisc));

            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();

        }

        [TestMethod]
        public void GetEntryListStatictisc_min_qulifier_group_success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> groupIds = new List<UInt32>();
                groupIds.Add(TestCharacterFieldId);
                var retlist = session.GetListEntryStatictisc(
                    TestRegularFormName,
                    "'Character Field' LIKE \"%你好%\"",
                    ARStatictisc.STAT_OP_MINIMUM,
                    TestIntFieldId,
                    groupIds.ToArray()
                    );
                Assert.AreEqual(2, retlist.Count);
                Dictionary<String, int> assertTable = new Dictionary<string, int>();
                //assertTable.Add("Hello Remedy", 7);
                assertTable.Add("你好 RemedySet Something", 1);
                assertTable.Add("你好 Remedy", 1);

                Assert.AreEqual(assertTable[retlist[0].GroupByValues[0].ToString()], Convert.ToInt32(retlist[0].Statictisc));
                Assert.AreEqual(assertTable[retlist[1].GroupByValues[0].ToString()], Convert.ToInt32(retlist[1].Statictisc));

            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }



        /* Get Entry List */

        [TestMethod]
        public void GetEntryList_qulifier_fieldIds_nopage_nocount_noorder_success()
        {
            ARSession session = new ARSession();
            try
            {
                int total = -1;
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
                //fieldIds.Add(1);
                var retlist = session.GetEntryList(
                    TestRegularFormName,
                    "'Character Field' LIKE \"%你好%\"",
                    fieldIds.ToArray(),
                    null,
                    null,
                    ref total,
                    null
                    );
                Assert.AreEqual(11, retlist.Count);
                Assert.AreEqual(-1, total);
                foreach (var entry in retlist)
                {
                    //Assert.AreEqual(entry.EntryIds.First(), entry.FieldValues.First(f => f.FieldId == 1).Value.ToString());
                    Assert.IsTrue(entry.FieldValues.First(f => f.FieldId == TestCharacterFieldId).Value.ToString().Contains("你好"));
                }

            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void GetEntryList_qulifier_fieldIds_haspage_nocount_noorder_success()
        {
            ARSession session = new ARSession();
            try
            {
                int total = -1;
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
                var retlist = session.GetEntryList(
                    TestRegularFormName,
                    "'Character Field' LIKE \"%你好%\"",
                    fieldIds.ToArray(),
                    0,
                    5,
                    ref total,
                    null
                    );
                Assert.AreEqual(5, retlist.Count);
                Assert.AreEqual(-1, total);
                foreach (var entry in retlist)
                {
                    //Assert.AreEqual(entry.EntryIds.First(), entry.FieldValues.First(f => f.FieldId == 1).Value.ToString());
                    Assert.IsTrue(entry.FieldValues.First(f => f.FieldId == TestCharacterFieldId).Value.ToString().Contains("你好"));
                }

                string maxentrylist = retlist.Max(e => e.EntryIds.First());
                string minentrylist = retlist.Min(e => e.EntryIds.First());
                Assert.AreEqual(4, Convert.ToInt32(maxentrylist) - Convert.ToInt32(minentrylist));

            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void GetEntryList_qulifier_fieldIds_haspage_hascount_noorder_success()
        {
            ARSession session = new ARSession();
            try
            {
                int total = 0;
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
                var retlist = session.GetEntryList(
                    TestRegularFormName,
                    "'Character Field' LIKE \"%你好%\"",
                    fieldIds.ToArray(),
                    0,
                    5,
                    ref total,
                    null
                    );
                Assert.AreEqual(5, retlist.Count);
                Assert.AreEqual(11, total);
                foreach (var entry in retlist)
                {
                    //Assert.AreEqual(entry.EntryIds.First(), entry.FieldValues.First(f => f.FieldId == 1).Value.ToString());
                    Assert.IsTrue(entry.FieldValues.First(f => f.FieldId == TestCharacterFieldId).Value.ToString().Contains("你好"));
                }

                string maxentrylist = retlist.Max(e => e.EntryIds.First());
                string minentrylist = retlist.Min(e => e.EntryIds.First());
                Assert.AreEqual(4, Convert.ToInt32(maxentrylist) - Convert.ToInt32(minentrylist));

            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void GetEntryList_qulifier_fieldIds_nopage_hascount_hasorder_success()
        {
            ARSession session = new ARSession();
            try
            {
                int total = 0;
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<UInt32> fieldIds = new List<UInt32>();
                fieldIds.Add(TestCharacterFieldId);
                fieldIds.Add(TestIntFieldId);
                List<ARSortInfo> sortInfo = new List<ARSortInfo>();
                sortInfo.Add(new ARSortInfo {
                    FieldId = TestCharacterFieldId,
                    Order = SortOrder.SORT_DESCENDING
                });
                var retlist = session.GetEntryList(
                    TestRegularFormName,
                    "'Character Field' LIKE \"%你好%\"",
                    fieldIds.ToArray(),
                    null,
                    null,
                    ref total,
                    sortInfo.ToArray()
                    );
                Assert.AreEqual(11, retlist.Count);
                Assert.AreEqual(11, total);
                foreach (var entry in retlist)
                {
                    //Assert.AreEqual(entry.EntryIds.First(), entry.FieldValues.First(f => f.FieldId == 1).Value.ToString());
                    Assert.IsTrue(entry.FieldValues.First(f => f.FieldId == TestCharacterFieldId).Value.ToString().Contains("你好"));
                }

                string maxentrylist = retlist.Max(e => e.EntryIds.First());
                string minentrylist = retlist.Min(e => e.EntryIds.First());
                Assert.AreEqual(10, Convert.ToInt32(maxentrylist) - Convert.ToInt32(minentrylist));

                Assert.AreEqual(TestCharacterFieldValueChinese + "Set Something", retlist[0].FieldValues[0].Value);
                Assert.AreEqual(TestCharacterFieldValueChinese + "Set Something", retlist[1].FieldValues[0].Value);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }
    }
}
