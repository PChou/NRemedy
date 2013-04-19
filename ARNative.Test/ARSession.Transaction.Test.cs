using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARNative;
using System.Collections.Generic;
using System.Linq;

namespace ARNative.Test
{
    [TestClass]
    public class ARSession_Transaction_Test : ARRegularConfig
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
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
        public void ARSession_Transcation_Create_Create_Success()
        {
            ARSession session = new ARSession();
            try{
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId,(object)TestCharacterFieldValue,ARDataType.DATA_TYPE_CHAR));

                //begin Transaction
                session.BeginBulkEntryTransaction();
                //create
                session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //let the second call success too
                //valuelist.Add(new ARFieldValue(7u, null, ARDataType.DATA_TYPE_NULL));
                session.CreateEntry(TestRegularFormName, valuelist.ToArray());
                //commit
                var result = session.EndBulkEntryTransaction(1);

                Assert.IsTrue(result.Success);
                Assert.AreEqual(2, result.ResultList.Count);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_CREATE, result.ResultList[0].CallType);
                var entryid1 = result.ResultList[0].EntryId;
                Assert.IsNull(result.ResultList[0].Status);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_CREATE, result.ResultList[1].CallType);
                var entryid2 = result.ResultList[1].EntryId;
                Assert.IsNull(result.ResultList[1].Status);


                List<string> entryIds = new List<string>();
                entryIds.Add(entryid1);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestCharacterFieldId);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(TestCharacterFieldValue, 
                    Entry.First(f => f.FieldId == TestCharacterFieldId).Value.ToString());

                entryIds.Clear();
                entryIds.Add(entryid2);
                List<ARFieldValue> Entry2 = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(TestCharacterFieldValue, 
                    Entry2.First(f => f.FieldId == TestCharacterFieldId).Value.ToString());

            }
            catch(ARException ex)
            {                
                Assert.AreEqual(null, ex);
            }
            session.LogOut();

        }

        [TestMethod]
        public void ARSession_Transcation_Create_Create_Failed()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValue, ARDataType.DATA_TYPE_CHAR));

                //begin Transaction
                session.BeginBulkEntryTransaction();
                //create
                session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //let the second call failed
                valuelist.Add(new ARFieldValue(7u, null, ARDataType.DATA_TYPE_NULL));
                session.CreateEntry(TestRegularFormName, valuelist.ToArray());
                //commit
                var result = session.EndBulkEntryTransaction(1);

                Assert.IsFalse(result.Success);
                Assert.AreEqual(2, result.ResultList.Count);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_CREATE, result.ResultList[0].CallType);
                var entryid1 = result.ResultList[0].EntryId;
                Assert.IsNull(result.ResultList[0].Status);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_CREATE, result.ResultList[1].CallType);
                Assert.AreEqual(null, result.ResultList[1].EntryId);
                Assert.IsNotNull(result.ResultList[1].Status);
                Assert.AreEqual(326, result.ResultList[1].Status.MessageNumber);

                List<string> entryIds = new List<string>();
                entryIds.Add(entryid1);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestCharacterFieldId);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.IsNull(Entry);

            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();

        }

        [TestMethod]
        public void ARSession_Transcation_Create_Set_Success()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValue, ARDataType.DATA_TYPE_CHAR));
                var entrytobeset = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //begin Transaction
                session.BeginBulkEntryTransaction();
                //create
                session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                List<string> entryid = new List<string>() { entrytobeset };
                List<ARFieldValue> up_fvs = new List<ARFieldValue>();
                up_fvs.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValueChinese, ARDataType.DATA_TYPE_CHAR));
                //let the second call success too
                session.SetEntry(TestRegularFormName, entryid.ToArray(), up_fvs.ToArray());
                //commit
                var result = session.EndBulkEntryTransaction(1);

                Assert.IsTrue(result.Success);
                Assert.AreEqual(2, result.ResultList.Count);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_CREATE, result.ResultList[0].CallType);
                var entryid1 = result.ResultList[0].EntryId;
                Assert.IsNull(result.ResultList[0].Status);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_SET, result.ResultList[1].CallType);
                Assert.IsNull(result.ResultList[1].Status);


                List<string> entryIds = new List<string>();
                entryIds.Add(entryid1);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestCharacterFieldId);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(TestCharacterFieldValue,
                    Entry.First(f => f.FieldId == TestCharacterFieldId).Value.ToString());


                List<ARFieldValue> Entry2UP = session.GetEntry(TestRegularFormName, entryid.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(TestCharacterFieldValueChinese,
                    Entry2UP.First(f => f.FieldId == TestCharacterFieldId).Value.ToString());

            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }


        [TestMethod]
        public void ARSession_Transcation_Create_Set_Failed()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValue, ARDataType.DATA_TYPE_CHAR));
                

                //begin Transaction
                session.BeginBulkEntryTransaction();
                //create
                session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                List<string> entryid = new List<string>() { "111111111111" };//entryid do not exist
                List<ARFieldValue> up_fvs = new List<ARFieldValue>();
                up_fvs.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValueChinese, ARDataType.DATA_TYPE_CHAR));
                //let the second call failed
                session.SetEntry(TestRegularFormName, entryid.ToArray(), up_fvs.ToArray());
                //commit
                var result = session.EndBulkEntryTransaction(1);

                Assert.IsFalse(result.Success);
                Assert.AreEqual(2, result.ResultList.Count);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_CREATE, result.ResultList[0].CallType);
                var entryid1 = result.ResultList[0].EntryId;
                Assert.IsNull(result.ResultList[0].Status);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_SET, result.ResultList[1].CallType);
                Assert.IsNotNull(result.ResultList[1].Status);
                //item do not exist in the db
                Assert.AreEqual(302, result.ResultList[1].Status.MessageNumber);

            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void ARSession_Transcation_Create_Delete_Failed()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValue, ARDataType.DATA_TYPE_CHAR));


                //begin Transaction
                session.BeginBulkEntryTransaction();
                //create
                session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                List<string> entryid = new List<string>() { "111111111111" };//entryid do not exist
                //let the second call failed
                session.DeleteEntry(TestRegularFormName, entryid.ToArray());
                //commit
                var result = session.EndBulkEntryTransaction(1);

                Assert.IsFalse(result.Success);
                Assert.AreEqual(2, result.ResultList.Count);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_CREATE, result.ResultList[0].CallType);
                var entryid1 = result.ResultList[0].EntryId;
                Assert.IsNull(result.ResultList[0].Status);

                Assert.AreEqual(EntryCallType.BULK_ENTRY_DELETE, result.ResultList[1].CallType);
                Assert.IsNotNull(result.ResultList[1].Status);
                //item do not exist in the db
                Assert.AreEqual(302, result.ResultList[1].Status.MessageNumber);

            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void ARSession_Transcation_Create_Set_Cancel()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValue, ARDataType.DATA_TYPE_CHAR));
                var entrytobeset = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //begin Transaction
                session.BeginBulkEntryTransaction();
                //create
                session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                List<string> entryid = new List<string>() { entrytobeset };
                List<ARFieldValue> up_fvs = new List<ARFieldValue>();
                up_fvs.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValueChinese, ARDataType.DATA_TYPE_CHAR));
                //let the second call success too
                session.SetEntry(TestRegularFormName, entryid.ToArray(), up_fvs.ToArray());
                //commit
                var result = session.EndBulkEntryTransaction(2);



                Assert.IsTrue(result.Success);

            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }
    }
}
