﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources.TournamentRegistration {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.TournamentRegistration.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Balance.
        /// </summary>
        public static string Balance {
            get {
                return ResourceManager.GetString("Balance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Show and edit my personal data.
        /// </summary>
        public static string EditUserDataLinkCaption {
            get {
                return ResourceManager.GetString("EditUserDataLinkCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of adjudicators is lower than the adjudicator policy specifies..
        /// </summary>
        public static string ErrorAdjWantedBelowPolicy {
            get {
                return ResourceManager.GetString("ErrorAdjWantedBelowPolicy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of adjucators is higher than the slot capacity. Please enter a smaller number of adjucators..
        /// </summary>
        public static string ErrorAdjWantedExceedAdjCap {
            get {
                return ResourceManager.GetString("ErrorAdjWantedExceedAdjCap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This organisation cannot be billed..
        /// </summary>
        public static string ErrorOrganizationCannotBeBilled {
            get {
                return ResourceManager.GetString("ErrorOrganizationCannotBeBilled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of teams is higher than the team capacity. Please enter a smaller number of teams..
        /// </summary>
        public static string ErrorTeamsWantedExceedTeamCap {
            get {
                return ResourceManager.GetString("ErrorTeamsWantedExceedTeamCap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to We are missing important information from you..
        /// </summary>
        public static string MissingPersonalData {
            get {
                return ResourceManager.GetString("MissingPersonalData", resourceCulture);
            }
        }
    }
}
