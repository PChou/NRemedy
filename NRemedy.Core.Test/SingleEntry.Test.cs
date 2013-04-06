using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRemedy;
using ARNative;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class SingleEntry_Test : Login_Test
    {
        #region Test Package
        protected static string TestRegularFormName = "NRemedy_Test_Regular_Form";
        protected static uint TestCharacterFieldId = 20000001;
        protected static uint TestIntFieldId = 20000002;
        protected static uint TestDateTimeFieldId = 20000003;
        protected static uint TestDateFieldId = 20000004;
        protected static uint TestTimeFieldId = 2000005;
        protected static uint TestRealFieldId = 20000006;
        protected static uint TestDecimalFieldId = 20000007;

        protected static uint TestCharacterFieldIdNotExist = 745678897;

        protected static string TestCharacterFieldValue = "Hello Remedy";
        protected static string TestCharacterFieldValueChinese = "你好 Remedy";


        #endregion

        /// <summary>
        /// perform env ready
        /// include clear all data in TestRegularFormName
        /// </summary>
        public SingleEntry_Test()
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
        public void ARProxy_CreateEntry_simple_success()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form {
                    CharacterField = TestCharacterFieldValue
                });
                Assert.AreNotEqual(null, entryId);
                NRemedy_Test_Regular_Form model = proxy.GetEntry(entryId);
                Assert.AreEqual(TestCharacterFieldValue, model.CharacterField);
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
        public void ARProxy_CreateEntry_datatype_character_chinese_with_delete()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form
                {
                    CharacterField = TestCharacterFieldValueChinese
                });
                Assert.AreNotEqual(null, entryId);

                //GetEntry
                NRemedy_Test_Regular_Form model = proxy.GetEntry(entryId);
                Assert.AreEqual(TestCharacterFieldValueChinese,  model.CharacterField);

                //DeleteEntry
                proxy.DeleteEntry(model);

                //GetAgain
                model = proxy.GetEntry(entryId);
                Assert.IsNull(model);

                //DeleteAgain
                proxy.DeleteEntry(TestRegularFormName, entryId);
            }
            catch (ARException ex)
            {
                Assert.AreEqual(null, ex);

            }
            finally
            {
                context.Dispose();
            }
        }

        [TestMethod]
        public void ARProxy_CreateEntry_datatype_character_chinese_with_set()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {

                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form
                {
                    CharacterField = TestCharacterFieldValueChinese,
                    IntegerField = 3
                });
                Assert.AreNotEqual(null, entryId);

                //GetEntry
                NRemedy_Test_Regular_Form model = proxy.GetEntry(entryId);
                Assert.AreEqual(TestCharacterFieldValueChinese, model.CharacterField);
                model.CharacterField = TestCharacterFieldValueChinese + "Set Something";

                //SetEntry
                proxy.SetEntry(model);
                model = null;

                //GetAgain
                model = proxy.GetEntry(entryId);
                Assert.IsNotNull(model);
                Assert.AreEqual(TestCharacterFieldValueChinese + "Set Something", model.CharacterField);
                Assert.AreEqual(3, model.IntegerField);

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
        public void ARProxy_CreateEntry_datatype_int()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form
                {
                    CharacterField = "should not changed",
                    IntegerField = 256
                });
                Assert.AreNotEqual(null, entryId);
                NRemedy_Test_Regular_Form model = proxy.GetEntry(entryId);
                Assert.AreEqual(256, model.IntegerField);

                model.IntegerField = 512;
                proxy.SetEntry(model);

                Assert.AreEqual(512, model.IntegerField);
                Assert.AreEqual("should not changed", model.CharacterField);


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
        public void ARProxy_CreateEntry_datatype_datetime()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                DateTime dt = DateTime.Now;
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form
                {
                    CharacterField = "should not changed",
                    DateTimeField = dt
                });
                Assert.AreNotEqual(null, entryId);
                NRemedy_Test_Regular_Form model = proxy.GetEntry(entryId);

                Assert.AreEqual(dt.Year, ((DateTime)model.DateTimeField).Year);
                Assert.AreEqual(dt.Month, ((DateTime)model.DateTimeField).Month);
                Assert.AreEqual(dt.Day, ((DateTime)model.DateTimeField).Day);
                Assert.AreEqual(dt.Hour, ((DateTime)model.DateTimeField).Hour);
                Assert.AreEqual(dt.Minute, ((DateTime)model.DateTimeField).Minute);
                Assert.AreEqual(dt.Second, ((DateTime)model.DateTimeField).Second);

                model.DateTimeField = dt + new TimeSpan(1, 1, 1);
                proxy.SetEntry(model);

                Assert.AreEqual(dt.Year, ((DateTime)model.DateTimeField).Year);
                Assert.AreEqual(dt.Month, ((DateTime)model.DateTimeField).Month);
                Assert.AreEqual(dt.Day, ((DateTime)model.DateTimeField).Day);
                Assert.AreEqual(dt.Hour + 1, ((DateTime)model.DateTimeField).Hour);
                Assert.AreEqual(dt.Minute + 1, ((DateTime)model.DateTimeField).Minute);
                Assert.AreEqual(dt.Second + 1, ((DateTime)model.DateTimeField).Second);

                //Assert.AreEqual(dt + new TimeSpan(1, 1, 1), model.DateTimeField);
                Assert.AreEqual("should not changed", model.CharacterField);


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
        public void ARProxy_CreateEntry_datatype_Real()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                DateTime dt = DateTime.Now;
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form
                {
                    CharacterField = "should not changed",
                    RealNumberField = 3.14
                });
                Assert.AreNotEqual(null, entryId);
                NRemedy_Test_Regular_Form model = proxy.GetEntry(entryId);
                Assert.AreEqual(3.14, model.RealNumberField);

                model.RealNumberField = 0.62;
                proxy.SetEntry(model);

                Assert.AreEqual(0.62, model.RealNumberField);

                //Assert.AreEqual(dt + new TimeSpan(1, 1, 1), model.DateTimeField);
                Assert.AreEqual("should not changed", model.CharacterField);


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
        public void ARProxy_CreateEntry_datatype_Decimal()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                DateTime dt = DateTime.Now;
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form
                {
                    CharacterField = "should not changed",
                    DecimalNumberField = 3.14m
                });
                Assert.AreNotEqual(null, entryId);
                NRemedy_Test_Regular_Form model = proxy.GetEntry(entryId);
                Assert.AreEqual(3.14m, model.DecimalNumberField);

                model.DecimalNumberField = 0.62m;
                proxy.SetEntry(model);

                Assert.AreEqual(0.62m, model.DecimalNumberField);

                //Assert.AreEqual(dt + new TimeSpan(1, 1, 1), model.DateTimeField);
                Assert.AreEqual("should not changed", model.CharacterField);


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
        public void ARProxy_CreateEntry_datatype_Enum()
        {
            ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                ARProxy<NRemedy_Test_Regular_Form> proxy = new ARProxy<NRemedy_Test_Regular_Form>(context);
                DateTime dt = DateTime.Now;
                string entryId = proxy.CreateEntry(new NRemedy_Test_Regular_Form
                {
                    CharacterField = "should not changed",
                    Status = NRemedy_Test_Regular_Form.Status_Enum.Fixed
                });
                Assert.AreNotEqual(null, entryId);
                NRemedy_Test_Regular_Form model = proxy.GetEntry(entryId);
                Assert.AreEqual(NRemedy_Test_Regular_Form.Status_Enum.Fixed, model.Status);

                model.Status = NRemedy_Test_Regular_Form.Status_Enum.Rejected;
                proxy.SetEntry(model);

                Assert.AreEqual(NRemedy_Test_Regular_Form.Status_Enum.Rejected, model.Status);

                //Assert.AreEqual(dt + new TimeSpan(1, 1, 1), model.DateTimeField);
                Assert.AreEqual("should not changed", model.CharacterField);


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
