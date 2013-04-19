using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NRemedy.Linq;
using System.Linq;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class JoinForm_Test : RegularConfig
    {

        [TestMethod]
        public void ARProxy_Create_Entry_JoinForm_No_Assert()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARProxy<NRemedy_Test_Join_Form> proxy = new ARProxy<NRemedy_Test_Join_Form>(context);
                string entryid = proxy.CreateEntry(new NRemedy_Test_Join_Form { 
                    GUID = Guid.NewGuid().ToString(),
                    ShortDescription = "shortdescription1",
                    ShortDescription2 = "shortdescription2",
                    CharacterField = "CharacterField",
                    CharacterField2 = "CharacterField2"
                });
            }

        }

        [TestMethod]
        public void ARProxy_GetEntryStatictisc_JoinForm()
        {
            List<Guid> guid = new List<Guid>();
            for (int i = 0; i < 10; i++)
            {
                guid.Add(Guid.NewGuid());
 
            }

            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARProxy<NRemedy_Test_Join_Form> proxy = new ARProxy<NRemedy_Test_Join_Form>(context);
                foreach (var g in guid)
                {
                    proxy.CreateEntry(new NRemedy_Test_Join_Form
                    {
                        GUID = g.ToString(),
                        ShortDescription = "shortdescription1",
                        ShortDescription2 = "shortdescription2",
                        CharacterField = "CharacterField",
                        CharacterField2 = "CharacterField2"
                    });
                }            

                ARSet<NRemedy_Test_Join_Form> set = new ARSet<NRemedy_Test_Join_Form>(context);

                foreach (var g in guid)
                {
                    var q = from s in set
                            where s.GUID == g.ToString()
                            select s;

                    Assert.AreEqual(1, q.Count());
                }

            }

        }

        [TestMethod]
        public void ARProxy_GetEntry_JoinForm()
        {
            List<Guid> guid = new List<Guid>();
            for (int i = 0; i < 10; i++)
            {
                guid.Add(Guid.NewGuid());

            }

            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARProxy<NRemedy_Test_Join_Form> proxy = new ARProxy<NRemedy_Test_Join_Form>(context);
                foreach (var g in guid)
                {
                    proxy.CreateEntry(new NRemedy_Test_Join_Form
                    {
                        GUID = g.ToString(),
                        ShortDescription = "shortdescription1",
                        ShortDescription2 = "shortdescription2",
                        CharacterField = "CharacterField",
                        CharacterField2 = "CharacterField2"
                    });
                }

                ARSet<NRemedy_Test_Join_Form> set = new ARSet<NRemedy_Test_Join_Form>(context);

                foreach (var g in guid)
                {
                    var q = from s in set
                            where s.GUID == g.ToString()
                            select s;

                    foreach (var r in q)
                    {
                        Assert.AreEqual( g.ToString(),r.GUID);
                    }
                }

            }

        }
    }
}
