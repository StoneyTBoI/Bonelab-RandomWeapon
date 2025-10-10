using BoneLib;
using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Jevil;
using MelonLoader;
using UnityEngine;
using Page = BoneLib.BoneMenu.Page;
using Random = UnityEngine.Random;

[assembly: MelonInfo(typeof(Bonelab_RandomWeapon.Core), "Bonelab-RandomWeapon", "1.0.0", "Stoney", null)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]


// TODO: Comment, Add Crate Filtering, Add weights, Black/Whitelists, Cleanup... so much cleanup...

namespace Bonelab_RandomWeapon
{
    public class Core : MelonMod
    {
        public enum BindType { THUMBSTICK_PRESS, DOUBLE_TAP_B }

        private MelonPreferences_Category randomWeaponCategory;
        private MelonPreferences_Entry<bool> mod_EnabledPref;
        private MelonPreferences_Entry<BindType> bind_ModePref;
        private MelonPreferences_Entry<bool> debug_EnabledPref;

        private MelonPreferences_Entry<bool> firearm_Pistol;
        private MelonPreferences_Entry<bool> firearm_SMG;
        private MelonPreferences_Entry<bool> firearm_Rifle;
        private MelonPreferences_Entry<bool> firearm_Shotgun;
        private MelonPreferences_Entry<bool> firearm_Other;

        private MelonPreferences_Entry<bool> melee_Blade;
        private MelonPreferences_Entry<bool> melee_Blunt;
        private MelonPreferences_Entry<bool> melee_Other;

        // Bonemenu (totally not stolen from Lakatrazz' RagdollPlayer)
        private Page mainPage;
        private BoolElement enabledElement;
        private EnumElement bindElement;
        private BoolElement debugElement;

        private static List<SpawnableCrate> cached_Weapons = new();
        private static float last_Spawn_Time = 0f;
        private static float spawn_Cooldown = 0.5f;
        private static float last_Tap_Time = 0f;
        private static bool waiting_Second_Tap = false;
        private const float doubleTap_Delay = 0.32f;
        private static bool mod_Enabled = true;
        private static BindType bind_Mode = BindType.THUMBSTICK_PRESS;


        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("[Bonelab_RandomWeapon] Initialized");
            Setup_Prefs();
            Hooking.OnLevelLoaded += _ => Cache_All_Weapons();
            Create_Bone_Menu();
        }

        private void Setup_Prefs()
        {
            randomWeaponCategory = MelonPreferences.CreateCategory("RandomWeapon");

            mod_EnabledPref = randomWeaponCategory.CreateEntry("mod_Enabled", true);
            bind_ModePref = randomWeaponCategory.CreateEntry("bind_Mode", BindType.THUMBSTICK_PRESS);
            debug_EnabledPref = randomWeaponCategory.CreateEntry("debug_Enabled", false);

            firearm_Pistol = randomWeaponCategory.CreateEntry("firearm_Pistol", true);
            firearm_SMG = randomWeaponCategory.CreateEntry("firearm_SMG", true);
            firearm_Rifle = randomWeaponCategory.CreateEntry("firearm_Rifle", true);
            firearm_Shotgun = randomWeaponCategory.CreateEntry("firearm_Shotgun", true);
            firearm_Other = randomWeaponCategory.CreateEntry("firearm_Other", false);

            melee_Blade = randomWeaponCategory.CreateEntry("melee_Blade", true);
            melee_Blunt = randomWeaponCategory.CreateEntry("melee_Blunt", true);
            melee_Other = randomWeaponCategory.CreateEntry("melee_Other", false);

            mod_Enabled = mod_EnabledPref.Value;
            bind_Mode = bind_ModePref.Value;
        }

        private void Create_Bone_Menu()
        {
            mainPage = Page.Root.CreatePage("Random Weapon", Color.white);

            enabledElement = mainPage.CreateBool("Enabled", Color.yellow, mod_Enabled, (v) =>
            {
                mod_Enabled = v;
                mod_EnabledPref.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });

            bindElement = mainPage.CreateEnum("Binding", Color.cyan, bind_Mode, (System.Enum val) =>
            {
                bind_Mode = (BindType)val;
                bind_ModePref.Value = bind_Mode;
                randomWeaponCategory.SaveToFile(true);
            });

            debugElement = mainPage.CreateBool("Debug Logs", Color.grey, debug_EnabledPref.Value, (v) =>
            {
                debug_EnabledPref.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });

            mainPage.CreateFunction("Re-cache Weapons", Color.yellow, () =>
            {
                Cache_All_Weapons();
                LoggerInstance.Msg("[Bonelab_RandomWeapon] Weapons re-cached!");
            });

            var firearmPage = mainPage.CreatePage("Firearms", Color.red);
            firearmPage.CreateFunction("!!! Re-cache after selecting !!!", Color.red, () => { });

            firearmPage.CreateBool("Pistol", Color.red, firearm_Pistol.Value, (v) =>
            {
                firearm_Pistol.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });
            firearmPage.CreateBool("SMG", Color.red, firearm_SMG.Value, (v) =>
            {
                firearm_SMG.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });
            firearmPage.CreateBool("Rifle", Color.red, firearm_Rifle.Value, (v) =>
            {
                firearm_Rifle.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });
            firearmPage.CreateBool("Shotgun", Color.red, firearm_Shotgun.Value, (v) =>
            {
                firearm_Shotgun.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });
            firearmPage.CreateBool("Other", Color.red, firearm_Other.Value, (v) =>
            {
                firearm_Other.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });

            var meleePage = mainPage.CreatePage("Melee", Color.green);
            meleePage.CreateFunction("!!! Re-cache after selecting !!!", Color.red, () => { });

            meleePage.CreateBool("Blade", Color.green, melee_Blade.Value, (v) =>
            {
                melee_Blade.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });
            meleePage.CreateBool("Blunt", Color.green, melee_Blunt.Value, (v) =>
            {
                melee_Blunt.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });
            meleePage.CreateBool("Other", Color.green, melee_Other.Value, (v) =>
            {
                melee_Other.Value = v;
                randomWeaponCategory.SaveToFile(true);
            });

            mainPage.CreateFunction("Spawn Random Weapon", Color.white, () => Spawn_Random_Weapon());
        }

        private void Cache_All_Weapons()
        {
            cached_Weapons.Clear();
            var warehouse = AssetWarehouse.Instance;
            if (warehouse == null || !AssetWarehouse.ready)
            {
                LoggerInstance.Warning("[Bonelab_RandomWeapon] AssetWarehouse not ready.");
                return;
            }

            LoggerInstance.Msg("[Bonelab_RandomWeapon] Caching spawnable crates...");

            foreach (var crate in warehouse.GetCrates<SpawnableCrate>())
            {
                if (crate == null) continue;

                var tags = crate.Tags;

                bool gun = false;
                foreach (var t in tags)
                {
                    if ((t == "Pistol" && firearm_Pistol.Value) ||
                        (t == "SMG" && firearm_SMG.Value) ||
                        (t == "Rifle" && firearm_Rifle.Value) ||
                        (t == "Shotgun" && firearm_Shotgun.Value) ||
                        (t == "Other" && firearm_Other.Value))
                    {
                        gun = true;
                        break;
                    }
                }

                bool melee = false;
                foreach (var t in tags)
                {
                    if ((t == "Blade" && melee_Blade.Value) ||
                        (t == "Blunt" && melee_Blunt.Value) ||
                        (t == "Other" && melee_Other.Value))
                    {
                        melee = true;
                        break;
                    }
                }

                if (gun || melee)
                    cached_Weapons.Add(crate);
            }

            LoggerInstance.Msg($"[Bonelab_RandomWeapon] Cached {cached_Weapons.Count} crates.");
        }

        private void Spawn_Random_Weapon()
        {
            if (cached_Weapons.Count == 0) return;
            if (Time.time - last_Spawn_Time < spawn_Cooldown) return;
            last_Spawn_Time = Time.time;

            if (Player.Head == null)
            {
                if (debug_EnabledPref.Value) LoggerInstance.Warning("[Bonelab_RandomWeapon] Player.Head is null!");
                return;
            }

            var crate = cached_Weapons[Random.Range(0, cached_Weapons.Count)];
            if (crate == null)
            {
                if (debug_EnabledPref.Value) LoggerInstance.Warning("[Bonelab_RandomWeapon] Selected crate is null!");
                return;
            }

            Vector3 pos = Player.Head.position + Player.Head.forward * 1.5f + Vector3.up * 0.2f;
            Quaternion rot = Quaternion.identity;

            crate.Spawn(pos, rot);

            if (debug_EnabledPref.Value)
                LoggerInstance.Msg($"[Bonelab_RandomWeapon] Spawned crate: {crate.Barcode.ID}");
        }

        public override void OnUpdate()
        {
            if (!mod_Enabled) return;
            var controller = Player.RightController;
            if (controller == null) return;

            if (Get_Input(controller))
                Spawn_Random_Weapon();
        }

        private static bool Get_Input(BaseController controller)
        {
            switch (bind_Mode)
            {
                default:
                case BindType.THUMBSTICK_PRESS:
                    waiting_Second_Tap = false;
                    last_Tap_Time = 0f;
                    return controller.GetThumbStickDown();

                case BindType.DOUBLE_TAP_B:
                    bool down = controller.GetBButtonDown();
                    float t = Time.realtimeSinceStartup;

                    if (down && waiting_Second_Tap)
                    {
                        if (t - last_Tap_Time <= doubleTap_Delay)
                            return true;
                        waiting_Second_Tap = false;
                        last_Tap_Time = 0f;
                    }
                    else if (down)
                    {
                        last_Tap_Time = t;
                        waiting_Second_Tap = true;
                    }
                    else if (t - last_Tap_Time > doubleTap_Delay)
                    {
                        waiting_Second_Tap = false;
                        last_Tap_Time = 0f;
                    }
                    return false;
            }
        }

        public override void OnApplicationQuit()
        {
            MelonPreferences.Save();
        }
    }
}



//                      ░██                                                     
//                         ███                                                    
//                           ██                                                   
//                           ██                                                   
//                           ░██                                                  
//                         ▓█▒░░▒▓███                                             
//                       ░▒░  ▒░░░▒▒▓██                                           
//                       ▒▒   ░░░░░▒░▒█▓                                          
//                      ░▒░  ░░░░░▒▓▓▓▓█                                          
//                      ▒░░░░░▒▒▓▓▒▓▓▒▒██                                         
//                     ░░░░ ░░░░░▒▒▒▒▒▓▓█                                         
//                     ░░░░▒▒▒░▓▓▒▒▒▓▒▒▓█░                                        
//                    ░░░▒▒▒▒▒▒▒▓▒▒▒▒▒▒▓▓█░                                       
//                   ░░░▒▒▒▒▒▒▒▒▒▒▓▒▓▓▓▒▓██░                                      
//                ░▒████▓▓▒▒▓▓█▒▒▒▒░▒▒▓▒▒▒▓█                                      
//               ██▒  ▓█▓▒▒▓▒█░    ▒░▒▒▒▒▓▓▓▓                                     
//              ▓█▓██░  ████▓▒  ████▒░▒▒▒▒▒▓▓▓                                    
//              ██▓ ░██  ▓███ ░█▒ ███▒▓▒▒▓▒█▓▓▓                                   
//              ██ ████▒   ██  ▓███████▒▒▒▒▒▒▒▓                                   
//              █   ▒█      ▒    ██▓  ▒█▓▒▒▒█▓▒░                                  
//              █▓       ▒░           ░▒▒▓▓▓▒▒▒                                   
//               █ ░▒▒▒▒░▒░   █       ▒▒▒▒▒░▒▒▒▒                                   
//               ░▒  ░░░██████▓     ░▓▓▓▓░░▒▓█                                    
//                ▒██░     ░      ▒█▒▒▒░░░▒▒█                                     
//                  ▓██▓      ░ ░▒▒░░░▒▒▒▒▓█  ░▒░░░                               
//                   ░▓▓███▓▒▒░░▒░░░▒░░░▒█▒ ░▒▒▒▒░░▒▒▒▒▒░░░                       
//                      ░░░░░░░░░░░░░░▒▓▒░▒▒▒▒▒▒▒▒░░░░░░░░░░░░░░░░░░░             
//                       ███████████████▓▓▒▒▒▒▒▒▒▒▒░░░░░░░░░░░ ░ ░░░░░░           
//                        ▒███████████▓▓▓▓▓▓▒▒▒▒▒▒░░░░░░░ ░░  ░░░░░░              
//                             ▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒░░░░░░░░ ░░░░░░░░░
