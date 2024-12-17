using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LaseaAtlyssSimpleModelSwapper
{
    [BepInPlugin("atlyss.mod.abysslasea.AtlyssSimpleModelSwapper", "Atlyss_Simple_Model_Swapper", "1.0")]
    public class LaseaAtlyssSimpleModelSwapper : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony harmony = new Harmony("atlyss.mod.abysslasea.AtlyssSimpleModelSwapper");
            harmony.PatchAll();
            LaseaAtlyssSimpleModelSwapper.loadModel();
        }

        public static void getFolders(string filePath, List<string> model)
        {
            foreach (string text in Directory.GetDirectories(filePath))
            {
                foreach (string item in Directory.GetFiles(text))
                {
                    model.Add(item);
                }
                LaseaAtlyssSimpleModelSwapper.getFolders(text, model);
            }
        }
        public static void loadModel()
        {
            try
            {
                string text = Directory.GetCurrentDirectory() + "/BepInEx/plugins/assets/model";
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                List<string> list = new List<string>();
                foreach (string item in Directory.GetFiles(text))
                {
                    list.Add(item);
                }
                LaseaAtlyssSimpleModelSwapper.getFolders(text, list);
                Object[] array = Resources.FindObjectsOfTypeAll(typeof(Mesh));
                foreach (string text2 in list)
                {
                    string[] array2 = text2.Split(new char[]
                    {
                '\\',
                '.'
                    });
                    int num = array2.Length - 2;
                    foreach (Mesh mesh in array)
                    {
                        if (mesh.name == array2[num])
                        {
                            byte[] array4 = File.ReadAllBytes(text2);

                            string path = Path.Combine(text, array2[num]);
                            File.WriteAllBytes(path, array4);
                            Mesh Mesh = Resources.Load<Mesh>(path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to load model: " + ex.Message);
            }

        }
        [HarmonyPatch]
        private class textureSwapper_patch
        {
            [HarmonyPatch(typeof(GameManager), "Awake")]
            [HarmonyPostfix]
            public static void textureSwapper_prefix(GameManager __instance)
            {
                LaseaAtlyssSimpleModelSwapper.loadModel();
            }
            [HarmonyPatch(typeof(LoadSceneManager), "Disable_LoadScreen")]
            [HarmonyPostfix]
            public static void textureSwapper_prefix(LoadSceneManager __instance)
            {
                LaseaAtlyssSimpleModelSwapper.loadModel();
            }
        }
    }
}
