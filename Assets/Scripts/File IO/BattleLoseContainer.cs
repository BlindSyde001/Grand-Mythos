using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleLoseContainer : MonoBehaviour
{
    // VARIABLES
    private InventoryManager inventoryManager;
    private GameManager gameManager;

    public GameObject LoadList;
    public List<SaveFileButton> SavedFiles;

    // UPDATES
    private void Start()
    {
        inventoryManager = InventoryManager._instance;
        gameManager = GameManager._instance;
    }

    // METHODS

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenLoadFiles()
    {
        string[] readFiles = GetFileNames(Application.persistentDataPath + "/Save Files", "*.json");
        // Interpret the Data on the read files
        for (int i = 0; i < readFiles.Length; i++)
        {
            // Open data packet from files
            SaveData SD = SaveManager.LoadFromFile(i);

            // Set the Lineup of heroes
            if (SD != null)
            {
                List<HeroExtension> tempHero = new();
                for (int j = 0; j < SD.lineupSave.Count; j++)
                {
                    tempHero.Add(gameManager._AllPartyMembers[SD.lineupSave[j]]);
                }

                // foreach party member, display their icon in order
                for (int k = 0; k < SD.lineupSave.Count; k++)
                {
                    SavedFiles[i].characterPortraits[k].sprite = tempHero[k].charPortrait;
                }
            }
            // Display name of the file
            SavedFiles[i].fileName.text = "Saved Game " + i;

            int hr = SD.playTimeData / 3600 % 24;
            int min = SD.playTimeData / 60 % 60;
            int sec = SD.playTimeData % 60;
            SavedFiles[i].timePlayed.text = ((hr < 10) ? ("0" + hr) : hr) + ":" +
                                            ((min < 10) ? ("0" + min) : min) + ":" +
                                            ((sec < 10) ? ("0" + sec) : sec);
            SavedFiles[i].moneyAcquired.text = SD.inventorySaveData.CreditsAmountData.ToString() + " Credits";
        }
    }
    private string[] GetFileNames(string path, string filter)
    {
        string[] files = Directory.GetFiles(path, filter);
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = Path.GetFileName(files[i]);
        }
        return files;
    }
    public void LoadGameData(int FileNumber)
    {
        var SD = SaveManager.LoadFromFile(FileNumber);

        #region Positional Data
        gameManager._LastKnownScene = SD.savedScene;
        gameManager._LastKnownPosition = SD.overworldPos;
        gameManager._LastKnownRotation = SD.overworldRot;
        #endregion
        #region Lineup Data
        gameManager._PartyLineup.Clear();
        BattleStateMachine._HeroesActive.Clear();
        BattleStateMachine._HeroesDowned.Clear();
        for (int i = 0; i < SD.lineupSave.Count; i++)
        {
            gameManager._PartyLineup.Add(gameManager._AllPartyMembers[SD.lineupSave[i]]);
        }
        #endregion
        #region Hero Data
        for (int i = 0; i < gameManager._AllPartyMembers.Count; i++)
        {
            gameManager._AllPartyMembers[i]._TotalExperience = SD.heroSaveData[i].totalExperienceSave;
            gameManager._AllPartyMembers[i].InitializeLevel();
            SaveManager.LoadWeaponData(SD, i);
            SaveManager.LoadArmourData(SD, i);
            SaveManager.LoadAccessoryData(SD, i);

            for (int j = 0; j < gameManager._AllPartyMembers[i].myTacticController._TacticsList.Count; j++)
            {
                gameManager._AllPartyMembers[i].myTacticController._TacticsList[j].isTurnedOn = SD.heroTacticData[i].tacticToggleList[j];

                if (SD.heroTacticData[i].tacticCndList[j] != "")
                {
                    gameManager._AllPartyMembers[i].myTacticController._TacticsList[j]._Condition = gameManager._ConditionsDatabase.Find(x => x.name == SD.heroTacticData[i].tacticCndList[j]);
                }
                if (SD.heroTacticData[i].tacticActionList[j] != "")
                {
                    if (SD.heroTacticData[i].tacticActionTypeList[j] == "SKILL")
                    {
                        if (SD.heroTacticData[i].tacticActionList[j] == "Attack")
                        {
                            gameManager._AllPartyMembers[i].myTacticController._TacticsList[j]._Action = gameManager._AllPartyMembers[i]._BasicAttack;
                        }
                        else
                        {
                            gameManager._AllPartyMembers[i].myTacticController._TacticsList[j]._Action = gameManager._HeroSkillsDatabase.Find(x => x._Name == SD.heroTacticData[i].tacticActionList[j]);
                        }
                    }
                    else if (SD.heroTacticData[i].tacticActionTypeList[j] == "ITEM")
                    {
                        gameManager._AllPartyMembers[i].myTacticController._TacticsList[j]._Action = gameManager._ItemSkillsDatabase.Find(x => x._Name == SD.heroTacticData[i].tacticActionList[j]);
                    }
                }
            }
        }

        for (int i = 0; i < gameManager._PartyLineup.Count; i++)
        {
            gameManager._PartyLineup[i]._CurrentHP = SD.heroSaveData[i].currentHPData;
            gameManager._PartyLineup[i]._CurrentMP = SD.heroSaveData[i].currentMPData;
        }
        #endregion
        #region Inventory Data
        inventoryManager.ConsumablesInBag.Clear();
        inventoryManager.EquipmentInBag.Clear();
        inventoryManager.LootInBag.Clear();
        inventoryManager.KeyItemsInBag.Clear();
        inventoryManager.ConditionsAcquired.Clear();

        for (int i = 0; i < SD.inventorySaveData.ConsumablesIdData.Count; i++)
        {
            Consumable newCon = gameManager._ConsumablesDatabase.Find(x => x._ItemID == SD.inventorySaveData.ConsumablesIdData[i]);
            newCon._ItemAmount = SD.inventorySaveData.ConsumablesAmountData[i];
            inventoryManager.ConsumablesInBag.Add(newCon);
        }
        for (int i = 0; i < SD.inventorySaveData.EquipmentNameData.Count; i++)
        {
            Equipment newEquip;
            switch (SD.inventorySaveData.EquipmentNameData[i])
            {
                case string a when a.Contains("Gun"):
                    newEquip = gameManager._GunsDatabase.Find(x => x._ItemID == SD.inventorySaveData.EquipmentIdData[i]);
                    newEquip._ItemAmount = SD.inventorySaveData.EquipmentAmountData[i];
                    inventoryManager.EquipmentInBag.Add(newEquip);
                    break;
                case string a when a.Contains("Warhammer"):
                    newEquip = gameManager._WarhammersDatabase.Find(x => x._ItemID == SD.inventorySaveData.EquipmentIdData[i]);
                    newEquip._ItemAmount = SD.inventorySaveData.EquipmentAmountData[i];
                    inventoryManager.EquipmentInBag.Add(newEquip);
                    break;
                case string a when a.Contains("Power Glove"):
                    newEquip = gameManager._PowerGlovesDatabase.Find(x => x._ItemID == SD.inventorySaveData.EquipmentIdData[i]);
                    newEquip._ItemAmount = SD.inventorySaveData.EquipmentAmountData[i];
                    inventoryManager.EquipmentInBag.Add(newEquip);
                    break;
                case string a when a.Contains("Grimoire"):
                    newEquip = gameManager._GrimoiresDatabase.Find(x => x._ItemID == SD.inventorySaveData.EquipmentIdData[i]);
                    newEquip._ItemAmount = SD.inventorySaveData.EquipmentAmountData[i];
                    inventoryManager.EquipmentInBag.Add(newEquip);
                    break;
                case string a when a.Contains("Leather"):
                    newEquip = gameManager._LeatherDatabase.Find(x => x._ItemID == SD.inventorySaveData.EquipmentIdData[i]);
                    newEquip._ItemAmount = SD.inventorySaveData.EquipmentAmountData[i];
                    inventoryManager.EquipmentInBag.Add(newEquip);
                    break;
                case string a when a.Contains("Mail"):
                    newEquip = gameManager._MailDatabase.Find(x => x._ItemID == SD.inventorySaveData.EquipmentIdData[i]);
                    newEquip._ItemAmount = SD.inventorySaveData.EquipmentAmountData[i];
                    inventoryManager.EquipmentInBag.Add(newEquip);
                    break;
                case string a when a.Contains("Chasis"):
                    newEquip = gameManager._ChasisDatabase.Find(x => x._ItemID == SD.inventorySaveData.EquipmentIdData[i]);
                    newEquip._ItemAmount = SD.inventorySaveData.EquipmentAmountData[i];
                    inventoryManager.EquipmentInBag.Add(newEquip);
                    break;
                case string a when a.Contains("Robe"):
                    newEquip = gameManager._RobesDatabase.Find(x => x._ItemID == SD.inventorySaveData.EquipmentIdData[i]);
                    newEquip._ItemAmount = SD.inventorySaveData.EquipmentAmountData[i];
                    inventoryManager.EquipmentInBag.Add(newEquip);
                    break;
                case string a when a.Contains("Accessory"):
                    newEquip = gameManager._AccessoryDatabase.Find(x => x._ItemID == SD.inventorySaveData.EquipmentIdData[i]);
                    newEquip._ItemAmount = SD.inventorySaveData.EquipmentAmountData[i];
                    inventoryManager.EquipmentInBag.Add(newEquip);
                    break;
            }
        }
        for (int i = 0; i < SD.inventorySaveData.LootIdData.Count; i++)
        {
            Loot newLoot = gameManager._LootDatabase.Find(x => x._ItemID == SD.inventorySaveData.KeyItemsIdData[i]);
            newLoot._ItemAmount = SD.inventorySaveData.LootAmountData[i];
            inventoryManager.LootInBag.Add(newLoot);
        }
        for (int i = 0; i < SD.inventorySaveData.KeyItemsIdData.Count; i++)
        {
            KeyItem newKey = gameManager._KeyItemsDatabase.Find(x => x._ItemID == SD.inventorySaveData.KeyItemsIdData[i]);
            inventoryManager.KeyItemsInBag.Add(newKey);
        }
        for (int i = 0; i < SD.inventorySaveData.ConditionsIdData.Count; i++)
        {
            Condition newCond = gameManager._ConditionsDatabase.Find(x => x.cndID == SD.inventorySaveData.ConditionsIdData[i]);
            inventoryManager.ConditionsAcquired.Add(newCond);
        }
        inventoryManager.creditsInBag = SD.inventorySaveData.CreditsAmountData;
        #endregion
        #region Time Data
        FindObjectOfType<InGameClock>().playTime = SD.playTimeData;
        #endregion
        LoadGameScene(gameManager._LastKnownScene);
    }
    private void LoadGameScene(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad);
    }
}
