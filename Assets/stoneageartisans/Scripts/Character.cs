using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public int id;

    public Constants.Weapon weapon;
    public int agility;
    public int agilityMin;
    public int brains;
    public int brainsMin;
    public int brawn;
    public int brawnMin;
    public int stamina;
    public int staminaMin;
    public int unspentPoints;
    public int statMax;

    bool tokenPlaced = false;

    float initiative;
    int action;
    int actionPoints;
    int defense;
    int hitPoints;
    int wounds;
    Dictionary<Constants.Weapon, int> damageMax;
    Dictionary<Constants.Weapon, int> damageMin;

    public Character(int statDefaultValue, int statMinValue, int statMaxValue, int startingPoints, int id)
    {
        this.id = id;
        agility = statDefaultValue;
        agilityMin = statMinValue;
        brains = statDefaultValue;
        brainsMin = statMinValue;
        brawn = statDefaultValue;
        brawnMin = statMinValue;
        stamina = statDefaultValue;
        staminaMin = statMinValue;
        unspentPoints = startingPoints;
        statMax = statMaxValue;
        weapon = Constants.Weapon.Unarmed;
        wounds = 0;

        calculateStats();
    }

    public Character(Character character)
    {
        id = character.id;
        agility = character.agility;
        agilityMin = character.agilityMin;
        brains = character.brains;
        brainsMin = character.brainsMin;
        brawn = character.brawn;
        brawnMin = character.brawnMin;
        stamina = character.stamina;
        staminaMin = character.staminaMin;
        unspentPoints = character.unspentPoints;
        statMax = character.statMax;
        weapon = character.weapon;
        wounds = character.wounds;

        calculateStats();
    }

    public void calculateStats()
    {
        hitPoints = Mathf.RoundToInt((float) (brawn + stamina) / 2.0f);

        if(((float) agility / 2.0f) > ((float) (brawn + agility + brains + stamina) / 8.0f))
        {
            defense = Mathf.RoundToInt((float) agility / 2.0f);
        }
        else
        {
            defense = Mathf.RoundToInt((float) (brawn + agility + brains + stamina) / 8.0f);
        }

        initiative = ((float) (agility + brains + stamina) / 6.0f);

        action = Mathf.RoundToInt((float) (agility + brains + stamina) / 6.0f);
        actionPoints = action;

        damageMin = new Dictionary<Constants.Weapon, int>();

        if(brawn > 9)
        {
            damageMin.Add(Constants.Weapon.Unarmed, Mathf.RoundToInt((float) (brawn - 7) / 3.0f));
        }
        else
        {
            damageMin.Add(Constants.Weapon.Unarmed, 0);
        }

        //damageMin.Add(Constants.Weapon.Knife, damageMin[Constants.Weapon.Hands] + 1);
        //damageMin.Add(Constants.Weapon.Club, damageMin[Constants.Weapon.Hands] + 2);
        //damageMin.Add(Constants.Weapon.Spear, damageMin[Constants.Weapon.Hands] + 1);

        damageMax = new Dictionary<Constants.Weapon, int>();

        if(brawn > 7)
        {
            damageMax.Add(Constants.Weapon.Unarmed, Mathf.RoundToInt((float) (brawn - 5) / 2.0f));
        }
        else
        {
            if(brawn < 6)
            {
                damageMax.Add(Constants.Weapon.Unarmed, 0);
            }
            else
            {
                damageMax.Add(Constants.Weapon.Unarmed, 1);
            }
        }

        //damageMax.Add(Constants.Weapon.Knife, damageMax[Constants.Weapon.Hands] + 2);
        //damageMax.Add(Constants.Weapon.Club, damageMax[Constants.Weapon.Hands] + 3);
        //damageMax.Add(Constants.Weapon.Spear, damageMax[Constants.Weapon.Hands] + 4);
    }

    public void resetActionPoints()
    {
        actionPoints = action;
    }

    public int getAction()
    {
        return action;
    }

    public int getActionPoints()
    {
        return actionPoints;
    }

    public float getInitiative()
    {
        return initiative;
    }

    public int getDamageMax()
    {
        return damageMax[weapon];
    }

    public int getDamageMin()
    {
        return damageMin[weapon];
    }

    public int getDefense()
    {
        return defense;
    }

    public int getHitPoints()
    {
        return hitPoints;
    }

    public int getWounds()
    {
        return wounds;
    }
}
