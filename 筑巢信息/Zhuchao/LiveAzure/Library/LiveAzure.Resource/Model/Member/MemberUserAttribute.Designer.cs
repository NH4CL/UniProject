﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.235
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace LiveAzure.Resource.Model.Member {
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
    public class MemberUserAttribute {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MemberUserAttribute() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LiveAzure.Resource.Model.Member.MemberUserAttribute", typeof(MemberUserAttribute).Assembly);
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
        ///   查找类似 Value 的本地化字符串。
        /// </summary>
        public static string Matter {
            get {
                return ResourceManager.GetString("Matter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Must be limited in 256 letters 的本地化字符串。
        /// </summary>
        public static string MatterLong {
            get {
                return ResourceManager.GetString("MatterLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Attributes 的本地化字符串。
        /// </summary>
        public static string OptID {
            get {
                return ResourceManager.GetString("OptID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Optional 的本地化字符串。
        /// </summary>
        public static string Optional {
            get {
                return ResourceManager.GetString("Optional", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Content 的本地化字符串。
        /// </summary>
        public static string OptResult {
            get {
                return ResourceManager.GetString("OptResult", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Result 的本地化字符串。
        /// </summary>
        public static string Result {
            get {
                return ResourceManager.GetString("Result", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 User 的本地化字符串。
        /// </summary>
        public static string User {
            get {
                return ResourceManager.GetString("User", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 User ID 的本地化字符串。
        /// </summary>
        public static string UserID {
            get {
                return ResourceManager.GetString("UserID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 User ID is required 的本地化字符串。
        /// </summary>
        public static string UserIDRequired {
            get {
                return ResourceManager.GetString("UserIDRequired", resourceCulture);
            }
        }
    }
}
