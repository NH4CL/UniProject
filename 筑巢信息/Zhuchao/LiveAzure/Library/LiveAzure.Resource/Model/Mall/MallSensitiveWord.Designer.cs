﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.237
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace LiveAzure.Resource.Model.Mall {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class MallSensitiveWord {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MallSensitiveWord() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LiveAzure.Resource.Model.Mall.MallSensitiveWord", typeof(MallSensitiveWord).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 Channel 的本地化字符串。
        /// </summary>
        public static string Channel {
            get {
                return ResourceManager.GetString("Channel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Channel ID  的本地化字符串。
        /// </summary>
        public static string ChlID {
            get {
                return ResourceManager.GetString("ChlID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Keyword 的本地化字符串。
        /// </summary>
        public static string Keyword {
            get {
                return ResourceManager.GetString("Keyword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Organization 的本地化字符串。
        /// </summary>
        public static string Organization {
            get {
                return ResourceManager.GetString("Organization", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Organization ID  的本地化字符串。
        /// </summary>
        public static string OrgID {
            get {
                return ResourceManager.GetString("OrgID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Remark 的本地化字符串。
        /// </summary>
        public static string Remark {
            get {
                return ResourceManager.GetString("Remark", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Word Status 的本地化字符串。
        /// </summary>
        public static string Wstatus {
            get {
                return ResourceManager.GetString("Wstatus", resourceCulture);
            }
        }
    }
}
