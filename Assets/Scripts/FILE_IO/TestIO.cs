using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestIO : MonoBehaviour
{
    public void ClickSave()
    {
        GameManager GM = FindObjectOfType<GameManager>();
        GM._LastKnownScene = SceneManager.GetActiveScene().name;
        GM._LastKnownPosition = FindObjectOfType<OverworldPlayerCircuit>().transform.position;
        GM._LastKnownRotation = FindObjectOfType<OverworldPlayerCircuit>().transform.rotation;

        SaveData.current.scene = GM._LastKnownScene;

        SaveData.current.overworldPos = GM._LastKnownPosition;
        SaveData.current.overworldRot = GM._LastKnownRotation;

        SaveData.current.allPartyMembersSave = GM._AllPartyMembers;
        SaveData.current.partyLineupSave = GM._PartyLineup;

        foreach(HeroExtension hero in GM._AllPartyMembers)
        {
            SerializableHero heroSave = new SerializableHero
            {
                totalExperienceSave = hero._TotalExperience,

                weaponIDSave = hero._Weapon._ItemID,
                armourIDSave = hero._Armour._ItemID,
                accessoryOneIDSave = hero._AccessoryOne._ItemID,
                accessoryTwoIDSave = hero._AccessoryTwo._ItemID,

                weaponSave = hero._Weapon,
                armourSave = hero._Armour,
                accessoryOneSave = hero._AccessoryOne,
                accessoryTwoSave = hero._AccessoryTwo
            };

            SaveData.current.heroSaveData.Add(heroSave);
        }
        SaveManager.SaveToFile(SaveData.current, "testSave");
        Debug.Log("Saved");
    }
    public void ClickLoad(string fileName)
    {
       var SD = SaveManager.LoadFromFile(fileName);

       var GM = FindObjectOfType<GameManager>();

        GM._LastKnownScene = SD.scene;
        GM._LastKnownPosition = SD.overworldPos;
        GM._LastKnownRotation = SD.overworldRot;

        GM._AllPartyMembers = SD.allPartyMembersSave;
        GM._PartyLineup = SD.partyLineupSave;

        for(int i = 0; i < GM._AllPartyMembers.Count; i++)
        {
            GM._AllPartyMembers[i]._TotalExperience = SD.heroSaveData[i].totalExperienceSave;
            ExtractWeaponData(SD, GM, i);
            ExtractArmourData(SD, GM, i);
            ExtractAccessoryData(SD, GM, i);
        }
    }

    private static void ExtractWeaponData(SaveData SD, GameManager GM, int i)
    {
        if (SD.heroSaveData[i].weaponSave != null)
        {
            switch (SD.heroSaveData[i].weaponSave.weaponType)
            {
                case Weapon.WeaponType.Gun:
                    try
                    {
                        GM._AllPartyMembers[i]._Weapon = GM._GunsDatabase.Find(x => x._ItemID == SD.heroSaveData[i].weaponIDSave);
                    }
                    catch
                    {
                        Debug.LogError("Cannot find Gun in the Database!");
                    }
                    break;
                case Weapon.WeaponType.Warhammer:
                    try
                    {
                        GM._AllPartyMembers[i]._Weapon = GM._WarhammersDatabase.Find(x => x._ItemID == SD.heroSaveData[i].weaponIDSave);
                    }
                    catch
                    {
                        Debug.LogError("Cannot find Warhammer in the Database!");
                    }
                    break;
                case Weapon.WeaponType.PowerGlove:
                    try
                    {
                        GM._AllPartyMembers[i]._Weapon = GM._PowerGlovesDatabase.Find(x => x._ItemID == SD.heroSaveData[i].weaponIDSave);
                    }
                    catch
                    {
                        Debug.LogError("Cannot find Power Glove in the Database!");
                    }
                    break;
                case Weapon.WeaponType.Grimoire:
                    try
                    {
                        GM._AllPartyMembers[i]._Weapon = GM._GrimoiresDatabase.Find(x => x._ItemID == SD.heroSaveData[i].weaponIDSave);
                    }
                    catch
                    {
                        Debug.LogError("Cannot find Grimoire in the Database!");
                    }
                    break;
            }
        }
        else
        {
            Debug.LogError("There is no Weapon equipped! Character always must have a Weapon");
        }
    }
    private static void ExtractArmourData(SaveData SD, GameManager GM, int i)
    {
        if (SD.heroSaveData[i].armourSave != null)
        {
            switch (SD.heroSaveData[i].armourSave.armourType)
            {
                case Armour.ArmourType.Leather:
                    try
                    {
                        GM._AllPartyMembers[i]._Armour = GM._LeatherDatabase.Find(x => x._ItemID == SD.heroSaveData[i].armourIDSave);
                    }
                    catch
                    {
                        Debug.LogError("Cannot find Leather in the Database!");
                    }
                    break;
                case Armour.ArmourType.Mail:
                    try
                    {
                        GM._AllPartyMembers[i]._Armour = GM._MailDatabase.Find(x => x._ItemID == SD.heroSaveData[i].armourIDSave);
                    }
                    catch
                    {
                        Debug.LogError("Cannot find Mail in the Database!");
                    }
                    break;
                case Armour.ArmourType.Chasis:
                    try
                    {
                        GM._AllPartyMembers[i]._Armour = GM._ChasisDatabase.Find(x => x._ItemID == SD.heroSaveData[i].armourIDSave);
                    }
                    catch
                    {
                        Debug.LogError("Cannot find Chasis in the Database!");
                    }
                    break;
                case Armour.ArmourType.Robes:
                    try
                    {
                        GM._AllPartyMembers[i]._Armour = GM._RobesDatabase.Find(x => x._ItemID == SD.heroSaveData[i].armourIDSave);
                    }
                    catch
                    {
                        Debug.LogError("Cannot find Robe in the Database!");
                    }
                    break;
            }
        }
        else
        {
            Debug.LogError("There is no Armour equipped! Character always must have a Weapon");
        }
    }
    private static void ExtractAccessoryData(SaveData SD, GameManager GM, int i)
    {
        if (SD.heroSaveData[i].accessoryOneSave != null)
        {
            try
            {
                GM._AllPartyMembers[i]._AccessoryOne = GM._AccessoryDatabase.Find(x => x._ItemID == SD.heroSaveData[i].accessoryOneIDSave);
            }
            catch
            {
                Debug.LogError("Cannot find Accessory for Acc1 in the Database!");
            }
        }
        if(SD.heroSaveData[i].accessoryTwoSave != null)
        {
            try
            {
                GM._AllPartyMembers[i]._AccessoryTwo = GM._AccessoryDatabase.Find(x => x._ItemID == SD.heroSaveData[i].accessoryTwoIDSave);
            }
            catch
            {
                Debug.LogError("Cannot find Accessory for Acc2 in the Database!");
            }
        }
    }
}