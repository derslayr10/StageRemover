using BepInEx;
using R2API;
using R2API.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RoR2;
using System;
using System.Linq;

namespace StageRemover
{
    //this line adds the R2API dependency
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]

    /*add other mod dependencies here using above format*/

    //this line determines whether the mod is client side or server side (default)
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    //this line defines your mod name to BepIn, set later
    [BepInPlugin(ModGuid, ModName, ModVer)]

    //this line adds in the R2API dependencies required
    [R2APISubmoduleDependency(nameof(ResourcesAPI), nameof(ItemAPI), nameof(LanguageAPI), nameof(PrefabAPI))]

    //defines the mod
    public class Main : BaseUnityPlugin
    {

        //enter the mod GUID inside quotes. Ex: com.Derslayr.CreateItemTemplate
        public const string ModGuid = "com.Derslayr.StageRemover";
        //enter the mod name inside quotes. Ex: TwistedLoopMod
        public const string ModName = "Stage Remover";
        //enter the mod version inside quotes. Ex: 1.0.0
        public const string ModVer = "1.0.0";

        public void RunConfig()
        {



        }

        public void Awake()
        {

            Logger.LogMessage("Loaded Stage Remover");
            Logger.LogMessage("Running Hooks...");
            Hooks();
            Logger.LogMessage("Hooks Loaded");

        }

        public void Hooks()
        {

            On.RoR2.Run.Start += (orig, self) => {

                Logger.LogWarning("PREPARING TO REMOVE STAGE: Sundered Grove aka 'rootjungle'");
                Logger.LogMessage("Loading Run...");

                orig(self);

                Logger.LogMessage("Run started... Checking destinations...");

                RemoveStage();

            };

        }

        public void RemoveStage() {

            foreach (SceneDef scene in SceneCatalog.allStageSceneDefs) {

                if (scene.destinations.Contains<SceneDef>(SceneCatalog.GetSceneDefFromSceneName("rootjungle"))) {

                    for (int i = 0; i < scene.destinations.Length; i++)
                    {

                        if (scene.destinations[i].baseSceneName == "rootjungle"){

                            Logger.LogWarning("Found Sundered Grove... Removing...");

                            scene.destinations = RemoveEntry(scene.destinations, i);

                        }

                    }

                }

            }

        }

        public SceneDef[] RemoveEntry(SceneDef[] destinationList, int index) {

            var newDestinations = new List<SceneDef>(destinationList);
            newDestinations.RemoveAt(index);

            if (!newDestinations.Contains(SceneCatalog.GetSceneDefFromSceneName("rootjungle"))){

                Logger.LogWarning("Successfully Removed Sundered Grove from Stage destinations");

            }

            return newDestinations.ToArray();

        }

    }

}
