using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARNative;
using NRemedy;
using System.Text;
using System.Collections.Generic;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class BLOB_Test : Login_Test
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

        protected static string TestRegularFormName = "NRemedy_Test_Regular_Form";
        protected static UInt32 attchFId = 20000011;
        protected static uint TestCharacterFieldId = 20000001;

        [TestMethod]
        public void ARProxy_Create_Entry_with_Att_01()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {

                string testStr = "This is test string , which will be save as file and create entry into AR. The Test method is ARProxy_Create_Entry_with_Att_01";

                byte[] buff = Encoding.UTF8.GetBytes(testStr);

                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form
                {
                    Attachment = new ARAttachment {
                        AttchmentName = "ARProxy_Create_Entry_with_Att_01.txt",
                        Buff = buff
                    }
                });


                NRemedy_Test_Regular_Form entry = proxy.GetEntry(entryId);

                entry.Attachment.AttchmentName = "ARProxy_Create_Entry_with_Att_01.txt";
                Assert.AreEqual(testStr, Encoding.UTF8.GetString(entry.Attachment.Buff));



                List<UInt32> attchFIds = new List<uint>();
                attchFIds.Add(attchFId);
                int total = -1;
                IList<NRemedy_Test_Regular_Form> entries = proxy.GetEntryList(null, attchFIds, null, null, ref total, null);
                foreach (var e in entries)
                {
                    if (e.Attachment != null && e.Attachment.AttchmentName == "ARProxy_Create_Entry_with_Att_01.txt")
                    {
                        Assert.AreEqual(testStr, Encoding.UTF8.GetString(e.Attachment.Buff));
                    }
                }

            }

        }
    }
}
