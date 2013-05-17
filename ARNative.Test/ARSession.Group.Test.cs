using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ARNative;

namespace ARNative.Test
{
    [TestClass]
    public class ARSession_Group_Test : ARSessionConfig
    {
        private static string group1 = null;
        private static string group2 = null; 
        private static string group3 = null;

        private static string user1 = null;
        private static string user2 = null;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                Cleanup();
                //create some groups should have different group category and type
                List<ARFieldValue> group_flvs1 = new List<ARFieldValue>();
                group_flvs1.Add(new ARFieldValue(107, 0, ARDataType.DATA_TYPE_ENUM));//group type
                group_flvs1.Add(new ARFieldValue(106, 88888880, ARDataType.DATA_TYPE_INTEGER));//group id
                group_flvs1.Add(new ARFieldValue(105, "Group_None_Regular", ARDataType.DATA_TYPE_CHAR));//group name
                group_flvs1.Add(new ARFieldValue(8, "Group_None_Regular", ARDataType.DATA_TYPE_CHAR));//group long name
                group_flvs1.Add(new ARFieldValue(120, 0, ARDataType.DATA_TYPE_ENUM));//group category
                group1 = session.CreateEntry("Group", group_flvs1.ToArray());

                List<ARFieldValue> group_flvs2 = new List<ARFieldValue>();
                group_flvs2.Add(new ARFieldValue(107, 1, ARDataType.DATA_TYPE_ENUM));//group type
                group_flvs2.Add(new ARFieldValue(106, 88888881, ARDataType.DATA_TYPE_INTEGER));//group id
                group_flvs2.Add(new ARFieldValue(105, "Group_View_Regular", ARDataType.DATA_TYPE_CHAR));//group name
                group_flvs2.Add(new ARFieldValue(8, "Group_View_Regular", ARDataType.DATA_TYPE_CHAR));//group long name
                group_flvs2.Add(new ARFieldValue(120, 0, ARDataType.DATA_TYPE_ENUM));//group category
                group2 = session.CreateEntry("Group", group_flvs2.ToArray());

                List<ARFieldValue> group_flvs3 = new List<ARFieldValue>();
                group_flvs3.Add(new ARFieldValue(107, 2, ARDataType.DATA_TYPE_ENUM));//group type
                group_flvs3.Add(new ARFieldValue(106, 88888882, ARDataType.DATA_TYPE_INTEGER));//group id
                group_flvs3.Add(new ARFieldValue(105, "Group_Change_Regular", ARDataType.DATA_TYPE_CHAR));//group name
                group_flvs3.Add(new ARFieldValue(8, "Group_Change_Regular", ARDataType.DATA_TYPE_CHAR));//group long name
                group_flvs3.Add(new ARFieldValue(120, 0, ARDataType.DATA_TYPE_ENUM));//group category
                group3 = session.CreateEntry("Group", group_flvs3.ToArray());


                //create a test user with groups and a user without groups
                List<ARFieldValue> flvs1 = new List<ARFieldValue>();
                flvs1.Add(new ARFieldValue(101, "group_test_user1", ARDataType.DATA_TYPE_CHAR));//login name
                flvs1.Add(new ARFieldValue(109, 1, ARDataType.DATA_TYPE_ENUM));//administrator must have fixed
                flvs1.Add(new ARFieldValue(110, 0, ARDataType.DATA_TYPE_ENUM));
                flvs1.Add(new ARFieldValue(8, "group_test_user1", ARDataType.DATA_TYPE_CHAR));//long name
                flvs1.Add(new ARFieldValue(102, "123", ARDataType.DATA_TYPE_CHAR));//pwd
                flvs1.Add(new ARFieldValue(7, 0, ARDataType.DATA_TYPE_ENUM));//status
                flvs1.Add(new ARFieldValue(104, ";88888880;88888881;88888882;", ARDataType.DATA_TYPE_CHAR));//group list
                user1 = session.CreateEntry("User", flvs1.ToArray());

                List<ARFieldValue> flvs2 = new List<ARFieldValue>();
                flvs2.Add(new ARFieldValue(101, "no_group_test_user2", ARDataType.DATA_TYPE_CHAR));//login name
                flvs2.Add(new ARFieldValue(109, 0, ARDataType.DATA_TYPE_ENUM));
                flvs2.Add(new ARFieldValue(110, 0, ARDataType.DATA_TYPE_ENUM));
                flvs2.Add(new ARFieldValue(8, "no_group_test_user2", ARDataType.DATA_TYPE_CHAR));//long name
                flvs2.Add(new ARFieldValue(102, "123", ARDataType.DATA_TYPE_CHAR));//pwd
                flvs2.Add(new ARFieldValue(7, 0, ARDataType.DATA_TYPE_ENUM));//status
                user2 = session.CreateEntry("User", flvs2.ToArray());


            }
            catch (ARException ex)
            {
                throw ex;
            }
            catch (Exception ex2)
            {
                throw ex2;
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
                int t = -1;
                //delete the test user and test groups
                session.Login(TestServer, TestAdmin, TestAdminPwd);

                var entry = session.GetEntryList("User", "'101'=\"group_test_user1\"", null, null, null,ref t, null);
                if(entry.Count > 0)
                    session.DeleteEntry("User", entry[0].EntryIds.ToArray());
                entry = session.GetEntryList("User", "'101'=\"no_group_test_user2\"", null, null, null, ref t, null);
                if (entry.Count > 0)
                    session.DeleteEntry("User", entry[0].EntryIds.ToArray());


                entry = session.GetEntryList("Group", "'106'=88888880", null, null, null, ref t, null);
                if (entry.Count > 0)
                    session.DeleteEntry("Group", entry[0].EntryIds.ToArray());
                entry = session.GetEntryList("Group", "'106'=88888881", null, null, null, ref t, null);
                if (entry.Count > 0)
                    session.DeleteEntry("Group", entry[0].EntryIds.ToArray());
                entry = session.GetEntryList("Group", "'106'=88888882", null, null, null, ref t, null);
                if (entry.Count > 0)
                    session.DeleteEntry("Group", entry[0].EntryIds.ToArray());

            }
            finally
            {
                session.LogOut();
            }
        }

        [TestMethod]
        public void ARSession_GetListGroup_Test()
        {
            ARSession session = new ARSession();
            try
            {
                session.Login(TestServer, TestAdmin, TestAdminPwd);
                var list = session.GetUserGroupList("group_test_user1");
                Assert.AreEqual(3, list.Count);
                var group1 = list.Find(new Predicate<ARGroupInfo>(delegate(ARGroupInfo gi){
                    return gi.GroupId == 88888880;
                }));

                Assert.AreEqual("Group_None_Regular", group1.GroupName);
                Assert.AreEqual(GroupCategory.REGULAR, group1.GroupCategory);
                Assert.AreEqual(GroupType.NONE, group1.GroupType);

                var group2 = list.Find(new Predicate<ARGroupInfo>(delegate(ARGroupInfo gi)
                {
                    return gi.GroupId == 88888881;
                }));

                Assert.AreEqual("Group_View_Regular", group2.GroupName);
                Assert.AreEqual(GroupCategory.REGULAR, group2.GroupCategory);
                Assert.AreEqual(GroupType.VIEW, group2.GroupType);


                var group3 = list.Find(new Predicate<ARGroupInfo>(delegate(ARGroupInfo gi)
                {
                    return gi.GroupId == 88888882;
                }));

                Assert.AreEqual("Group_Change_Regular", group3.GroupName);
                Assert.AreEqual(GroupCategory.REGULAR, group3.GroupCategory);
                Assert.AreEqual(GroupType.CHANGE, group3.GroupType);

                var list2 = session.GetUserGroupList("no_group_test_user2");
                Assert.AreEqual(0, list2.Count);
            }
            catch (Exception ex)
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
