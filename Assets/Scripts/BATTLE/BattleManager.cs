using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // VARIABLES
    internal GameManager GM;

    public List<Transform> _HeroSpawns;    // Where do they initially spawn?
    public List<Transform> _EnemySpawns;

    private List<GameObject> _HeroModels;  // The character models in scene
    private List<GameObject> _EnemyModels;

    private List<HeroExtension> _HeroesActive; // Who are currently in battle and can take actions
    private List<EnemyExtension> _EnemiesActive;

    private List<HeroExtension> _HeroesDowned; // Who are currently in battle but are K.O'd
    private List<EnemyExtension> _EnemiesDowned;

    // UPDATES
    private void Awake()
    {
        GM = FindObjectOfType<GameManager>();

        _HeroModels = new List<GameObject>();
        _EnemyModels = new List<GameObject>();
        _HeroesActive = new List<HeroExtension>();
        _EnemiesActive = new List<EnemyExtension>();
        _HeroesDowned = new List<HeroExtension>();
        _EnemiesDowned = new List<EnemyExtension>();
    }
    private void Start()
    {
        SpawnCharacterModels();
    }
    
    // METHODS
    private void SpawnCharacterModels() // Spawn models into game, add heroes into active or downed list for battle.
    {
        AddHeroIntoBattle();
        AddEnemyIntoBattle();
    }
    private void AddHeroIntoBattle()
    {
        for (int i = 0; i < GM._PartyLineup.Count; i++)
        {
            GameObject instantiatedHero = Instantiate((GM._PartyLineup[i]._CharacterModel) as GameObject,
                _HeroSpawns[i].position,
                _HeroSpawns[i].rotation);
            _HeroModels.Add(instantiatedHero);

            if (GM._PartyLineup[i]._CurrentHP > 0)
            {
                _HeroesActive.Add(GM._PartyLineup[i]);
            }
            else if (GM._PartyLineup[i]._CurrentHP <= 0)
            {
                _HeroesDowned.Add(GM._PartyLineup[i]);
            }
        }
    }
    private void AddEnemyIntoBattle()
    {
        for (int i = 0; i < GM._EnemyLineup.Count; i++)
        {
            GameObject instantiatedEnemy = Instantiate((GM._EnemyLineup[i]._CharacterModel) as GameObject,
                _EnemySpawns[i].position,
                _EnemySpawns[i].rotation);
            _EnemyModels.Add(instantiatedEnemy);

            if (GM._EnemyLineup[i]._CurrentHP > 0)
            {
                _EnemiesActive.Add(GM._EnemyLineup[i]);
            }
            else if (GM._EnemyLineup[i]._CurrentHP == 0)
            {
                _EnemiesDowned.Add(GM._EnemyLineup[i]);
            }
        }
    }
}