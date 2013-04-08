using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARNative;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ARNative.Test
{
    [TestClass]
    public class ARSession_BLOB_Test : ARRegularConfig
    {

        [TestMethod]
        public void ARSession_Create_Entry_with_Att_01()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);

                /* create entry with att and get it for assert */
                string testStr = "先假设这就是附件内容";

                byte[] buff = Encoding.UTF8.GetBytes(testStr);

                List<ARFieldValue> fvl = new List<ARFieldValue>();
                fvl.Add(new ARFieldValue
                {
                    FieldId = attchFId,
                    DataType = ARDataType.DATA_TYPE_ATTACH,
                    Value = new ARAttachment
                    {
                        AttchmentName = "附件.txt",
                        Buff = buff
                    }
                });

                fvl.Add(new ARFieldValue(TestCharacterFieldId, "Attachment Flag", ARDataType.DATA_TYPE_CHAR));

                string entryId = session.CreateEntry(TestRegularFormName, fvl.ToArray());

                Assert.IsNotNull(entryId);

                List<uint> fieldIds = new List<uint>();
                fieldIds.Add(attchFId);
                List<string> entryIds = new List<string>();
                entryIds.Add(entryId);

                List<ARFieldValue> arfvl = session.GetEntry(TestRegularFormName, entryIds.ToArray(), fieldIds.ToArray());

                Assert.AreEqual(1, arfvl.Count);
                Assert.IsTrue(arfvl[0].Value is ARAttachment);

                ARAttachment att = (ARAttachment)arfvl[0].Value;

                String rStr = Encoding.UTF8.GetString(att.Buff);

                Assert.AreEqual(
                    testStr,
                    rStr);



                byte[] setByte = Encoding.UTF8.GetBytes(testStr + " after set.");
                List<ARFieldValue> set_arfvl = new List<ARFieldValue>();
                set_arfvl.Add(new ARFieldValue{
                    DataType = ARDataType.DATA_TYPE_ATTACH,
                    FieldId = attchFId,
                    Value = new ARAttachment
                    {
                        AttchmentName = "ARSession_Create_Entry_with_Att_01.txt",
                        Buff = setByte
                    }
                });
                session.SetEntry(TestRegularFormName, entryIds.ToArray(), set_arfvl.ToArray());



                /* Get blob from the entry */
                byte[] buffer = session.GetBLOB(TestRegularFormName, entryIds.ToArray(), attchFId);
                String rStr2 = Encoding.UTF8.GetString(buffer);
                Assert.AreEqual(
                    testStr + " after set.",
                    rStr2);

                /* Get entry list */
                int total = -1;

                var listEntry = session.GetEntryList(TestRegularFormName,

                    "'Character Field' = \"Attachment Flag\""
                    , fieldIds.ToArray(), null, null, ref total, null);

                foreach (var l in listEntry)
                {
                    Assert.IsTrue(l.FieldValues[0].Value is ARAttachment);
                    ARAttachment att2 = (ARAttachment)l.FieldValues[0].Value;
                    String rStr3 = Encoding.UTF8.GetString(att2.Buff);
                    Assert.AreEqual(
                        testStr + " after set.",
                        rStr3);
                }

            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);
            }
            session.LogOut();

        }

    }
}
