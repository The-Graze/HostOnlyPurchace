using BepInEx;
using System;
using System.Collections;
using UnityEngine;
using HarmonyLib;
using static Mono.Security.X509.X520;
namespace HostOnlyPurchace
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        Plugin() => HarmonyPatches.ApplyHarmonyPatches();
                   
    }
    public static class ValuesETC
    {
        public static TerminalNode HostOnly = new TerminalNode()
        {
            displayText = "Only The host can Purchase Items or route the Ship.\n\nClosing Terminal in 3 seconds",
            clearPreviousText = true,
            acceptAnything = true,
        };
        public static IEnumerator Close(Terminal term)
        {
            yield return new WaitForSeconds(3);
            term.QuitTerminal();
        }
    }

    [HarmonyPatch(typeof(Terminal))]
    [HarmonyPatch("LoadNewNode")]
    static class TerminalPatch
    {
        static bool Prefix(TerminalNode node, Terminal __instance)
        {
            if (!RoundManager.Instance.IsHost)
            {
                if (node.isConfirmationNode || node.name.Contains("route"))
                {
                    __instance.LoadNewNode(ValuesETC.HostOnly);
                    __instance.StartCoroutine(ValuesETC.Close(__instance));
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
    }
}   