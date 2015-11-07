﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources.User {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.User.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to Please note, that you will have to change your password at your first login..
        /// </summary>
        public static string EMailChangePasswordNote {
            get {
                return ResourceManager.GetString("EMailChangePasswordNote", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Click the link below to login and change your password now..
        /// </summary>
        public static string EMailClickLink {
            get {
                return ResourceManager.GetString("EMailClickLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your initial password:.
        /// </summary>
        public static string EMailPassword {
            get {
                return ResourceManager.GetString("EMailPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your E-Mail address is your username:.
        /// </summary>
        public static string EMailUsername {
            get {
                return ResourceManager.GetString("EMailUsername", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A user with this e-mail address already exists..
        /// </summary>
        public static string ErrorDuplicateEMail {
            get {
                return ResourceManager.GetString("ErrorDuplicateEMail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to E-mail or password incorrect..
        /// </summary>
        public static string ErrorEMailPasswordIncorrect {
            get {
                return ResourceManager.GetString("ErrorEMailPasswordIncorrect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Old password incorrect..
        /// </summary>
        public static string ErrorOldPasswordIncorrect {
            get {
                return ResourceManager.GetString("ErrorOldPasswordIncorrect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Organisation is incorrect. Please verify your input..
        /// </summary>
        public static string ErrorOrganizationNotFound {
            get {
                return ResourceManager.GetString("ErrorOrganizationNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Passwords do not match..
        /// </summary>
        public static string ErrorPasswordsDoNotMatch {
            get {
                return ResourceManager.GetString("ErrorPasswordsDoNotMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The new password must not be identitical to the old password..
        /// </summary>
        public static string ErrorPasswordsIdentical {
            get {
                return ResourceManager.GetString("ErrorPasswordsIdentical", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to We could not find a matching user. Please verify your input..
        /// </summary>
        public static string ErrorUserNotFound {
            get {
                return ResourceManager.GetString("ErrorUserNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your password for DebReg was reset based on a user request..
        /// </summary>
        public static string ResetEMailBodyIntro {
            get {
                return ResourceManager.GetString("ResetEMailBodyIntro", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your DebReg password was reset.
        /// </summary>
        public static string ResetEMailSubject {
            get {
                return ResourceManager.GetString("ResetEMailSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Welcome to DebReg, the exclusive service for debating tournament management..
        /// </summary>
        public static string WelcomeEMailBodyIntro {
            get {
                return ResourceManager.GetString("WelcomeEMailBodyIntro", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Welcome to DebReg!.
        /// </summary>
        public static string WelcomeEMailSubject {
            get {
                return ResourceManager.GetString("WelcomeEMailSubject", resourceCulture);
            }
        }
    }
}