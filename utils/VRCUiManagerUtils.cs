﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VRCModLoader;

namespace VRCTools
{
    public static class VRCUiManagerUtils
    {

        private static VRCUiManager uiManagerInstance;
        private static EventInfo onPageShown;

        public static event Action<VRCUiPage> OnPageShown
        {
            add => GetOnPageShown()?.AddEventHandler(GetVRCUiManager(), value);
            remove => GetOnPageShown()?.RemoveEventHandler(GetVRCUiManager(), value);
        }

        public static VRCUiManager GetVRCUiManager()
        {
            if (uiManagerInstance == null)
            {
                FieldInfo[] nonpublicStaticPopupFields = typeof(VRCUiManager).GetFields(BindingFlags.NonPublic | BindingFlags.Static);
                if (nonpublicStaticPopupFields.Length == 0)
                {
                    VRCModLogger.Log("[VRCUiManagerUtils] nonpublicStaticPopupFields.Length == 0");
                    return null;
                }
                FieldInfo uiManagerInstanceField = nonpublicStaticPopupFields.First(field => field.FieldType == typeof(VRCUiManager));
                if (uiManagerInstanceField == null)
                {
                    VRCModLogger.Log("[VRCUiManagerUtils] uiManagerInstanceField == null");
                    return null;
                }
                uiManagerInstance = uiManagerInstanceField.GetValue(null) as VRCUiManager;
            }

            return uiManagerInstance;
        }

        public static IEnumerator WaitForUiManagerInit()
        {
            VRCModLogger.Log("WaitForUIManager");
            if (uiManagerInstance == null)
            {
                FieldInfo[] nonpublicStaticFields = typeof(VRCUiManager).GetFields(BindingFlags.NonPublic | BindingFlags.Static);
                if (nonpublicStaticFields.Length == 0)
                {
                    VRCModLogger.Log("[VRCUiManagerUtils] nonpublicStaticFields.Length == 0");
                    yield break;
                }
                FieldInfo uiManagerInstanceField = nonpublicStaticFields.First(field => field.FieldType == typeof(VRCUiManager));
                if (uiManagerInstanceField == null)
                {
                    VRCModLogger.Log("[VRCUiManagerUtils] uiManagerInstanceField == null");
                    yield break;
                }
                uiManagerInstance = uiManagerInstanceField.GetValue(null) as VRCUiManager;
                VRCModLogger.Log("[VRCUiManagerUtils] Waiting for UI Manager...");
                while (uiManagerInstance == null)
                {
                    uiManagerInstance = uiManagerInstanceField.GetValue(null) as VRCUiManager;
                    yield return null;
                }
                VRCModLogger.Log("[VRCUiManagerUtils] UI Manager loaded");
            }
        }

        private static EventInfo GetOnPageShown()
        {
            if (onPageShown == null)
            {
                EventInfo[] events = typeof(VRCUiManager).GetEvents(BindingFlags.Public | BindingFlags.Instance);
                foreach (EventInfo fi in events)
                {
                    if (fi.EventHandlerType == typeof(Action<VRCUiPage>))
                    {
                        onPageShown = fi;
                        break;
                    }
                }
                if (onPageShown == null)
                {
                    VRCModLogger.LogError("[VRCUiManagerUtils] Unable to find VRCUiManager.onPageShown");
                    return null;
                }
            }
            return onPageShown;
        }
    }
}
