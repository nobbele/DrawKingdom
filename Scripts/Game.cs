using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Game : Node2D
{
    [Export]
    public int Gold = 100;

    [Export]
    public int Population = 100;

    [Export]
    public int Happiness = 70;

    [Export]
    public int Food = 80;

    [Export]
    public int Wood = 20;

    [Export]
    public int Stone = 20;

    [Export]
    public int Iron = 0;

    [Export]
    public int Day = 1;

    [Export]
    public int DragonVisitDay = 7;

    [Export]
    public int RequiredGold = 500;

    [Export]
    public bool IsGameOver = false;

    public void EndDay()
    {
        int starvingPeople = Population - Food;
        ConsumeFood();
        Starvation(starvingPeople);

        GameOverCheck();
    }

    public void StartDay()
    {
        Day += 1;
        DragonVisitDayCheck();
        TaxIncome();
        // RandomEvent();
    }

    public void GameOverCheck()
    {
        if (Population < 1)
        {
            IsGameOver = true;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private void ConsumeFood()
    {
        Food = Math.Max(Food - Population, 0);
    }

    private void Starvation(int starvingPeople)
    {
        int starvationChance = 15;

        int deaths = 0;
        for (int i = 0; i < starvingPeople; i++)
        {
            bool died = Random.Shared.Next(100) < starvationChance;
            if (died)
            {
                deaths += 1;
            }
        }

        Population -= deaths;
    }

    private void TaxIncome()
    {
        double taxRate = Happiness * 0.01;
        int taxedAmount = (int)Math.Floor(Population * taxRate);
        Gold += taxedAmount;
    }

    private void DragonVisitDayCheck()
    {
        if (Day == DragonVisitDay)
        {
            if (Gold < RequiredGold)
            {
                IsGameOver = true;
                return;
            }

            Gold -= RequiredGold;
            DragonVisitDay += 7;
            RequiredGold *= 2;
        }
    }

    private enum RandomEvent
    {
        Trade,
        FindFood,
        Disease,
        Unhappiness,
        Plunder,
        TaxComplains,
    }

    private RandomEvent SelectRandomEvent()
    {
        Dictionary<RandomEvent, int> weights = new()
        {
            { RandomEvent.Trade, 4500 },
            { RandomEvent.FindFood, 3000 },
            { RandomEvent.Disease, 600 + Population * 3 },
            { RandomEvent.Unhappiness, Happiness < 60 ? 1000 : 0 },
            { RandomEvent.Plunder, 500 },
            { RandomEvent.TaxComplains, 2000 },
        };

        int roll = Random.Shared.Next(weights.Values.Sum());

        int cumulative = roll;
        foreach (var (k, v) in weights)
        {
            if (cumulative < v)
                return k;
				
            cumulative += v;
        }

        return weights.Keys.Last();
    }
}
