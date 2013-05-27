using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NRemedy.CodeGenerator.Test
{
    [TestClass]
    public class UnitTest1
    {
        protected static string TestServer = "172.16.137.13";
        protected static string TestAdmin = "Demo";
        protected static string TestAdminPwd = "123abc";
        [TestMethod]
        public void TestMethod1()
        {
            ARLoginContext _context = new ARLoginContext(TestServer, TestAdmin, TestAdminPwd);
            try
            {
                _context.Login();
                CodeCompileUnit ccu = new CodeCompileUnit();
                CSharpModelGenerator cmgc = new CSharpModelGenerator(_context);
                CSharpModelGeneratorDefaultFactory factory = new CSharpModelGeneratorDefaultFactory();
                ccu = cmgc.GenerateModelCCU("ARNative_Test_Form", factory);
                CSharpCodeProvider cprovider = new CSharpCodeProvider();//提供对 C# 代码生成器和代码编译器的实例的访问
                ICodeGenerator gen = cprovider.CreateGenerator();//C# 代码生成器的实例。
                ICodeCompiler compiler = cprovider.CreateCompiler();//C# 代码编译器的实例。
                //CS代码文件的字符串
                StringBuilder fileContent = new StringBuilder();
                using (StringWriter sw = new StringWriter(fileContent))
                {
                    gen.GenerateCodeFromCompileUnit(ccu, sw, new CodeGeneratorOptions());//主要是想把生成的代码保存为cs文件
                }

                //在编译时候需要的编译参数
                string str = System.Environment.CurrentDirectory;
                string filePath = str + @"\GenerateTest.dll";//生成cs文件的保存位置
                CompilerParameters cp = new CompilerParameters();
                cp.ReferencedAssemblies.Add("System.dll");
                cp.ReferencedAssemblies.Add(@"NRemedy.dll");
                cp.OutputAssembly = filePath;//输出dll的位置
                cp.GenerateInMemory = true; //是否只在内存中生成
                cp.IncludeDebugInformation = true;//包含调试符号  pdb文件
                cp.GenerateExecutable = false;//生成dll,不是exe 
                cp.WarningLevel = 4;//获取或设置使编译器中止编译的警告级别。
                cp.TreatWarningsAsErrors = false;
                File.WriteAllText(filePath, fileContent.ToString());
                File.WriteAllText(str + @"\GenerateTest.txt", fileContent.ToString());
                CompilerResults cr = compiler.CompileAssemblyFromFile(cp, filePath);
                String outputMessage = "";
                foreach (string item in cr.Output)
                {
                    outputMessage += item + Environment.NewLine;//调试的最终输出信息
                }
               
            }
            catch { }
            finally{_context.Dispose();}
        }
    }
}
