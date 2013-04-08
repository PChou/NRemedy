using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARNative;
using System.Collections.Generic;
using System.Linq;

namespace ARNative.Test
{
    //this test is for CreateEntry,GetEntry,DeleteEntry,SetEntry
    [TestClass]
    public class ARSession_CreateEntry_Test : ARRegularConfig
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer,TestAdmin,TestAdminPwd);
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
                    session.DeleteEntry(TestRegularFormName,entry.EntryIds.ToArray());
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
        [TestCategory("SingleEntry")]
        public void CreateEntry_simple_success()
        {
            ARSession session = new ARSession();
            try{
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId,(object)TestCharacterFieldValue,ARDataType.DATA_TYPE_CHAR));
                string entryid = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                List<string> entryIds = new List<string>();
                entryIds.Add(entryid);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestCharacterFieldId);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(TestCharacterFieldValue, 
                    Entry.First(f => f.FieldId == TestCharacterFieldId).Value.ToString());

            }
            catch(ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }


        [TestMethod]
        public void CreateEntry_form_not_found_303()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValue, ARDataType.DATA_TYPE_CHAR));
                session.CreateEntry(TestRegularFormName + "Y(HGYU", valuelist.ToArray());
            }
            catch (ARException ex)
            {
                Assert.AreEqual(303, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  49
            }
            session.LogOut();
        }

        [TestMethod]
        public void CreateEntry_field_not_found_311()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldIdNotExist, (object)TestCharacterFieldValue, ARDataType.DATA_TYPE_CHAR));
                session.CreateEntry(TestRegularFormName, valuelist.ToArray());
            }
            catch (ARException ex)
            {
                Assert.AreEqual(311, ex.MessageNumber); //183977-ErrorMsgs-7604.pdf page  50
            }
            session.LogOut();
        }

        [TestMethod]
        public void CreateEntry_datatype_character_chinese_with_delete()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValueChinese, ARDataType.DATA_TYPE_CHAR));
                string entryId = session.CreateEntry(TestRegularFormName, valuelist.ToArray());
                Assert.AreNotEqual(null, entryId);

                //GetEntry
                List<string> IdList = new List<string>();
                IdList.Add(entryId);
                List<uint> FIdList = new List<uint>();
                FIdList.Add((uint)TestCharacterFieldId);
                List<ARFieldValue> fieldsReturns = session.GetEntry(TestRegularFormName, IdList.ToArray(), FIdList.ToArray());
                Assert.AreEqual(1, fieldsReturns.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_CHAR, fieldsReturns[0].DataType);
                Assert.AreEqual(TestCharacterFieldId, fieldsReturns[0].FieldId);
                Assert.AreEqual(TestCharacterFieldValueChinese, fieldsReturns[0].Value.ToString());

                //DeleteEntry
                session.DeleteEntry(TestRegularFormName, IdList.ToArray());

                //GetAgain
                fieldsReturns.Clear();
                fieldsReturns = session.GetEntry(TestRegularFormName, IdList.ToArray(), FIdList.ToArray());
                Assert.IsNull(fieldsReturns);

                //DeleteAgain
                session.DeleteEntry(TestRegularFormName, IdList.ToArray());


            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void CreateEntry_datatype_character_chinese_with_set()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValueChinese, ARDataType.DATA_TYPE_CHAR));
                string entryId = session.CreateEntry(TestRegularFormName, valuelist.ToArray());
                Assert.AreNotEqual(null, entryId);

                //GetEntry
                List<string> IdList = new List<string>();
                IdList.Add(entryId);
                List<uint> FIdList = new List<uint>();
                FIdList.Add((uint)TestCharacterFieldId);
                List<ARFieldValue> fieldsReturns = session.GetEntry(TestRegularFormName, IdList.ToArray(), FIdList.ToArray());
                Assert.AreEqual(1, fieldsReturns.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_CHAR, fieldsReturns[0].DataType);
                Assert.AreEqual(TestCharacterFieldId, fieldsReturns[0].FieldId);
                Assert.AreEqual(TestCharacterFieldValueChinese, fieldsReturns[0].Value.ToString());

                //SetEntry
                fieldsReturns.Clear();
                fieldsReturns.Add(new ARFieldValue(TestCharacterFieldId,(object)(TestCharacterFieldValueChinese + "Set Something"),ARDataType.DATA_TYPE_CHAR));
                session.SetEntry(TestRegularFormName, IdList.ToArray(), fieldsReturns.ToArray());

                //GetAgain
                fieldsReturns.Clear();
                fieldsReturns = session.GetEntry(TestRegularFormName, IdList.ToArray(), FIdList.ToArray());
                Assert.AreEqual(1, fieldsReturns.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_CHAR, fieldsReturns[0].DataType);
                Assert.AreEqual(TestCharacterFieldId, fieldsReturns[0].FieldId);
                Assert.AreEqual(TestCharacterFieldValueChinese + "Set Something", fieldsReturns[0].Value.ToString());

            }
            catch (Exception ex)
            {
                Assert.AreEqual(null, ex);

            }
            session.LogOut();
        }

        [TestMethod]
        public void CreateEntry_datatype_int()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                //Create
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestIntFieldId, 256, ARDataType.DATA_TYPE_INTEGER));
                string entryid = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //Get for Assert
                List<string> entryIds = new List<string>();
                entryIds.Add(entryid);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestIntFieldId);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_INTEGER, Entry[0].DataType);
                Assert.AreEqual(TestIntFieldId, Entry[0].FieldId);
                Assert.AreEqual(256, Entry[0].Value);

                //Set
                valuelist.Clear();
                valuelist.Add(new ARFieldValue(TestIntFieldId, 512, ARDataType.DATA_TYPE_INTEGER));
                session.SetEntry(TestRegularFormName, entryIds.ToArray(), valuelist.ToArray());

                //Get for Assert
                Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_INTEGER, Entry[0].DataType);
                Assert.AreEqual(TestIntFieldId, Entry[0].FieldId);
                Assert.AreEqual(512, Entry[0].Value);


            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void CreateEntry_datatype_DateTime()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                //Create
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                //convert datetime to second from 1970-1-1
                TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
                int secondtoNow = (int)ts.TotalSeconds;
                valuelist.Add(new ARFieldValue(TestDateTimeFieldId, secondtoNow, ARDataType.DATA_TYPE_TIME));
                string entryid = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //Get for Assert
                List<string> entryIds = new List<string>();
                entryIds.Add(entryid);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestDateTimeFieldId);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_TIME, Entry[0].DataType);
                Assert.AreEqual(TestDateTimeFieldId, Entry[0].FieldId);
                Assert.AreEqual(secondtoNow, Entry[0].Value);

                //Set
                valuelist.Clear();
                valuelist.Add(new ARFieldValue(TestDateTimeFieldId, secondtoNow+100, ARDataType.DATA_TYPE_TIME));
                session.SetEntry(TestRegularFormName, entryIds.ToArray(), valuelist.ToArray());

                //Get for Assert
                Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_TIME, Entry[0].DataType);
                Assert.AreEqual(TestDateTimeFieldId, Entry[0].FieldId);
                Assert.AreEqual(secondtoNow + 100, Entry[0].Value);

            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void CreateEntry_datatype_Date()
        {
            //TODO: after Julian convert impelement
        }


        [TestMethod]
        public void CreateEntry_datatype_Time()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                //Create
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                DateTime dt = DateTime.Now;
                int secondtoNow = dt.Hour*3600 + dt.Minute*60 + dt.Second;
                valuelist.Add(new ARFieldValue(TestTimeFieldId, secondtoNow, ARDataType.DATA_TYPE_TIME_OF_DAY));
                string entryid = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //Get for Assert
                List<string> entryIds = new List<string>();
                entryIds.Add(entryid);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestTimeFieldId);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_TIME_OF_DAY, Entry[0].DataType);
                Assert.AreEqual(TestTimeFieldId, Entry[0].FieldId);
                Assert.AreEqual(secondtoNow, Entry[0].Value);

                //Set
                valuelist.Clear();
                valuelist.Add(new ARFieldValue(TestTimeFieldId, secondtoNow + 100, ARDataType.DATA_TYPE_TIME_OF_DAY));
                session.SetEntry(TestRegularFormName, entryIds.ToArray(), valuelist.ToArray());

                //Get for Assert
                Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_TIME_OF_DAY, Entry[0].DataType);
                Assert.AreEqual(TestTimeFieldId, Entry[0].FieldId);
                Assert.AreEqual(secondtoNow + 100, Entry[0].Value);


            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void CreateEntry_datatype_Real()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                //Create
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestRealFieldId, 3.1415926, ARDataType.DATA_TYPE_REAL));
                string entryid = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //Get for Assert
                List<string> entryIds = new List<string>();
                entryIds.Add(entryid);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestRealFieldId);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_REAL, Entry[0].DataType);
                Assert.AreEqual(TestRealFieldId, Entry[0].FieldId);
                Assert.AreEqual(3.1415926, Entry[0].Value);

                //Set
                valuelist.Clear();
                valuelist.Add(new ARFieldValue(TestRealFieldId, 0.618, ARDataType.DATA_TYPE_REAL));
                session.SetEntry(TestRegularFormName, entryIds.ToArray(), valuelist.ToArray());
                //Get for Assert
                Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_REAL, Entry[0].DataType);
                Assert.AreEqual(TestRealFieldId, Entry[0].FieldId);
                Assert.AreEqual(0.618, Entry[0].Value);

            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void CreateEntry_datatype_Decimal()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                //Create
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestDecimalFieldId, 3.14, ARDataType.DATA_TYPE_DECIMAL));
                string entryid = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //Get for Assert
                List<string> entryIds = new List<string>();
                entryIds.Add(entryid);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestDecimalFieldId);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_DECIMAL, Entry[0].DataType);
                Assert.AreEqual(TestDecimalFieldId, Entry[0].FieldId);
                Assert.AreEqual(3.14, Convert.ToDouble(Entry[0].Value));

                //Set
                valuelist.Clear();
                valuelist.Add(new ARFieldValue(TestDecimalFieldId, 0.61, ARDataType.DATA_TYPE_DECIMAL));
                session.SetEntry(TestRegularFormName, entryIds.ToArray(), valuelist.ToArray());

                //Get for Assert
                Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_DECIMAL, Entry[0].DataType);
                Assert.AreEqual(TestDecimalFieldId, Entry[0].FieldId);
                Assert.AreEqual(0.61, Convert.ToDouble(Entry[0].Value));
            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }

        [TestMethod]
        public void CreateEntry_datatype_Enum_Status()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                //Create
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(7, 1, ARDataType.DATA_TYPE_ENUM));
                string entryid = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                //Get for Assert
                List<string> entryIds = new List<string>();
                entryIds.Add(entryid);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add(7);
                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_ENUM, Entry[0].DataType);
                Assert.AreEqual(7, (int)Entry[0].FieldId);
                Assert.AreEqual((UInt32)1, Entry[0].Value);

                //Set
                valuelist.Clear();
                valuelist.Add(new ARFieldValue(7, 2, ARDataType.DATA_TYPE_ENUM));
                session.SetEntry(TestRegularFormName, entryIds.ToArray(), valuelist.ToArray());

                //Get for Assert
                Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(1, Entry.Count);
                Assert.AreEqual(ARDataType.DATA_TYPE_ENUM, Entry[0].DataType);
                Assert.AreEqual(7, (int)Entry[0].FieldId);
                Assert.AreEqual((UInt32)2, Entry[0].Value);
            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();
        }
    }
}
