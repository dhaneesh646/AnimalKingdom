using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Xml;
using UnityEditor.Build;



namespace JMRSDK.EditorScript
{
    public enum PlatformType
    {
        SM = 0,
        CU
    }

    public static class ManifestConfig
    {
        private static string manifestPath = Path.Combine(Environment.CurrentDirectory + "/Assets/Plugins/Android/AndroidManifest.xml");

        public static PlatformType currentPlatform = PlatformType.SM;

        #region Iteraction Type Editor Prefs
        private const string interactionTypeControllerPrefKey = "InteractionTypeController";
        private static bool interactionTypeController {
            get => EditorPrefs.GetBool(interactionTypeControllerPrefKey);
            set => EditorPrefs.SetBool(interactionTypeControllerPrefKey, value);
        }
        private const string interactionTypeGazeAndClickPrefKey = "InteractionTypeGazeAndClick";
        private static bool interactionTypeGazeAndClick
        {
            get => EditorPrefs.GetBool(interactionTypeGazeAndClickPrefKey);
            set => EditorPrefs.SetBool(interactionTypeGazeAndClickPrefKey, value);
        }
        private const string interactionTypeGazeAndDwellPrefKey = "InteractionTypeGazeAndDwell";
        private static bool interactionTypeGazeAndDwell {
            get => EditorPrefs.GetBool(interactionTypeGazeAndDwellPrefKey);
            set => EditorPrefs.SetBool(interactionTypeGazeAndDwellPrefKey, value);
        }
        private const string interactionConfigurationPrefKey = "InteractionType";
        private static string interactionConfiguration
        {
            get => EditorPrefs.GetString(interactionConfigurationPrefKey);
            set => EditorPrefs.SetString(interactionConfigurationPrefKey, value);
        }


      
        #endregion

        #region Device Type Editor Prefs
        private const string deviceTypePROPrefKey = "DeviceTypePRO";
        private static bool deviceTypePRO
        {
            get => EditorPrefs.GetBool(deviceTypePROPrefKey);
            set => EditorPrefs.SetBool(deviceTypePROPrefKey, value);
        }
        private const string deviceTypeLITEPrefKey = "DeviceTypeLITE";
        private static bool deviceTypeLITE
        {
            get => EditorPrefs.GetBool(deviceTypeLITEPrefKey);
            set => EditorPrefs.SetBool(deviceTypeLITEPrefKey, value);
        }
        private const string deviceTypeCARDBOARDPrefKey = "DeviceTypeCARDBOARD";
        private static bool deviceTypeCARDBOARD
        {
            get => EditorPrefs.GetBool(deviceTypeCARDBOARDPrefKey);
            set => EditorPrefs.SetBool(deviceTypeCARDBOARDPrefKey, value);
        }        
        private const string deviceTypeHOLOBOARDPrefKey = "DeviceTypeHOLOBOARD";
        private static bool deviceTypeHOLOBOARD
        {
            get => EditorPrefs.GetBool(deviceTypeHOLOBOARDPrefKey);
            set => EditorPrefs.SetBool(deviceTypeHOLOBOARDPrefKey, value);
        }


        #endregion


        /// <summary>
        /// Completely Reset Android manifest [All permissions & attributes disabled]
        /// </summary>
        //[MenuItem("JioMixedReality/Manifest/ResetManifest", priority = 11)]
        public static void ResetAndroidManifest()
        {
            ResetPermissions();
            DisableRecentHistoryAttributes();

            Debug.Log("Status --> Android Manifest Reset");
        }

        /// <summary>
        /// Use to set the manifest to SM platform
        /// </summary>
        // [MenuItem("Update Manifest/SM Manifest Setup")]
        public static void SM_Setup()
        {
            currentPlatform = PlatformType.SM;
            ResetAndroidManifest();
            // DisableRecentHistoryAttributes();

            Debug.Log("Status --> SM Manifest Setup Completed");
        }

        /// <summary>
        /// Use to set the manifest to CU platform
        /// </summary>
        //  [MenuItem("Update Manifest/CU Manifest setup")]
        public static void CU_Setup()
        {
            currentPlatform = PlatformType.CU;
            ResetAndroidManifest();
            // EnableRecentHistoryAttributes();

            Debug.Log("Status --> CU Manifest Setup Completed");
        }

        #region PERMISSION EDITOR

        static string disabledCamera = $"<!--<uses-permission android:name=\"android.permission.CAMERA\"/>-->";
        static string enabledCamera = $"<uses-permission android:name=\"android.permission.CAMERA\"/>";

        static string disabledAudio = $"<!--<uses-permission android:name=\"android.permission.RECORD_AUDIO\"/>-->";
        static string enabledAudio = $"<uses-permission android:name=\"android.permission.RECORD_AUDIO\"/>";

        static string disableWriteExternalPermission = $"<uses-permission android:name=\"android.permission.WRITE_EXTERNAL_STORAGE\" tools:node=\"remove\" />";
        static string enableWriteExternalPermission = $"<uses-permission android:name=\"android.permission.WRITE_EXTERNAL_STORAGE\"/>";

        static string disableReadExternalPermission = $"<uses-permission android:name=\"android.permission.READ_EXTERNAL_STORAGE\" tools:node=\"remove\" />";
        static string enableReadExternalPermission = $"<uses-permission android:name=\"android.permission.READ_EXTERNAL_STORAGE\"/>";

        /// <summary>
        /// Toggles Audio and Camera permission at the same time [Recommended to use this method]
        /// By Default Permissions should be disabled
        /// </summary>
        //  [MenuItem("Update Manifest/Toggle Camera Audio Permission")]
        public static void ToggleCameraAudioPrmission()
        {
            TogglePermissions(enabledCamera, disabledCamera);
            TogglePermissions(enabledAudio, disabledAudio);
        }

        /// <summary>
        /// Toggles Read and write external storage permission at the same time [Recommended to use this method]
        /// By Default Permissions should be disabled
        /// </summary>
        //  [MenuItem("Update Manifest/Toggle Storage Permission")]
        public static void ToggleStoragePermissions()
        {
            TogglePermissions(enableWriteExternalPermission, disableWriteExternalPermission);
            TogglePermissions(enableReadExternalPermission, disableReadExternalPermission);
        }

        /// <summary>
        /// Remove all mentioned permissions
        /// [Camera,Audio,Write external,Read External]
        /// </summary>
        //[MenuItem("JioMixedReality/Manifest/ResetPermissions", priority = 12)]
        public static void ResetPermissions()
        {
            TogglePermissions(enabledCamera, disabledCamera, true);
            TogglePermissions(enabledAudio, disabledAudio, true);

            TogglePermissions(enableWriteExternalPermission, disableWriteExternalPermission, true);
            TogglePermissions(enableReadExternalPermission, disableReadExternalPermission, true);
        }

        public static void TogglePermissions(string enableString, string disableString, bool isReset = false)
        {
            string manifest = ReadString(manifestPath);

            if (string.IsNullOrEmpty(manifest))
            {
                Debug.LogError("Manifest not found."); return;
            }

            if (isReset)
            {
                if (manifest.Contains(disableString))
                {
                    //Debug.LogWarning("Status -->  Permission already disabled");
                    return;
                }
                else if (manifest.Contains(enableString))
                {
                    manifest = manifest.Replace(enableString, disableString);
                    Debug.Log("Status -->  Permission RESET");
                }
            }
            else
            {
                //*** Dont reverse these conditions , otherwise this toggel wont work properly ***
                if (manifest.Contains(disableString))
                {
                    manifest = manifest.Replace(disableString, enableString);
                    Debug.Log("Status -->  Permission Enabled");
                }
                else if (manifest.Contains(enableString))
                {
                    manifest = manifest.Replace(enableString, disableString);
                    Debug.Log("Status -->  Permission Disabled");
                }
            }
            WriteString(manifest, manifestPath);
        }
        #endregion

        #region Utilities

        static string ReadString(string readPath = "")
        {
            StreamReader reader = new StreamReader(readPath);
            string textInFile = reader.ReadToEnd();
            reader.Close();
            return textInFile;
        }

        static void WriteString(string text, string writePath = "")
        {
            StreamWriter writer = new StreamWriter(writePath);
            writer.Write(text);
            writer.Close();
        }

        #endregion

        #region XML ATTRIBUTE UPDATE

        static string xmlString = string.Empty;
        private const string xmlPath = "Assets/Plugins/Android/AndroidManifest.xml";

        private const string xmlActivityNodePath = "manifest/application/activity";

        private const string NoHistory = "android:noHistory";
        private const string ExcludeFromRecents = "android:excludeFromRecents";

        private const string xmlMetaDataNotePath = "//manifest/application/meta-data";
        private const string attributeKey_PRO_LITE = "com.jiotesseract.platform";
        private const string attributeKey_CATEGORY = "com.jiotesseract.mr.category";
        private const string attributeKey_INTERACTIONTYPE = "com.jiotesseract.mr.interactiontype";
        private const string attributeKey_LICENSEKEY = "com.jiotesseract.licensekey";
        private const string AndroidValue = "android:value";
        private const string AndroidName = "android:name";
        private static string[] CATEGORIES = {"0", "1", "2", "3", "4", "5", "6", "7"};
        private static string[] INTERACTIONTYPE = {"Controller", "GazeAndClick", "GazeAndDwell"};
        private static string[] DEVICETYPE = {"PRO", "LITE", "CARDBOARD","HOLOBOARD"};


        public static void DisableRecentHistoryAttributes()
        {
            UpdateXMLNonRecurringAttributes(xmlActivityNodePath, NoHistory, "false");
            UpdateXMLNonRecurringAttributes(xmlActivityNodePath, ExcludeFromRecents, "false");
        }

        public static void EnableRecentHistoryAttributes()
        {
            UpdateXMLNonRecurringAttributes(xmlActivityNodePath, NoHistory, "true");
            UpdateXMLNonRecurringAttributes(xmlActivityNodePath, ExcludeFromRecents, "true");
        }

        #region Configure Device Type
        [MenuItem("JioMixedReality/Manifest/Configure Device/Configure for PRO", priority = 1)]
        public static void SetDeviceTypePROCheckBoxBool()
        {
            deviceTypePRO = !deviceTypePRO;
            ConfigureDeviceAttributeStringValue();
        }

        [MenuItem("JioMixedReality/Manifest/Configure Device/Configure for PRO", true)]
        public static bool ConfigDeviceType_PRO()
        {
            Menu.SetChecked("JioMixedReality/Manifest/Configure Device/Configure for PRO", deviceTypePRO);
            return true;
        }


        
        [MenuItem("JioMixedReality/Manifest/Configure Device/Configure for LITE", priority = 2)]
        public static void SetDeviceTypeLITECheckBoxBool()
        {
            deviceTypeLITE = !deviceTypeLITE;
            ConfigureDeviceAttributeStringValue();
        }

        [MenuItem("JioMixedReality/Manifest/Configure Device/Configure for LITE", true)]
        public static bool ConfigDeviceType_LITE()
        {
            Menu.SetChecked("JioMixedReality/Manifest/Configure Device/Configure for LITE", deviceTypeLITE);
            return true;
        }

        

        
        [MenuItem("JioMixedReality/Manifest/Configure Device/Configure for CARDBOARD", priority = 3)]
        public static void SetDeviceTypeCARDBOARDCheckBoxBool()
        {
            deviceTypeCARDBOARD = !deviceTypeCARDBOARD;
            ConfigureDeviceAttributeStringValue();
        }

        [MenuItem("JioMixedReality/Manifest/Configure Device/Configure for CARDBOARD", true)]
        public static bool ConfigDeviceType_CARDBOARD()
        {
            Menu.SetChecked("JioMixedReality/Manifest/Configure Device/Configure for CARDBOARD", deviceTypeCARDBOARD);
            return true;
        }

        
        
        [MenuItem("JioMixedReality/Manifest/Configure Device/Configure for HOLOBOARD", priority = 4)]
        public static void SetDeviceTypeHOLOBOARDCheckBoxBool()
        {
            deviceTypeHOLOBOARD = !deviceTypeHOLOBOARD;
            ConfigureDeviceAttributeStringValue();
        }

        [MenuItem("JioMixedReality/Manifest/Configure Device/Configure for HOLOBOARD", true)]
        public static bool ConfigDeviceType_HOLOBOARD()
        {
            Menu.SetChecked("JioMixedReality/Manifest/Configure Device/Configure for HOLOBOARD", deviceTypeHOLOBOARD);
            return true;
        }

        #endregion

        #region Configure Category
        [MenuItem("JioMixedReality/Manifest/Configure Category/Entertainment")]
        public static void ConfigCategory_Entertainment()
        {
            UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, CATEGORIES[0], AndroidName, attributeKey_CATEGORY);
        }

        [MenuItem("JioMixedReality/Manifest/Configure Category/Gaming")]
        public static void ConfigCategory_Gaming()
        {
            UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, CATEGORIES[1], AndroidName, attributeKey_CATEGORY);
        }

        [MenuItem("JioMixedReality/Manifest/Configure Category/Learning")]
        public static void ConfigCategory_Learning()
        {
            UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, CATEGORIES[2], AndroidName, attributeKey_CATEGORY);
        }

        [MenuItem("JioMixedReality/Manifest/Configure Category/Productivity")]
        public static void ConfigCategory_Productivity()
        {
            UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, CATEGORIES[3], AndroidName, attributeKey_CATEGORY);
        }

        [MenuItem("JioMixedReality/Manifest/Configure Category/Utilities")]
        public static void ConfigCategory_Utilities()
        {
            UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, CATEGORIES[4], AndroidName, attributeKey_CATEGORY);
        }

        [MenuItem("JioMixedReality/Manifest/Configure Category/Health And Wellness")]
        public static void ConfigCategory_HealthAndWellness()
        {
            UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, CATEGORIES[5], AndroidName, attributeKey_CATEGORY);
        }

        [MenuItem("JioMixedReality/Manifest/Configure Category/Shopping")]
        public static void ConfigCategory_Shopping()
        {
            UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, CATEGORIES[6], AndroidName, attributeKey_CATEGORY);
        }

        [MenuItem("JioMixedReality/Manifest/Configure Category/Miscellaneous")]
        public static void ConfigCategory_Miscellaneous()
        {
            UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, CATEGORIES[7], AndroidName, attributeKey_CATEGORY);
        }
        #endregion

        #region Configure Interaction
        [MenuItem("JioMixedReality/Manifest/Configure Interaction/Controller", priority =1)]
        public static void SetControllerCheckBoxBool()
        {
            interactionTypeController = !interactionTypeController;
            ConfigureInteractionAttributeStringValue();
        }
                
        [MenuItem("JioMixedReality/Manifest/Configure Interaction/Controller", true)]
        public static bool ConfigInteraction_ControllerlInteraction()
        {
            Menu.SetChecked("JioMixedReality/Manifest/Configure Interaction/Controller", interactionTypeController);
            return true;
        }
        
        [MenuItem("JioMixedReality/Manifest/Configure Interaction/Gaze and Click",priority =2)]
        public static void SetGazeAndClickCheckBoxBool()
        {
            interactionTypeGazeAndClick = !interactionTypeGazeAndClick;
            ConfigureInteractionAttributeStringValue();
        } 
                
        [MenuItem("JioMixedReality/Manifest/Configure Interaction/Gaze and Click",true)]
        public static bool ConfigInteraction_GazeAndClickInteraction()
        {
            Menu.SetChecked("JioMixedReality/Manifest/Configure Interaction/Gaze and Click", interactionTypeGazeAndClick);
            return true;
        }

        [MenuItem("JioMixedReality/Manifest/Configure Interaction/Gaze and Dwell",priority =3)]
        public static void SetGazeAndDwellCheckBoxBool()
        {
            interactionTypeGazeAndDwell = !interactionTypeGazeAndDwell;
            ConfigureInteractionAttributeStringValue();
        }
        
        [MenuItem("JioMixedReality/Manifest/Configure Interaction/Gaze and Dwell",true)]
        public static bool ConfigInteraction_GazeAndDwellInteraction()
        {
            Menu.SetChecked("JioMixedReality/Manifest/Configure Interaction/Gaze and Dwell", interactionTypeGazeAndDwell);
            return true;
        }
        #endregion

        #region Configure License
        public static void ConfigureLisenceKey(string key)
        {
            UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, key , AndroidName, attributeKey_LICENSEKEY);
        }
        #endregion

        private static void ConfigureInteractionAttributeStringValue()
        {

            string interactionTypeStringVal = "";
            if (interactionTypeController)
            {
                interactionTypeStringVal = INTERACTIONTYPE[0];
            }

            if (interactionTypeGazeAndClick)
            {
                if(string.IsNullOrEmpty(interactionTypeStringVal))
                {
                    interactionTypeStringVal = INTERACTIONTYPE[1];
                }
                else
                {
                    interactionTypeStringVal= string.Concat(interactionTypeStringVal,"|",INTERACTIONTYPE[1]);
                }
            }
            if (interactionTypeGazeAndDwell)
            {
                if(string.IsNullOrEmpty(interactionTypeStringVal))
                {
                    interactionTypeStringVal = INTERACTIONTYPE[2];
                }
                else
                {
                    interactionTypeStringVal= string.Concat(interactionTypeStringVal,"|",INTERACTIONTYPE[2]);
                }
            }
                Debug.LogWarning("JMRSDK=> Interaction Attribute Key Set =>> " + interactionTypeStringVal);
                UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, interactionTypeStringVal, AndroidName, attributeKey_INTERACTIONTYPE);
        }        
        private static void ConfigureDeviceAttributeStringValue()
        {

            string deviceTypeStringVal = "";
            if (deviceTypePRO)
            {
                deviceTypeStringVal = DEVICETYPE[0];
            }

            if (deviceTypeLITE)
            {
                if(string.IsNullOrEmpty(deviceTypeStringVal))
                {
                    deviceTypeStringVal = DEVICETYPE[1];
                }
                else
                {
                    deviceTypeStringVal = string.Concat(deviceTypeStringVal, "|",DEVICETYPE[1]);
                }
            }
            if (deviceTypeCARDBOARD)
            {
                if(string.IsNullOrEmpty(deviceTypeStringVal))
                {
                    deviceTypeStringVal = DEVICETYPE[2];
                }
                else
                {
                    deviceTypeStringVal = string.Concat(deviceTypeStringVal, "|",DEVICETYPE[2]);
                }
               
            }            
            if (deviceTypeHOLOBOARD)
            {
                if(string.IsNullOrEmpty(deviceTypeStringVal))
                {
                    deviceTypeStringVal = DEVICETYPE[3];
                }
                else
                {
                    deviceTypeStringVal = string.Concat(deviceTypeStringVal, "|",DEVICETYPE[3]);
                }
            }
                Debug.LogWarning("JMRSDK=> Device Type Attribute Key Set =>> " + deviceTypeStringVal);
                 UpdateXMLRecurringAttributes(xmlMetaDataNotePath, AndroidValue, deviceTypeStringVal, AndroidName, attributeKey_PRO_LITE);
        }

        /// <summary>
        /// For unique node attributes (which doesnt repeat in manifest)
        /// </summary>
        /// <param name="xmlNodePath"></param>
        /// <param name="xmlAttributeName"></param>
        /// <param name="value"></param>
        private static void UpdateXMLNonRecurringAttributes(string xmlNodePath, string xmlAttributeName, string value)
        {
            xmlString = File.ReadAllText(xmlPath);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            //var element = doc.SelectSingleNode(xmlNodePath) as XmlElement;
            var list = doc.SelectNodes(xmlNodePath);

            foreach (XmlElement element in list)
            {
                if (element != null && element.HasAttribute(xmlAttributeName))
                {
                    element.SetAttribute(xmlAttributeName, value.ToString());
                    doc.Save(xmlPath);
                }
                else
                {
                    Debug.LogError("XML Element " + xmlAttributeName + " not found. Please check the PATH");
                }
            }
        }

        /// <summary>
        /// For re-curring node attributes (which repeats in manifest)
        /// </summary>
        /// <param name="xmlNodePath"></param>
        /// <param name="xmlTargetAttributeName"></param>
        /// <param name="value"></param>
        /// <param name="xmlSearchAttribute"></param>
        /// <param name="xmlSearchAttributeKey"></param>
        private static void UpdateXMLRecurringAttributes(string xmlNodePath, string xmlTargetAttributeName, string value, string xmlSearchAttribute, string xmlSearchAttributeKey)
        {
            xmlString = File.ReadAllText(xmlPath);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            var list = doc.SelectNodes(xmlNodePath);

            foreach (XmlElement element in list)
            {
                if (element != null && element.HasAttribute(xmlSearchAttribute) && element.HasAttribute(xmlTargetAttributeName))
                {
                    if (element.Attributes[xmlSearchAttribute].Value == xmlSearchAttributeKey)
                    {
                        element.SetAttribute(xmlTargetAttributeName, value.ToString());
                        doc.Save(xmlPath);
                    }
                }
                else
                {
                    Debug.LogError("XML Element not found. Please check the PATH");
                }
            }
        }

        private static void readXMLAttributes(string xmlNodePath, string xmlTargetAttributeName, string xmlSearchAttribute, string xmlSearchAttributeKey)
        {
            xmlString = File.ReadAllText(xmlPath);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            var list = doc.SelectNodes(xmlNodePath);

            foreach (XmlElement element in list)
            {
                    Debug.LogError(element.ToString());
                if (element != null && element.HasAttribute(xmlSearchAttribute) && element.HasAttribute(xmlTargetAttributeName))
                {

                    if (element.Attributes[xmlSearchAttribute].Value == xmlSearchAttributeKey)
                    {
                        Debug.LogError(element.ToString());   
                    }
                }
                else
                {
                    Debug.LogError("XML Element not found. Please check the PATH");
                }
            }
        }

        #endregion


        public static void  WarningDialog()
        {
            bool popUpConfirmation= EditorUtility.DisplayDialog("Revalidating", "Please ensure you have entered correct DeviceType, InteractionType and License Key", "Proceed with the Build", "Let me Check");
            if (!popUpConfirmation)
                throw new BuildFailedException("Configure DeviceType, InteractionType and License Key in the editor. ");
        }

    }

    public class LicenseDiaplayWindow: EditorWindow
    {
        string licenseText = "";

        void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Get the License Key generated from developer console", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);
            licenseText = EditorGUILayout.TextField("Key", licenseText);

            if (GUILayout.Button("Save"))
            {
                ManifestConfig.ConfigureLisenceKey(licenseText);
                Close();
            }
        }

        [MenuItem("JioMixedReality/Manifest/Configure License Key", priority = 1)]
        private static void ShowWindow()
        {
            LicenseDiaplayWindow window = new LicenseDiaplayWindow();
            window.position = new Rect(Screen.width , Screen.height , 350, 100);
            window.ShowPopup();
        }


    }

#if UNITY_EDITOR
    public class DisplayDialog : IPreprocessBuild
    {

        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildTarget buildTarget, string str)
        {
           ManifestConfig.WarningDialog();
        }

    }

#endif

}