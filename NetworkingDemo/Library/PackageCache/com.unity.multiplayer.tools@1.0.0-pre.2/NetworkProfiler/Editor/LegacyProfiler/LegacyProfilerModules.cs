#if !UNITY_2021_2_OR_NEWER
#define NETWORK_PROFILER_SUPPORT_LEGACY
#endif

#if ENABLE_PROFILER
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    [InitializeOnLoad]
    static class LegacyProfilerModules
    {
#pragma warning disable IDE1006 // disable naming rule violation check
        [Serializable]
        internal class NetcodeProfilerCounter
        {
            // Note: These fields are named this way for internal serialization aligned w/ Unity profiler metadata
            public string m_Name;
            public string m_Category;
        }

        [Serializable]
        internal class NetcodeProfilerModuleData
        {
            // Note: These fields are named this way for internal serialization aligned w/ Unity profiler metadata
            public List<NetcodeProfilerCounter> m_ChartCounters = new List<NetcodeProfilerCounter>();
            public List<NetcodeProfilerCounter> m_DetailCounters = new List<NetcodeProfilerCounter>();
            public string m_Name;
        }
        
        [Serializable]
        internal class NetcodeModules
        {
            // Note: These fields are named this way for internal serialization aligned w/ Unity profiler metadata
            public List<NetcodeProfilerModuleData> m_Modules;
        }
#pragma warning restore IDE1006 // restore naming rule violation check

        static List<string> LegacyModuleNames = new List<string>
        {
// Profiler V1 naming within MLAPI            
#if UNITY_2020_3_OR_NEWER
            "MLAPI RPCs",
            "MLAPI Operations",
            "MLAPI Messages",
#endif
        };

        const string LegacyDynamicModulesName = "ProfilerWindow.DynamicModules";

        static bool ShouldAddLegacyModules()
        {
#if !NETWORK_PROFILER_SUPPORT_LEGACY
            return false;
#else
            var legacyModules = GetLegacyModules();

            bool hasCorrectLegacyModules = 
                legacyModules.Count == ProfilerModuleDefinitions.Modules.Count
                && ProfilerModuleDefinitions.Modules.All(
                    module =>
                        legacyModules.Count(legacyModule => 
                            legacyModule.m_Name.Equals(module.Name)) == 1);
            return !hasCorrectLegacyModules;
#endif
        }

        static bool ShouldCleanAllLegacyModule()
        {
#if !NETWORK_PROFILER_SUPPORT_LEGACY
            return true;
#else
            return false;
#endif
        }

        static List<NetcodeProfilerModuleData> GetLegacyModules()
        {
            var existingLegacyModules = new List<NetcodeProfilerModuleData>();
            
            var dynamicModules = GetDynamicModules();

            if (dynamicModules != null)
            {
                var legacyModules = dynamicModules.m_Modules;
                
                existingLegacyModules = legacyModules.FindAll(x => LegacyModuleNames.Contains(x.m_Name));
            }

            return existingLegacyModules;
        }

        static void CleanLegacyModules(bool keepCurrentLegacyModules)
        {
            var dynamicModules = GetDynamicModules();

            if (dynamicModules != null)
            {
                var modules = dynamicModules.m_Modules;
                bool cleanedSome = false;

                cleanedSome |= 0 < modules.RemoveAll(module => LegacyModuleNames.Contains(module.m_Name));

                if (!keepCurrentLegacyModules)
                {
                    cleanedSome |= 0 < modules.RemoveAll(
                        legacyModule => ProfilerModuleDefinitions.Modules.Any(
                            module => module.Name.Equals(legacyModule.m_Name)));
                }

                if (cleanedSome)
                {
                    SaveDynamicModules(dynamicModules);
                }
            }
        }
        
        static LegacyProfilerModules()
        {
            // We are using legacy modules, but it doesn't exist in the list
            if (ShouldAddLegacyModules())
            {
                // Clean existing legacy modules and updating the latest one if needed
                CleanLegacyModules(false);
                EnsureProfilerModulesExistInProfilerConfiguration();
            }
            
            // Newest version - delete everything but Extensibility profilers
            else if(ShouldCleanAllLegacyModule())
            {
                // We're removing all legacy modules
                CleanLegacyModules(false);
            }
            
            // We are using legacy modules, and it exists in the list, so we just
            // need to remove old ones
            else
            {
                // Last Legacy Module exist, we want to keep it
                CleanLegacyModules(true);
            }
        }
        
        static void EnsureProfilerModulesExistInProfilerConfiguration()
        {
            var dynamicModules = GetDynamicModules();

            if (dynamicModules != null)
            {
                var modules = dynamicModules.m_Modules;
                bool anyChanges = false;

                foreach (var module in ProfilerModuleDefinitions.Modules)
                {
                    anyChanges |= TryAddModule(modules, module.Name, module.Counters);
                }

                if (anyChanges)
                {
                    SaveDynamicModules(dynamicModules);
                }
            }
        }
        
        static bool TryAddModule(List<NetcodeProfilerModuleData> modules, string moduleName, string[] counterNames)
        {
            if (!modules.Any(x => x.m_Name == moduleName))
            {
                var counters = counterNames.Select(name => new NetcodeProfilerCounter
                {
                    m_Name = name,
                    m_Category = ProfilerCategory.Scripts.Name
                }).ToList();
                
                var newModule = new NetcodeProfilerModuleData
                {
                    m_Name = moduleName,
                    m_ChartCounters = counters,
                    m_DetailCounters = counters,
                };
                modules.Add(newModule);

                return true;
            }

            return false;
        }

        static NetcodeModules GetDynamicModules()
        {
            var dynamicModulesJson = EditorPrefs.GetString(LegacyDynamicModulesName);
            return JsonUtility.FromJson<NetcodeModules>(dynamicModulesJson);
        }

        static void SaveDynamicModules(NetcodeModules dynamicModules)
        {
            EditorPrefs.SetString(LegacyDynamicModulesName, JsonUtility.ToJson(dynamicModules));
        }
    }
}
#endif