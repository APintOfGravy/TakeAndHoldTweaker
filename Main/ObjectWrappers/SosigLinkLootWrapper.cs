﻿using FistVR;
// using MagazinePatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNHFramework.ObjectTemplates;
using TNHFramework.Utilities;
using UnityEngine;

namespace TNHFramework
{
    public class SosigLinkLootWrapper : MonoBehaviour
    {
        public EquipmentGroup group;
        public bool dontDrop = false;
        public bool shouldDropOnCleanup;

        void OnDestroy()
        {
            if (dontDrop)
                return;
            
            TNHTweakerLogger.Log("TNHTweaker -- Lootable link was destroyed!", TNHTweakerLogger.LogType.TNH);

            List<EquipmentGroup> selectedGroups = group.GetSpawnedEquipmentGroups();
            string selectedItem;
            int spawnedItems = 0;

            foreach (EquipmentGroup selectedGroup in selectedGroups)
            {
                for (int itemIndex = 0; itemIndex < selectedGroup.ItemsToSpawn; itemIndex++)
                {
                    if (selectedGroup.IsCompatibleMagazine)
                    {
                        FVRObject mag = FirearmUtils.GetAmmoContainerForEquipped(selectedGroup.MinAmmoCapacity, selectedGroup.MaxAmmoCapacity);
                        if (mag != null)
                        {
                            selectedItem = mag.ItemID;
                        }
                        else
                        {
                            TNHTweakerLogger.Log(
                                "TNHTweaker -- Spawning nothing, since group was compatible magazines, and could not find a compatible magazine for player",
                                TNHTweakerLogger.LogType.TNH);
                            return;
                        }
                    }

                    else
                    {
                        selectedItem = selectedGroup.GetObjects().GetRandom();
                    }

                    if (LoadedTemplateManager.LoadedVaultFiles.ContainsKey(selectedItem))
                    {
                        Transform newTransform = transform;
                        newTransform.position = transform.position + (Vector3.up * 0.1f * spawnedItems);
                        VaultSystem.SpawnVaultFile(LoadedTemplateManager.LoadedVaultFiles[selectedItem], newTransform, true, false, false, out _, Vector3.zero);
                    }
                    else if (LoadedTemplateManager.LoadedLegacyVaultFiles.ContainsKey(selectedItem))
                    {
                        AnvilManager.Run(TNHFrameworkUtils.SpawnFirearm(LoadedTemplateManager.LoadedLegacyVaultFiles[selectedItem],
                            transform.position + (Vector3.up * 0.1f * spawnedItems), transform.rotation));
                    }
                    else
                    {
                        Instantiate(IM.OD[selectedItem].GetGameObject(), transform.position + (Vector3.up * 0.1f * spawnedItems), transform.rotation);
                    }

                    spawnedItems += 1;
                }
            }
        }
    }
}