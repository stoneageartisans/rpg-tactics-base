using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : Character
{
    public Constants.TacticalStance tacticalStance;

    public NonPlayerCharacter(int statDefaultValue, int statMinValue, int statMaxValue, int startingPoints, int id)
        : base(statDefaultValue, statMinValue, statMaxValue, startingPoints, id)
    {
        this.id = id;

        allotUnspentPoints();
        determineTacticalStance();
    }

    /* 
     * My eventual intent will be to make this tweakable in the editor. Currently the decision-engine
     * is weighted as follows:
     *     Raise stat is 3 of 6 (50%)
     *     Keep stat is 1 of 6  (17%)
     *     Lower stat is 2 of 6 (33%)
     */
    void allotUnspentPoints()
    {
        const int Brawn = 0;
        const int Agility = 1;
        const int Brains = 2;
        const int Stamina = 3;
        const int TotalStats = 4;

        const int Raise = 0;
        const int Keep = 1;
        const int Lower = 2;

        int currentStat = Brawn;

        while(unspentPoints > 0)
        {
            int choice;

            // Get random number from 1 to 6
            int n = Random.Range(1, 7);

            // Raise stat if 1, 2 or 3
            if(n > 0 && n < 4)
            {
                choice = Raise;
            }
            // Keep stat same if 4
            else if(n > 3 && n < 5)
            {
                choice = Keep;
            }
            // Lower stat if 5 or 6
            else
            {
                choice = Lower;
            }

            switch(choice)
            {
                case Raise:
                    switch(currentStat)
                    {
                        case Brawn:
                            increaseStat(ref brawn);
                            break;
                        case Agility:
                            increaseStat(ref agility);
                            break;
                        case Brains:
                            increaseStat(ref brains);
                            break;
                        case Stamina:
                            increaseStat(ref stamina);
                            break;
                    }
                    break;
                case Keep:
                    // Do nothing
                    break;
                case Lower:
                    switch(currentStat)
                    {
                        case Brawn:
                            decreaseStat(ref brawn);
                            break;
                        case Agility:
                            decreaseStat(ref agility);
                            break;
                        case Brains:
                            decreaseStat(ref brains);
                            break;
                        case Stamina:
                            decreaseStat(ref stamina);
                            break;
                    }
                    break;
            }

            // Move to the next stat
            currentStat ++;

            // When the last stat is done, go back to the beginning
            if(currentStat == TotalStats)
            {
                currentStat = Brawn;
            }
        }

        calculateStats();

        /*
        Debug.Log( "Brawn: " + brawn );
        Debug.Log( "Agility: " + agility );
        Debug.Log( "Brains: " + brains );
        Debug.Log( "Stamina: " + stamina );
        Debug.Log( "Hit Points: " + getHitPoints() );
        Debug.Log( "Action Points: " + getActionPoints() );
        Debug.Log( "Initiative: " + getInitiative() );
        Debug.Log( "Defense: " + getDefense() );
        Debug.Log( "Damage: " + getDamageMin() + " - " + getDamageMax() );
        Debug.Log( "Weapon: " + weapon.ToString() );
        Debug.Log( "Unspent Points: " + unspentPoints );
        Debug.Log( "Tactical Stance: " + tacticalStance.ToString() );
        */
    }

    void decreaseStat(ref int stat)
    {
        if(stat > Constants.StatMinValue)
        {
            stat --;
            unspentPoints ++;
        }
    }

    /* 
     * This will be used to determine how the AI is placed on the map, and how what
     * actions are more likely to be selected during combat
     */
    void determineTacticalStance()
    {
        // Get random number from 0 to 2
        tacticalStance = ((Constants.TacticalStance) Random.Range(0, 3));
    }

    void increaseStat(ref int stat)
    {
        if(stat < statMax)
        {
            if(unspentPoints > 0)
            {
                stat ++;
                unspentPoints --;
            }
        }
    }
}
