using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARNative.Test
{
    [TestClass]
    public class ARSession_ImpersonatUser_Test : ARRegularConfig
    {

        private static string ImpersonatedChineseUser;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {

            //create a user which has login name with unicode character
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(101u, "周平", ARDataType.DATA_TYPE_CHAR));
                valuelist.Add(new ARFieldValue(8u, "周平", ARDataType.DATA_TYPE_CHAR));
                valuelist.Add(new ARFieldValue(102u, "123", ARDataType.DATA_TYPE_CHAR));
                valuelist.Add(new ARFieldValue(109u, 1, ARDataType.DATA_TYPE_ENUM));
                session.CreateEntry("User", valuelist.ToArray());

                ImpersonatedChineseUser = "周平";
            }
            finally
            {
                session.LogOut();
            }

        }

        [ClassCleanup]
        public static void Cleanup()
        {
            //cleanup the user which has been created in Initialize
            ARSession session = new ARSession();
            try
            {
                //clean user
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add(1u);
                int total = -1;
                var entry = session.GetEntryList("User", "'101'=\"周平\"", fieldIds.ToArray(), null, null, ref total, null);
                if (entry != null || entry.Count != 0)
                {
                    foreach(var e in entry)
                        session.DeleteEntry("User", e.EntryIds.ToArray());
                }

                //clean data in TestRegularFormName
                int totalCount = -1;
                List<AREntry> allentries = session.GetEntryList(
                    TestRegularFormName,
                    null,
                    null,
                    null,
                    null,
                    ref totalCount,
                    null);

                foreach (AREntry e in allentries)
                {
                    session.DeleteEntry(TestRegularFormName, e.EntryIds.ToArray());
                }

            }
            finally
            {
                session.LogOut();
            }
        }

        [TestMethod]
        public void Impersonat_Create_Test()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValue, ARDataType.DATA_TYPE_CHAR));

                session.SetImpersonatedUser(ImpersonatedChineseUser);

                string entryid = session.CreateEntry(TestRegularFormName, valuelist.ToArray());

                List<string> entryIds = new List<string>();
                entryIds.Add(entryid);
                List<uint> fieldIds = new List<uint>();
                fieldIds.Add((uint)TestCharacterFieldId);
                fieldIds.Add(2);
                fieldIds.Add(5);

                session.SetImpersonatedUser(null);

                List<ARFieldValue> Entry = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());
                Assert.AreEqual(TestCharacterFieldValue,
                    Entry.First(f => f.FieldId == TestCharacterFieldId).Value.ToString());
                Assert.AreEqual(ImpersonatedChineseUser,
                    Entry.First(f => f.FieldId == 2).Value.ToString());
                Assert.AreEqual(ImpersonatedChineseUser,
                    Entry.First(f => f.FieldId == 5).Value.ToString());

            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            finally
            {
                session.LogOut();
            }

        }

        [TestMethod]
        public void Impersonat_Create_Test_but_User_Not_Exist()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                List<ARFieldValue> valuelist = new List<ARFieldValue>();
                valuelist.Add(new ARFieldValue(TestCharacterFieldId, (object)TestCharacterFieldValue, ARDataType.DATA_TYPE_CHAR));

                session.SetImpersonatedUser("周平123");

                string entryid = session.CreateEntry(TestRegularFormName, valuelist.ToArray());


            }
            catch (ARException ex)
            {
                Assert.AreEqual(623, ex.MessageNumber); //authentication failed
            }
            finally
            {
                session.LogOut();
            }
        }
    }
}
