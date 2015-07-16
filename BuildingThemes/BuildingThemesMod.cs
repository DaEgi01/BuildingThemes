﻿using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using System;
using BuildingThemes.GUI;

namespace BuildingThemes
{
    public class BuildingThemesMod : LoadingExtensionBase, IUserMod
    {
        public string Name
        {
            get { return "Building Themes"; }
        }
        public string Description
        {
            get { return "Create building themes and apply them to cities and districts."; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup("Building Themes");
            group.AddCheckbox("Unlock Policies Panel From Start", PolicyPanelEnabler.Unlock, delegate(bool c) { PolicyPanelEnabler.Unlock = c; });
            group.AddCheckbox("Generate Debug Output", Debugger.Enabled, delegate(bool c) { Debugger.Enabled = c; });
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            Debugger.Initialize();

            Debugger.Log("ON_CREATED");
            Debugger.Log("Building Themes: Initializing Mod...");

            PolicyPanelEnabler.Register();

            Singleton<BuildingThemesManager>.instance.Reset();
            Singleton<BuildingThemesManager>.instance.searchBuildingThemeMods();

            try { Detour.BuildingManagerDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.ZoneBlockDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.ImmaterialResourceManagerDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PrivateBuildingAIDetour<ResidentialBuildingAI>.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PrivateBuildingAIDetour<CommercialBuildingAI>.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PrivateBuildingAIDetour<IndustrialBuildingAI>.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PrivateBuildingAIDetour<OfficeBuildingAI>.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PoliciesPanelDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            Debugger.Log("Building Themes: Mod successfully intialized.");
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            Debugger.Log("ON_LEVEL_LOADED");

            Debugger.AppendModList();
            Debugger.AppendThemeList();

            PolicyPanelEnabler.UnlockPolicyToolbarButton();
            UIThemeManager.Initialize();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            Debugger.Log("ON_LEVEL_UNLOADING");

            UIThemeManager.Destroy();
        }

        public override void OnReleased()
        {
            base.OnReleased();

            Debugger.Log("ON_RELEASED");

            Debugger.Log("Building Themes: Reverting detoured methods...");

            Singleton<BuildingThemesManager>.instance.Reset();

            try
            {
                Detour.BuildingManagerDetour.Revert();
                Detour.ZoneBlockDetour.Revert();
                Detour.ImmaterialResourceManagerDetour.Revert();
                Detour.PrivateBuildingAIDetour<ResidentialBuildingAI>.Revert();
                Detour.PrivateBuildingAIDetour<CommercialBuildingAI>.Revert();
                Detour.PrivateBuildingAIDetour<IndustrialBuildingAI>.Revert();
                Detour.PrivateBuildingAIDetour<OfficeBuildingAI>.Revert();
                Detour.PoliciesPanelDetour.Revert();
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
            }

            PolicyPanelEnabler.Unregister();

            Debugger.Log("Building Themes: Done!");

            Debugger.Deinitialize();
        }
    }
}
