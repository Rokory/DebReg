﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources.Models.Organization {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.Models.Organization.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Abbreviation.
        /// </summary>
        public static string Abbreviation {
            get {
                return ResourceManager.GetString("Abbreviation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Abbreviation may only contain plain letters, digits, spaces, dashes and underscores and must be between 3 and 10 characters long..
        /// </summary>
        public static string ErrorAbbreviationRegEx {
            get {
                return ResourceManager.GetString("ErrorAbbreviationRegEx", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Abbreviation is required..
        /// </summary>
        public static string ErrorAbbreviationRequired {
            get {
                return ResourceManager.GetString("ErrorAbbreviationRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Is University or Higher Educational Institution.
        /// </summary>
        public static string IsUniversity {
            get {
                return ResourceManager.GetString("IsUniversity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Associated University (University Linked to the Debating Society).
        /// </summary>
        public static string LinkedOrganization {
            get {
                return ResourceManager.GetString("LinkedOrganization", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Associated Universities.
        /// </summary>
        public static string LinkedOrganizations {
            get {
                return ResourceManager.GetString("LinkedOrganizations", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Organization Name.
        /// </summary>
        public static string Name {
            get {
                return ResourceManager.GetString("Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tournaments Registered.
        /// </summary>
        public static string TournamentRegistrations {
            get {
                return ResourceManager.GetString("TournamentRegistrations", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value Added Tax ID (if applicable).
        /// </summary>
        public static string VATId {
            get {
                return ResourceManager.GetString("VATId", resourceCulture);
            }
        }
    }
}