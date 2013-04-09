using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRemedy;
using NRemedy.Linq;

namespace NRemedy.Core.Test
{
    [TestClass]
    public class Linq_Test : RegularConfig
    {
        [ClassInitialize]
        public static void Initialize(TestContext context2)
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

                NRemedy_Test_Regular_Form newentry = new NRemedy_Test_Regular_Form();
                newentry.CharacterField = TestCharacterFieldValue;
                newentry.IntegerField = 1;
                for (int i = 0; i < 7; i++)
                {
                    proxy.CreateEntry(newentry);
                }

                newentry.CharacterField = TestCharacterFieldValueChinese;
                newentry.IntegerField = 2;
                for (int i = 0; i < 9; i++)
                {
                    proxy.CreateEntry(newentry);
                }

                newentry.CharacterField = TestCharacterFieldValueChinese + "Set Something";
                newentry.IntegerField = 3;
                for (int i = 0; i < 2; i++)
                {
                    proxy.CreateEntry(newentry);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("env init in Construct error.", ex);
            }
            finally
            {
                context.Dispose();
            }
        }

        #region ling parse test , can be offline

        [TestMethod]
        public void LinqParse_where_equal_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField == "Hello Remedy"
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasWhere);
                Assert.AreEqual("('Character Field' = \"Hello Remedy\")", tr.Qulification.ToString());

            }

        }

        [TestMethod]
        public void LinqParse_where_gt_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.IntegerField > 1
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasWhere);
                Assert.AreEqual("('Integer Field' > 1)", tr.Qulification.ToString());

            }

        }

        [TestMethod]
        public void LinqParse_where_lt_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.DecimalNumberField < 2m
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasWhere);
                Assert.AreEqual("('Decimal Number Field' < 2)", tr.Qulification.ToString());
            }

        }

        [TestMethod]
        public void LinqParse_where_gteq_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                DateTime dt = DateTime.Now;
                string str = dt.ToString();
                var q = from s in set
                        where s.DateTimeField >= dt
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasWhere);
                Assert.AreEqual(string.Format("('Date/Time Field' >= \"{0}\")", str)
                    , tr.Qulification.ToString());
            }

        }

        [TestMethod]
        public void LinqParse_where_lteq_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                DateTime dt = DateTime.Now;
                string str = dt.ToString();
                var q = from s in set
                        where s.RealNumberField <= 3.14
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasWhere);
                Assert.AreEqual("('Real Number Field' <= 3.14)"
                    , tr.Qulification.ToString());
            }

        }

        [TestMethod]
        public void LinqParse_where_contains_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField.Contains("%Hello world%")
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasWhere);
                Assert.AreEqual("('Character Field' LIKE \"%Hello world%\")"
                    , tr.Qulification.ToString());

            }

        }


        [TestMethod]
        public void LinqParse_where_mixed_clause_01()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField.Contains("%Hello world%") && s.IntegerField == 65535
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasWhere);
                Assert.AreEqual("(('Character Field' LIKE \"%Hello world%\") AND ('Integer Field' = 65535))"
                    , tr.Qulification.ToString());

            }

        }

        [TestMethod]
        public void LinqParse_where_mixed_clause_02()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField.Contains("%Hello world%") && (s.IntegerField == 65535 || s.IntegerField > 65535)
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasWhere);
                Assert.AreEqual("(('Character Field' LIKE \"%Hello world%\") AND (('Integer Field' = 65535) OR ('Integer Field' > 65535)))"
                    , tr.Qulification.ToString());
            }

        }

        [TestMethod]
        public void LinqParse_select_oneproperty_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        select s.CharacterField;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasSelect);

                NRemedy_Test_Regular_Form tmp = new NRemedy_Test_Regular_Form();
                tmp.CharacterField = "tmp";

                var fun = tr.SelectExpression.Compile();
                Assert.AreEqual("tmp", (string)fun.DynamicInvoke(tmp));
                Assert.IsTrue(tr.SelectedProperties.First(m => m.SourceMemberName == "CharacterField") != null);
            }
        }

        [TestMethod]
        public void LinqParse_select_anouymous_object_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        select new
                        {
                            s.RealNumberField,
                            s.DateTimeField
                        };
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasSelect);

                DateTime dt = DateTime.Now;
                NRemedy_Test_Regular_Form tmp = new NRemedy_Test_Regular_Form();
                tmp.RealNumberField = 3.14;
                tmp.DateTimeField = dt;

                var fun = tr.SelectExpression.Compile();
                dynamic dymic = fun.DynamicInvoke(tmp);
                Assert.AreEqual(3.14, dymic.RealNumberField);
                Assert.AreEqual(dt, dymic.DateTimeField);

                Assert.IsTrue(tr.SelectedProperties.First(m => m.SourceMemberName == "RealNumberField") != null);
                Assert.IsTrue(tr.SelectedProperties.First(m => m.SourceMemberName == "DateTimeField") != null);
            }
        }

        [TestMethod]
        public void LinqParse_select_another_object_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        select new Temp
                        {
                            Str = s.CharacterField,
                            dt = (DateTime)s.DateTimeField,
                            de = (Decimal)s.DecimalNumberField
                        };
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                //var tr2 = ((ARQueryProvider<NRemedy_Test_Regular_Form>)q.Provider).ExecuteOnlyTranslate(q.Expression);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasSelect);

                DateTime dt = DateTime.Now;
                NRemedy_Test_Regular_Form tmp = new NRemedy_Test_Regular_Form();
                tmp.CharacterField = "3.14";
                tmp.DateTimeField = dt;
                tmp.DecimalNumberField = 3.14m;

                var fun = tr.SelectExpression.Compile();
                Temp temp = (Temp)fun.DynamicInvoke(tmp);
                Assert.AreEqual("3.14", temp.Str);
                Assert.AreEqual(dt, temp.dt);
                Assert.AreEqual(3.14m, temp.de);

                Assert.IsTrue(tr.SelectedProperties.First(m => m.SourceMemberName == "CharacterField") != null);
                Assert.IsTrue(tr.SelectedProperties.First(m => m.SourceMemberName == "DateTimeField") != null);
                Assert.IsTrue(tr.SelectedProperties.First(m => m.SourceMemberName == "DecimalNumberField") != null);
            }
        }


        [TestMethod]
        public void LinqParse_Skip_const_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set.Skip(10)
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasSkip);
                Assert.IsNotNull(tr.Skip);
                Assert.AreEqual(10, tr.Skip);

            }
        }

        [TestMethod]
        public void LinqParse_Skip_var_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                string i = "10";
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set.Skip(Convert.ToInt32(i))
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasSkip);
                Assert.IsNotNull(tr.Skip);
                Assert.AreEqual(10, tr.Skip);

            }
        }

        [TestMethod]
        public void LinqParse_Skip_muti_exception_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                string i = "10";
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set.Skip(Convert.ToInt32(i)).Skip(1)
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                try
                {
                    TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(typeof(NotSupportedException), ex.GetType());
                }


            }
        }

        [TestMethod]
        public void LinqParse_Take_const_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set.Take(11)
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasTake);
                Assert.IsNotNull(tr.Take);
                Assert.AreEqual(11, tr.Take);

            }
        }


        [TestMethod]
        public void LinqParse_Take_var_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                string i = "10";
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set.Take(Convert.ToInt32(i))
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasTake);
                Assert.IsNotNull(tr.Take);
                Assert.AreEqual(10, tr.Take);

            }
        }

        [TestMethod]
        public void LinqParse_Take_muti_exception_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                string i = "10";
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set.Take(Convert.ToInt32(i)).Take(1)
                        select s;
                Assert.IsTrue(q.Provider is ARQueryProvider<NRemedy_Test_Regular_Form>);
                try
                {
                    TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(typeof(NotSupportedException), ex.GetType());
                }


            }
        }

        [TestMethod]
        public void LinqParse_Take_Skip_Combine_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set.Take(10).Skip(2)
                        select s;
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasTake);
                Assert.IsTrue(tr.HasSkip);
                Assert.AreEqual(10, tr.Take);
                Assert.AreEqual(2, tr.Skip);

            }
        }

        [TestMethod]
        public void LinqParse_OrderBy_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        orderby s.CharacterField
                        select s;
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasOrderBy);
                Assert.AreEqual("CharacterField", tr.OrderByList.First().Property);
                Assert.AreEqual("OrderBy", tr.OrderByList.First().Method);

            }
        }

        [TestMethod]
        public void LinqParse_OrderByDescending_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        orderby s.CharacterField descending
                        select s;
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasOrderBy);
                Assert.AreEqual("CharacterField", tr.OrderByList.First().Property);
                Assert.AreEqual("OrderByDescending", tr.OrderByList.First().Method);

            }
        }

        [TestMethod]
        public void LinqParse_OrderBy_muti_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        orderby s.CharacterField descending
                        orderby s.IntegerField
                        select s;
                TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
                Assert.IsTrue(tr.HasOrderBy);
                Assert.AreEqual("CharacterField", tr.OrderByList.First().Property);
                Assert.AreEqual("OrderByDescending", tr.OrderByList.First().Method);
                Assert.AreEqual("IntegerField", tr.OrderByList[1].Property);
                Assert.AreEqual("OrderBy", tr.OrderByList[1].Method);

            }
        }

        //[TestMethod]
        //public void LinqParse_single_Groupby_clause()
        //{
        //    using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
        //    {
        //        ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
        //        var q = from s in set
        //                group s by s.CharacterField into g
        //                select g;
        //        TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
        //        Assert.AreEqual("CharacterField", tr.GroupedProperties[0].SourceMemberName);
        //        Assert.IsTrue(tr.HasGroupBy);
        //    }
        //}

        //[TestMethod]
        //public void LinqParse_muti_Groupby_clause()
        //{
        //    using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
        //    {
        //        ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
        //        var q = from s in set
        //                group s by new
        //                {
        //                    s.CharacterField,
        //                    s.IntegerField
        //                }
        //                into g
        //                select g;

        //        TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
        //        Assert.IsTrue(tr.HasGroupBy);
        //        Assert.AreEqual(2, tr.GroupedProperties.Count);
        //        Assert.AreEqual("CharacterField", tr.GroupedProperties[0].SourceMemberName);
        //        Assert.AreEqual("IntegerField", tr.GroupedProperties[1].SourceMemberName);

        //    }
        //}

        //[TestMethod]
        //public void LinqParse_muti_Groupby_mix_select_clause()
        //{
        //    using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
        //    {
        //        ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
        //        var q = from s in set
        //                group s by new
        //                {
        //                    s.CharacterField,
        //                    s.IntegerField
        //                }
        //                    into g
        //                    select new { 
        //                        g.Key.CharacterField,
        //                        g.Key.IntegerField,
        //                        sum = g.Sum(r => r.RealNumberField)
        //                    };



        //        TranslateResult tr = q.Translate<NRemedy_Test_Regular_Form>();
        //        Assert.IsTrue(tr.HasGroupBy);
        //        Assert.IsTrue(tr.HasSelect);

        //        Assert.IsTrue(tr.HasStatictisc);
        //        Assert.AreEqual("Sum", tr.StatictiscVerb);

        //        Assert.AreEqual(2, tr.GroupedProperties.Count);
        //        Assert.AreEqual("CharacterField", tr.GroupedProperties[0].SourceMemberName);
        //        Assert.AreEqual("IntegerField", tr.GroupedProperties[1].SourceMemberName);
        //        Assert.AreEqual(2, tr.SelectedProperties.Count);
        //        Assert.AreEqual("CharacterField", tr.SelectedProperties[0].SourceMemberName);
        //        Assert.AreEqual("IntegerField", tr.SelectedProperties[1].SourceMemberName);
        //        Assert.AreEqual("RealNumberField", tr.StatictiscTarget.SourceMemberName);

        //    }
        //}

        #endregion

        #region linq query test , must be online

        [TestMethod]
        public void LinqQuery_where_equal_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField == TestCharacterFieldValue
                        select s;
                int count = 0;
                foreach (var entry in q)
                {
                    Assert.AreEqual(TestCharacterFieldValue, entry.CharacterField);
                    count++;
                }
                Assert.AreEqual(7, count);


            }
        }

        [TestMethod]
        public void LinqQuery_where_contains_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField.Contains("%你好%")
                        select s;
                int count = 0;
                foreach (var entry in q)
                {
                    Assert.IsTrue(entry.CharacterField.Contains("你好"));
                    count++;
                }
                Assert.AreEqual(11, count);
            }
        }

        [TestMethod]
        public void LinqQuery_where_gteq_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.IntegerField >= 2
                        select s;
                int count = 0;
                foreach (var entry in q)
                {
                    Assert.IsTrue(entry.IntegerField >= 2);
                    count++;
                }
                Assert.AreEqual(11, count);
            }
        }

        [TestMethod]
        public void LinqQuery_where_mixed_clause_01()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField.Contains("%Remedy%") && s.IntegerField == 3
                        select s;
                int count = 0;
                foreach (var entry in q)
                {
                    Assert.IsTrue(entry.CharacterField.Contains("Remedy"));
                    Assert.AreEqual(3, entry.IntegerField);
                    count++;
                }
                Assert.AreEqual(2, count);

            }

        }

        [TestMethod]
        public void LinqQuery_where_mixed_count_clause_01()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        where s.CharacterField.Contains("%Remedy%") && s.IntegerField == 3
                        select s;
                int count = q.Count();
                //foreach (var entry in q)
                //{
                //    Assert.IsTrue(entry.CharacterField.Contains("Remedy"));
                //    Assert.AreEqual(3, entry.IntegerField);
                //    count++;
                //}
                Assert.AreEqual(2, count);

            }

        }

        [TestMethod]
        public void LinqQuery_select_oneproperty_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        select s.CharacterField;
                int count = 0;
                foreach (var g in q)
                {
                    Assert.IsTrue(g.Contains("Remedy"));
                    count++;
                }
                Assert.AreEqual(18, count);
            }

        }

        [TestMethod]
        public void LinqQuery_select_anouymous_object_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        select new
                        {
                            s.CharacterField,
                            s.IntegerField
                        };
                int count = 0;
                foreach (var g in q)
                {
                    if (g.IntegerField == 1)
                        Assert.AreEqual(TestCharacterFieldValue, g.CharacterField);
                    if (g.IntegerField == 2)
                        Assert.AreEqual(TestCharacterFieldValueChinese, g.CharacterField);
                    if (g.IntegerField == 3)
                        Assert.AreEqual(TestCharacterFieldValueChinese + "Set Something", g.CharacterField);
                    count++;
                }
                Assert.AreEqual(18, count);
            }
        }

        [TestMethod]
        public void LinqQuery_Skip_const_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set.Skip(7)
                        select s;
                int count = 0;
                foreach (var g in q)
                {
                    Assert.IsTrue(g.CharacterField.Contains("你好"));
                    count++;
                }
                Assert.AreEqual(11, count);

            }
        }

        [TestMethod]
        public void LinqQuery_Take_Skip_Combine_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set.Take(9).Skip(7)
                        select s;
                int count = 0;
                foreach (var g in q)
                {
                    Assert.AreEqual(TestCharacterFieldValueChinese, g.CharacterField);
                    count++;
                }
                Assert.AreEqual(9, count);

            }
        }



        [TestMethod]
        public void LinqQuery_OrderBy_clause()
        {
            using (ARLoginContext context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd))
            {
                ARSet<NRemedy_Test_Regular_Form> set = new ARSet<NRemedy_Test_Regular_Form>(context);
                var q = from s in set
                        orderby s.RequestID
                        select s;
                var q2 = from s in set
                         orderby s.RequestID descending
                         select s;
                var q2list = q2.ToList();
                int count = 17;
                foreach (var g in q)
                {
                    Assert.AreEqual(g.RequestID, q2list[count].RequestID);
                    count--;
                }
                Assert.AreEqual(-1, count);

            }
        }

        #endregion
    }

    internal class Temp
    {
        public string Str { get; set; }

        public DateTime dt { get; set; }

        public decimal de { get; set; }

    }

}
