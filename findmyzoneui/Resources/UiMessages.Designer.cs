﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace findmyzoneui.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class UiMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal UiMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("findmyzoneui.Resources.UiMessages", typeof(UiMessages).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Le fichier de correspondance code Insee et code postal n&apos;a pas été trouvé.
        ///Voulez vous le télécharger automatiquement ?.
        /// </summary>
        internal static string AskDownloadZipInseeCodes {
            get {
                return ResourceManager.GetString("AskDownloadZipInseeCodes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fichier code Insee &lt;&gt; code Postal.
        /// </summary>
        internal static string AskDownloadZipInseeCodesTitle {
            get {
                return ResourceManager.GetString("AskDownloadZipInseeCodesTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Téléchargement du fichier de correspondance code Insee &lt;&gt; code Postal.
        /// </summary>
        internal static string DownloadingZipInseeCodes {
            get {
                return ResourceManager.GetString("DownloadingZipInseeCodes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Erreur.
        /// </summary>
        internal static string Error {
            get {
                return ResourceManager.GetString("Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Le fichier de correspondance code Insee et code postal est nécessaire, l&apos;application ne peut pas fonctionner sans. Vous pouvez le télécharger manuellement : 
        ///- lien dataset {0} 
        ///- lien direct {1}.
        /// </summary>
        internal static string ErrorDownloadingZipInseeCodes {
            get {
                return ResourceManager.GetString("ErrorDownloadingZipInseeCodes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Première utilisation.
        /// </summary>
        internal static string FirstTimeUse {
            get {
                return ResourceManager.GetString("FirstTimeUse", resourceCulture);
            }
        }
    }
}
