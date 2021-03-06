﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources.DebRegCommunication.SendMail {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.DebRegCommunication.SendMail.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///we received a request to change your e-mail address in DebReg to {1}. If this is not correct, please ignore this message.
        ///
        ///To confirm the new e-mail address, click the link below. Please note, that you might have to login with your old e-mail address one last time.
        ///
        ///{2}.
        /// </summary>
        public static string ConfirmEMailBody {
            get {
                return ResourceManager.GetString("ConfirmEMailBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///Dear {0},
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///we received a request to change your e-mail address in DebReg to &lt;strong&gt;{1}&lt;/strong&gt;. If this is not correct, please ignore this message.
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///&lt;a href=&quot;{2}&quot;&gt;Cick here to confirm your new e-mail address.&lt;/a&gt;. Please note, that you might have to login with your old e-mail address one last time.
        ///&lt;/p&gt;.
        /// </summary>
        public static string ConfirmEMailBodyHTML {
            get {
                return ResourceManager.GetString("ConfirmEMailBodyHTML", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Confirm your new e-mail address in DebReg.
        /// </summary>
        public static string ConfirmEMailSubject {
            get {
                return ResourceManager.GetString("ConfirmEMailSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///we received a request to reset your DebReg password. If the password reset was requested by yourself, please click the link below. Otherwise ignore this message.
        ///
        ///To reset your password click this link:
        ///{1}.
        /// </summary>
        public static string RequestPasswordResetBody {
            get {
                return ResourceManager.GetString("RequestPasswordResetBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///Dear {0},
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///we received a request to reset your &lt;strong&gt;DebReg&lt;/strong&gt; password. If the password reset was requested by yourself, please click the link below. Otherwise ignore this message.
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///&lt;a href=&quot;{1}&quot;&gt;Reset your password&lt;/a&gt;
        ///&lt;/p&gt;.
        /// </summary>
        public static string RequestPasswordResetBodyHTML {
            get {
                return ResourceManager.GetString("RequestPasswordResetBodyHTML", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DebReg Password Reset Request.
        /// </summary>
        public static string RequestPasswordResetSubject {
            get {
                return ResourceManager.GetString("RequestPasswordResetSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///welcome to DebReg, the exclusive service for debating tournament management.
        ///
        ///A new user account was created for you. Please click the link below to set your password and login.
        ///
        ///{1}.
        /// </summary>
        public static string UserRegisteredBody {
            get {
                return ResourceManager.GetString("UserRegisteredBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///&lt;p&gt;
        ///Dear {0},
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///welcome to &lt;strong&gt;DebReg&lt;/strong&gt;, the exclusive service for debating tournament management. A new user account was created for you.&lt;/p&gt;
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///&lt;a href=&quot;{1}&quot;&gt;Set password and log in.&lt;/a&gt;
        ///&lt;/p&gt;.
        /// </summary>
        public static string UserRegisteredBodyHTML {
            get {
                return ResourceManager.GetString("UserRegisteredBodyHTML", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///{1} has registered you as {2} for {3}. Please help us providing you an excellent service by checking and completing your personal data for the tournament.
        ///
        ///To do so, click the link below.
        ///{4}.
        /// </summary>
        public static string UserRegisteredForTournamentBody {
            get {
                return ResourceManager.GetString("UserRegisteredForTournamentBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///Dear {0},
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///{1} has registered you as {2} for {3}. Please help us providing you an excellent service by &lt;a href=&quot;{4}&quot;&gt;checking and completing your personal data for the tournament.&lt;/a&gt;
        ///&lt;/p&gt;.
        /// </summary>
        public static string UserRegisteredForTournamentBodyHTML {
            get {
                return ResourceManager.GetString("UserRegisteredForTournamentBodyHTML", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Registration for {1}.
        /// </summary>
        public static string UserRegisteredForTournamentSubject {
            get {
                return ResourceManager.GetString("UserRegisteredForTournamentSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Welcome to DebReg!.
        /// </summary>
        public static string UserRegisteredSubject {
            get {
                return ResourceManager.GetString("UserRegisteredSubject", resourceCulture);
            }
        }
    }
}
