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
        public const string ModVer = "2.0.0";

        //Define configs
        public static bool EnableRoost { get; set; }
        public static bool EnablePlains { get; set; }
        public static bool EnableAqueduct { get; set; }
        public static bool EnableWetland { get; set; }
        public static bool EnableRallypoint { get; set; }
        public static bool EnableAcres { get; set; }
        public static bool EnableDepths { get; set; }
        public static bool EnableSiren { get; set; }
        public static bool EnableGrove { get; set; }
        public static bool EnableMeadow { get; set; }

        public static string CustomStages { get; set; }

        public HashSet<SceneDef> Stage1Set = new HashSet<SceneDef>();
        public HashSet<SceneDef> Stage2Set = new HashSet<SceneDef>();
        public HashSet<SceneDef> Stage3Set = new HashSet<SceneDef>();
        public HashSet<SceneDef> Stage4Set = new HashSet<SceneDef>();
        public HashSet<SceneDef> Stage5Set = new HashSet<SceneDef>();
        public HashSet<SceneDef> FailsafeSet = new HashSet<SceneDef>();

        public List<SceneDef> StagesToRemove = new List<SceneDef>();

        public void RunConfig() {

            EnableRoost = Config.Bind<bool>("Vanilla Maps", "enableRoost", true, "Enable Distant Roost in Stage Destinations? Default: true").Value;
            EnablePlains = Config.Bind<bool>("Vanilla Maps", "enablePlains", true, "Enable Titanic Plains in Stage Destinations? Default: true").Value;
            EnableAqueduct = Config.Bind<bool>("Vanilla Maps", "enableAqueduct", true, "Enable Abandoned Aqueduct in Stage Destinations? Default: true").Value;
            EnableWetland = Config.Bind<bool>("Vanilla Maps", "enableWetland", true, "Enable Wetland Aspect in Stage Destinations? Default: true").Value;
            EnableRallypoint = Config.Bind<bool>("Vanilla Maps", "enableRallypoint", true, "Enable Rallypoint Delta in Stage Destinations? Default: true").Value;
            EnableAcres = Config.Bind<bool>("Vanilla Maps", "enableAcres", true, "Enable Scorched Acres in Stage Destinations? Default: true").Value;
            EnableDepths = Config.Bind<bool>("Vanilla Maps", "enableDepths", true, "Enable Abyssal Depths in Stage Destinations? Default: true").Value;
            EnableSiren = Config.Bind<bool>("Vanilla Maps", "enableSiren", true, "Enable Siren's Call in Stage Destinations? Default: true").Value;
            EnableGrove = Config.Bind<bool>("Vanilla Maps", "enableGrove", true, "Enable Sundered Grove in Stage Destinations? Default: true").Value;
            EnableMeadow = Config.Bind<bool>("Vanilla Maps", "enableMeadow", true, "Enable Sky Meadow in Stage Destinations? Default: true").Value;

            CustomStages = Config.Bind<string>("Custom Stages", "customStages", "", "Please enter the Scene Name of each modded map you wish to remove (Ex: Titanic Plains has scene name 'golemplains'). Separate each entry with a comma.").Value;

        }

        public void DefineStagesToRemove() {

            if (!EnableRoost) {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("blackbeach"));
            
            }

            if (!EnablePlains)
            {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("golemplains"));

            }

            if (!EnableAqueduct)
            {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("goolake"));

            }

            if (!EnableWetland)
            {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("foggyswamp"));

            }

            if (!EnableRallypoint)
            {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("frozenwall"));

            }

            if (!EnableAcres)
            {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("wispgraveyard"));

            }
            
            if (!EnableDepths)
            {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("dampcavesimple"));

            }

            if (!EnableSiren)
            {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("shipgraveyard"));

            }

            if (!EnableGrove)
            {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("rootjungle"));

            }

            if (!EnableMeadow)
            {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName("skymeadow"));

            }

            string[] customremoval = CustomStages.Split(',');
            foreach (var scene in customremoval) {

                StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName(scene));
            
            }

            GenerateStageLists();

        }

        public void GenerateStageLists() {

            var startingStages = Run.instance.startingScenes;

            foreach (SceneDef scene in startingStages) {

                for (int i = 0; i < scene.destinations.Length; i++) {

                    Stage1Set.Add(scene.destinations[i]);
                
                }
            
            }

            foreach (SceneDef scene in Stage1Set)
            {

                for (int i = 0; i < scene.destinations.Length; i++)
                {

                    Stage2Set.Add(scene.destinations[i]);

                }

            }

            foreach (SceneDef scene in Stage2Set)
            {

                for (int i = 0; i < scene.destinations.Length; i++)
                {

                    Stage3Set.Add(scene.destinations[i]);

                }

            }

            foreach (SceneDef scene in Stage3Set)
            {

                for (int i = 0; i < scene.destinations.Length; i++)
                {

                    Stage4Set.Add(scene.destinations[i]);

                }

            }

            foreach (SceneDef scene in Stage4Set)
            {

                for (int i = 0; i < scene.destinations.Length; i++)
                {

                    Stage5Set.Add(scene.destinations[i]);

                }

            }

            for (int i = 0; i < StagesToRemove.Count; i++) {

                if (Stage1Set.Contains(StagesToRemove[i])) {

                    Stage1Set.Remove(StagesToRemove[i]);
                
                }

                if (Stage2Set.Contains(StagesToRemove[i]))
                {

                    Stage2Set.Remove(StagesToRemove[i]);

                }

                if (Stage3Set.Contains(StagesToRemove[i]))
                {

                    Stage3Set.Remove(StagesToRemove[i]);

                }

                if (Stage4Set.Contains(StagesToRemove[i]))
                {

                    Stage4Set.Remove(StagesToRemove[i]);

                }

                if (Stage5Set.Contains(StagesToRemove[i]))
                {

                    Stage5Set.Remove(StagesToRemove[i]);

                }

            }

        }

        public void Awake()
        {

            RunConfig();
            DefineStagesToRemove();
            Logger.LogMessage("Loaded Stage Remover");
            Logger.LogMessage("Running Hooks...");
            Hooks();
            Logger.LogMessage("Hooks Loaded");

        }

        public void Hooks()
        {

            On.RoR2.Run.PickNextStageScene += PickValidScene;

        }

        private void PickValidScene(On.RoR2.Run.orig_PickNextStageScene orig, Run self, SceneDef[] choices)
        {

            if (StagesToRemove.Contains(Run.instance.nextStageScene))
            {

                foreach (SceneDef scene in Run.instance.nextStageScene.destinations)
                {

                    FailsafeSet.Add(scene);

                }

                for (int i = 0; i < StagesToRemove.Count; i++)
                {

                    if (FailsafeSet.Contains(StagesToRemove[i]))
                    {

                        FailsafeSet.Remove(StagesToRemove[i]);

                    }

                }

                if (Run.instance.nextStageScene.destinations.Contains(SceneCatalog.GetSceneDefFromSceneName("blackbeach")))
                {

                    if (!Stage1Set.Any())
                    {
                        if (!FailsafeSet.Any())
                        {

                            Logger.LogError("ERROR: NO VALID SCENE, USING DEFAULT DESTINATIONS");
                            choices = Run.instance.nextStageScene.destinations;

                        }

                        else
                        {

                            choices = FailsafeSet.ToArray();

                        }

                    }

                    else
                    {

                        choices = Stage1Set.ToArray();

                    }

                }

                else if (Run.instance.nextStageScene.destinations.Contains(SceneCatalog.GetSceneDefFromSceneName("goolake")))
                {

                    if (!Stage2Set.Any())
                    {
                        if (!FailsafeSet.Any())
                        {

                            Logger.LogError("ERROR: NO VALID SCENE, USING DEFAULT DESTINATIONS");
                            choices = Run.instance.nextStageScene.destinations;

                        }

                        else
                        {

                            choices = FailsafeSet.ToArray();

                        }

                    }

                    else
                    {

                        choices = Stage2Set.ToArray();

                    }

                }

                else if (Run.instance.nextStageScene.destinations.Contains(SceneCatalog.GetSceneDefFromSceneName("frozenwall")))
                {

                    if (!Stage3Set.Any())
                    {
                        if (!FailsafeSet.Any())
                        {

                            Logger.LogError("ERROR: NO VALID SCENE, USING DEFAULT DESTINATIONS");
                            choices = Run.instance.nextStageScene.destinations;

                        }

                        else
                        {

                            choices = FailsafeSet.ToArray();

                        }

                    }

                    else
                    {

                        choices = Stage3Set.ToArray();

                    }

                }

                else if (Run.instance.nextStageScene.destinations.Contains(SceneCatalog.GetSceneDefFromSceneName("rootjungle")))
                {

                    if (!Stage4Set.Any())
                    {
                        if (!FailsafeSet.Any())
                        {

                            Logger.LogError("ERROR: NO VALID SCENE, USING DEFAULT DESTINATIONS");
                            choices = Run.instance.nextStageScene.destinations;

                        }

                        else
                        {

                            choices = FailsafeSet.ToArray();

                        }

                    }

                    else
                    {

                        choices = Stage4Set.ToArray();

                    }

                }

                else if (Run.instance.nextStageScene.destinations.Contains(SceneCatalog.GetSceneDefFromSceneName("skymeadow")))
                {

                    if (!Stage5Set.Any())
                    {
                        if (!FailsafeSet.Any())
                        {

                            Logger.LogError("ERROR: NO VALID SCENE, USING DEFAULT DESTINATIONS");
                            choices = Run.instance.nextStageScene.destinations;

                        }

                        else
                        {

                            choices = FailsafeSet.ToArray();

                        }

                    }

                    else
                    {

                        choices = Stage5Set.ToArray();

                    }

                }

                Run.instance.PickNextStageScene(choices);

            }

            orig(self, choices);

        }

    }

}
