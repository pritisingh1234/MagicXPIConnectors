using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using Magicsoftware.iBolt.Common.Configuration;
using Magicsoftware.iBolt.Common.Utilities;

namespace MagicSoftware.MQTT
{
   class MQTTResourceManager
   {

      private static global::System.Globalization.CultureInfo resourceCulture;
      private static ResourceManager resourceManager = null;

      [ExportedResourceManager("mqtt")]
      public static ResourceManager ResourceManager
      {
         get
         {
            if (ReferenceEquals(resourceManager, null))
            {
               var debugResources = false;
               var sharedSettings = SharedSettings.Default;
               if (sharedSettings != null)
               {
                  debugResources = SharedSettings.Default.GetBooleanProperty("DebugLocalization");
               }
               resourceManager = debugResources ?
                   new DebuggingResourceManager("MQTTResources", typeof(MQTTResourceManager).Assembly) :
                   new ResourceManager("MQTTResources", typeof(MQTTResourceManager).Assembly);
            }
            return resourceManager;
         }
      }

      /// <summary>
      ///   Overrides the current thread's CurrentUICulture property for all
      ///   resource lookups using this strongly typed resource class.
      /// </summary>
      [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
      public static global::System.Globalization.CultureInfo Culture
      {
         get
         {
            return resourceCulture;
         }
         set
         {
            resourceCulture = value;
         }
      }


      public static string OK
      {
         get
         {
            return ResourceManager.GetString("OK", resourceCulture);
         }
      }

      public static string Cancel
      {
         get
         {
            return ResourceManager.GetString("Cancel", resourceCulture);
         }
      }

      public static string HdrConnection
      {
         get
         {
            return ResourceManager.GetString("hdrConnection", resourceCulture);
         }
      }

      public static string LblResourceName
      {
         get
         {
            return ResourceManager.GetString("lblResourceName", resourceCulture);
         }
      }
      public static string HdrPublishSettings
      {
         get
         {
            return ResourceManager.GetString("hdrPublishSettings", resourceCulture);
         }
      }

      public static string LblQoS
      {
         get
         {
            return ResourceManager.GetString("lblQoS", resourceCulture);
         }
      }

      public static string LblRetained
      {
         get
         {
            return ResourceManager.GetString("lblRetained", resourceCulture);
         }
      }

      public static string HdrResultOptions
      {
         get
         {
            return ResourceManager.GetString("hdrResultOptions", resourceCulture);
         }
      }

      public static string LblStoreResultIn
      {
         get
         {
            return ResourceManager.GetString("lblStoreResultIn", resourceCulture);
         }
      }
      public static string LblOperationSuccess
      {
         get
         {
            return ResourceManager.GetString("lblOperationSuccess", resourceCulture);
         }
      }
      public static string StepWindowTitle
      {
         get
         {
            return ResourceManager.GetString("StepWindowTitle", resourceCulture);
         }
      }

      public static string BtnNew
      {
         get
         {
            return ResourceManager.GetString("btnNew", resourceCulture);
         }
      }

      public static string BtnDelete
      {
         get
         {
            return ResourceManager.GetString("btnDelete", resourceCulture);
         }
      }

      public static string HdrTopic
      {
         get
         {
            return ResourceManager.GetString("hdrTopic", resourceCulture);
         }
      }

      public static string HdrQoS
      {
         get
         {
            return ResourceManager.GetString("hdrQoS", resourceCulture);
         }
      }

      public static string HdrMessagePayload
      {
         get
         {
            return ResourceManager.GetString("hdrMessagePayload", resourceCulture);
         }
      }

      public static string HdrMessageTopic
      {
         get
         {
            return ResourceManager.GetString("hdrMessageTopic", resourceCulture);
         }
      }


      public static string TriggerWindowTitle
      {
         get
         {
            return ResourceManager.GetString("TriggerWindowTitle", resourceCulture);
         }
      }

      public static string Error
      {
         get
         {
            return ResourceManager.GetString("Error", resourceCulture);
         }
      }

      public static string Information
      {
         get
         {
            return ResourceManager.GetString("Information", resourceCulture);
         }
      }

      public static string MQTTConnectionSuccessfullMsg
      {
         get
         {
            return ResourceManager.GetString("MQTTConnectionSuccessfullMsg", resourceCulture);
         }
      }

      public static string MTTTBrokerURLsCantBeEmptyMsg
      {
         get
         {
            return ResourceManager.GetString("MTTTBrokerURLsCantBeEmptyMsg", resourceCulture);
         }
      }

      public static string MQTTShouldDefineConfigurationDialogsMsg
      {
         get
         {
            return ResourceManager.GetString("MQTTShouldDefineConfigurationDialogsMsg", resourceCulture);
         }
      }

      public static string MQTTStoreResultValueInFileNameMsg
      {
         get
         {
            return ResourceManager.GetString("MQTTStoreResultValueInFileNameMsg", resourceCulture);
         }
      }

      public static string MQTTStoreResultValueInVariableNameMsg
      {
         get
         {
            return ResourceManager.GetString("MQTTStoreResultValueInVariableNameMsg", resourceCulture);
         }
      }

      public static string MQTTSelectVariableToStoreOperationStatusMsg
      {
         get
         {
            return ResourceManager.GetString("MQTTSelectVariableToStoreOperationStatusMsg", resourceCulture);
         }
      }

      public static string MQTTGenerateIDCaption
      {
         get
         {
            return ResourceManager.GetString("Generate ID", resourceCulture);
         }
      }

      public static string MQTTOverwriteClientIdValueMsg
      {
         get
         {
            return ResourceManager.GetString("MQTTOverwriteClientIdValueMsg", resourceCulture);
         }
      }

      public static string MQTTTopicFieldCantBeEmptyMsg
      {
         get
         {
            return ResourceManager.GetString("MQTTTopicFieldCantBeEmptyMsg", resourceCulture);
         }
      }

      public static string MQTTTopicIsNotValidMsg
      {
         get
         {
            return ResourceManager.GetString("MQTTTopicIsNotValidMsg", resourceCulture);
         }
      }

      public static string FilePath
      {
         get
         {
            return ResourceManager.GetString("FilePath", resourceCulture);
         }
      }

      public static string SelectAFile
      {
         get
         {
            return ResourceManager.GetString("SelectAFile", resourceCulture);
         }
      }
       public static string MQTTRetainedYes
        {
           get
           {
               return ResourceManager.GetString("MQTTRetainedYes", resourceCulture);
           }
       }
       public static string MQTTRetainedNo
       {
           get
           {
               return ResourceManager.GetString("MQTTRetainedNo", resourceCulture);
           }
       }
       public static string MQTTStoreResultInVariable
       {
           get
           {
               return ResourceManager.GetString("MQTTStoreResultInVariable", resourceCulture);
           }
       }
       public static string MQTTStoreResultInFile
       {
           get
           {
               return ResourceManager.GetString("MQTTStoreResultInFile", resourceCulture);
           }
       }
       public static string MQTTOperationSuccessVariable
        {
           get
           {
               return ResourceManager.GetString("MQTTOperationSuccessVariable", resourceCulture);
           }
       }
       public static string MQTTOperationSuccessNone
        {
           get
           {
               return ResourceManager.GetString("MQTTOperationSuccessNone", resourceCulture);
           }
       }
   }
}
