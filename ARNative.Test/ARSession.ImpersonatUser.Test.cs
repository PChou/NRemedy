using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARNative.Test
{
    [TestClass]
    public class ARSession_ImpersonatUser_Test : ARSession_CreateEntry_Test
    {
        //#region Test Package
        //protected static string TestRegularFormName = "NRemedy_Test_Regular_Form";
        //protected static uint TestCharacterFieldId = 20000001;
        //protected static uint TestIntFieldId = 20000002;
        //protected static uint TestCharacterFieldIdNotExist = 745678897;

        //protected static string TestCharacterFieldValue = "Hello Remedy";
        //protected static string TestCharacterFieldValueChinese = "你好 Remedy";


        //#endregion

        private static string ImpersonatedChineseUser = "周平";

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
    }
}
