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

        public List<HashSet<SceneDef>> StageCategories = new List<HashSet<SceneDef>>();
        public List<string> CompareList = new List<string>();

        SceneDef CurrentScene;

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

            if (customremoval[0].Equals("")) {

                customremoval = Array.Empty<string>();
            
            }

            if (customremoval.Any()) {

                foreach (var scene in customremoval)
                {

                    StagesToRemove.Add(SceneCatalog.GetSceneDefFromSceneName(scene));

                }

            }

            GenerateStageLists();

        }

        public HashSet<SceneDef> MakeSet(HashSet<SceneDef> sceneSet, List<SceneDef> inputList) {

            foreach (SceneDef scene in inputList) {

                for (int i = 0; i < scene.destinations.Length; i++) {

                    sceneSet.Add(scene.destinations[i]);
                
                }
            
            }

            return sceneSet;
        
        }

        public HashSet<SceneDef> MakeStartSet(HashSet<SceneDef> sceneSet, List<SceneDef> inputList)
        {

            foreach (SceneDef scene in inputList)
            {

                sceneSet.Add(scene);

            }

            return sceneSet;

        }

        public HashSet<SceneDef> RemoveScenes(HashSet<SceneDef> sceneSet, List<SceneDef> inputList) {

            for (int i = 0; i < inputList.Count; i++) {

                if (sceneSet.Contains(inputList[i])) {

                    sceneSet.Remove(inputList[i]);

                }
            
            }

            return sceneSet;
        
        }

        public void GenerateStageLists() {

            var startingStages = Run.instance.startingScenes;

            Stage1Set = MakeStartSet(Stage1Set, startingStages.ToList());

            Stage2Set = MakeSet(Stage2Set, Stage1Set.ToList());

            Stage3Set = MakeSet(Stage3Set, Stage2Set.ToList());

            Stage4Set = MakeSet(Stage4Set, Stage3Set.ToList());

            Stage5Set = MakeSet(Stage5Set, Stage4Set.ToList());

            Stage1Set = RemoveScenes(Stage1Set, StagesToRemove);

            Stage2Set = RemoveScenes(Stage2Set, StagesToRemove);

            Stage3Set = RemoveScenes(Stage3Set, StagesToRemove);

            Stage4Set = RemoveScenes(Stage4Set, StagesToRemove);

            Stage5Set = RemoveScenes(Stage5Set, StagesToRemove);

            StageCategories.Add(Stage1Set);
            StageCategories.Add(Stage2Set);
            StageCategories.Add(Stage3Set);
            StageCategories.Add(Stage4Set);
            StageCategories.Add(Stage5Set);

            CompareList.Add("blackbeach");
            CompareList.Add("goolake");
            CompareList.Add("frozenwall");
            CompareList.Add("rootjungle");
            CompareList.Add("skymeadow");
            
        }

        public void Awake()
        {

            Logger.LogMessage("Loaded Stage Remover");
            Logger.LogMessage("Loading Stage Remover Configs");
            RunConfig();
            Logger.LogMessage("Running Hooks...");
            Hooks();
            Logger.LogMessage("Hooks Loaded");

        }

        public void Hooks()
        {
            On.RoR2.Run.Start += (orig, self) => {

                Logger.LogMessage("Getting Stages to Remove");
                DefineStagesToRemove();

                orig(self);

            };

            On.RoR2.Stage.Start += (orig, self) => {

                CurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
                orig(self);
            
            };

            On.RoR2.Run.PickNextStageScene += PickValidScene;

        }

        private bool Check(HashSet<SceneDef> setToCheck, string groupIdentifier, out SceneDef[] choices, SceneDef currentScene) {

            choices = Array.Empty<SceneDef>();

            if (currentScene.destinations.Contains(SceneCatalog.GetSceneDefFromSceneName(groupIdentifier))) {

                if (setToCheck.Any())
                {

                    choices = setToCheck.ToArray();

                    return true;

                }

            }

            else {

                choices = Array.Empty<SceneDef>();
                return false;

            }

            return false;

        }

        private void PickValidScene(On.RoR2.Run.orig_PickNextStageScene orig, Run self, SceneDef[] choices)
        {

            if (Run.instance.stageClearCount > 0) {

                foreach (SceneDef scene in CurrentScene.destinations)
                {

                    if (StagesToRemove.Contains(scene))
                    {

                        foreach (SceneDef destinations in scene.destinations)
                        {

                            FailsafeSet.Add(destinations);

                        }

                        RemoveScenes(FailsafeSet, StagesToRemove);

                        for (int i = 0; i < StageCategories.Count; i++)
                        {

                            if (Check(StageCategories[i], CompareList[i], out choices, CurrentScene))
                            {

                                break;

                            }

                        }

                        if (!choices.Any())
                        {

                            if (!FailsafeSet.Any())
                            {

                                Logger.LogError("ERROR: NO VALID SCENE, USING DEFAULT DESTINATIONS");
                                choices = CurrentScene.destinations;

                            }

                            else
                            {

                                choices = FailsafeSet.ToArray();

                            }

                        }

                        FailsafeSet.Clear();

                    }

                }

            }

            FailsafeSet.Clear();

            orig(self, choices);

        }

    }

}
