using System;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Services.Description;
using System.Xml.Serialization;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace LiveAzure.Utility
{
    /// <summary>
    /// 动态WebService服务类
    /// </summary>
    /// <example>
    ///     SoapHelper oSoap = new SoapHelper("http://www.webservicex.net/globalweather.asmx", "globalweather");
    ///     object oReturn = oSoap.Invoke("GetWeather", new object[] { "Shanghai", "China" });
    /// </example>
    public class SoapHelper
    {

        private string WebServiceUrl { get; set; }
        private string WebServiceNamespace { get; set; }
        private string WebServiceClassname { get; set; }

        private CodeCompileUnit SoapCompileUnit;
        private CodeDomProvider SoapCodeProvider;
        private CompilerResults SoapCompilerResults;

        /// <summary>
        /// 动态 WebService 构造函数
        /// </summary>
        /// <param name="pUrl">WebService 地址</param>
        /// <param name="pClassname">类名，可省略，可空</param>
        public SoapHelper(string pUrl, string pClassname = null)
        {
            if (String.IsNullOrEmpty(pClassname))
                WebServiceClassname = GetWsClassName(pUrl);
            else
                WebServiceClassname = pClassname;
            WebServiceNamespace = "LiveAzure.Utility";

            try
            {
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(pUrl + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                sdi.ProtocolName = "Soap";
                sdi.Style = ServiceDescriptionImportStyle.Client;
                sdi.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateNewAsync;
                CodeNamespace cn = new CodeNamespace(this.WebServiceNamespace);
                SoapCompileUnit = new CodeCompileUnit();
                SoapCompileUnit.Namespaces.Add(cn);
                sdi.Import(cn, SoapCompileUnit);

                CSharpCodeProvider csc = new CSharpCodeProvider();
                SoapCodeProvider = CodeDomProvider.CreateProvider("CSharp");
                CompilerParameters cparam = new CompilerParameters();
                cparam.GenerateExecutable = false;
                cparam.GenerateInMemory = true;
                cparam.ReferencedAssemblies.Add("System.dll");
                cparam.ReferencedAssemblies.Add("System.XML.dll");
                cparam.ReferencedAssemblies.Add("System.Web.Services.dll");
                cparam.ReferencedAssemblies.Add("System.Data.dll");

                SoapCompilerResults = SoapCodeProvider.CompileAssemblyFromDom(cparam, SoapCompileUnit);
                if (SoapCompilerResults.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError ce in SoapCompilerResults.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }

        /// <summary>
        /// 析构函数，ToDo 如何清理
        /// </summary>
        ~SoapHelper()
        {
        }

        /// <summary>
        /// 调用远程方法
        /// </summary>
        /// <param name="pMethodName">方法名</param>
        /// <param name="pArgs">参数集</param>
        /// <example>
        ///     SoapHelper oSoap = new SoapHelper("http://www.webservicex.net/globalweather.asmx", "globalweather");
        ///     object oReturn = oSoap.Invoke("GetWeather", new object[] { "Shanghai", "China" });
        /// </example>
        /// <returns>调用结果</returns>
        public object Invoke(string pMethodName, object[] pArgs)
        {
            Assembly assembly = SoapCompilerResults.CompiledAssembly;
            Type type = assembly.GetType(this.WebServiceNamespace + "." + this.WebServiceClassname, true, true);
            Object instance = Activator.CreateInstance(type);
            MethodInfo mi = type.GetMethod(pMethodName);
            return mi.Invoke(instance, pArgs);
        }

        /// <summary>
        /// 用于测试，根据WebService地址，模似生成一个代理类，生成的代码可以用以下方法保存下来，默认是保存在bin目录下面
        /// </summary>
        public void ExportCShareFile()
        {
            //上面是
            TextWriter writer = File.CreateText("MyWebServiceFile.cs");
            SoapCodeProvider.GenerateCodeFromCompileUnit(SoapCompileUnit, writer, null);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// 如果构造时没有提供类名，则分解URL中的类名
        /// </summary>
        /// <param name="pUrl">WebService 地址</param>
        /// <returns>类名</returns>
        private string GetWsClassName(string pUrl)
        {
            string[] parts = pUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }
    }
}
