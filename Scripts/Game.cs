using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game : Node2D
{
	public static Game Instance { get; private set; }

    [Export] public int Gold = 100;
    [Export] public int Population = 100;
    [Export] public int Happiness = 70;
    [Export] public int Day = 1;
    [Export] public int DragonVisitDay = 7;
    [Export] public int RequiredGold = 500;
    [Export] public bool IsGameOver = false;
    
    [Signal]
    public delegate void OnTickEventHandler();
    
    [Export] public GameResource[] GameResources;
	
    private PackedScene _mouseDraw = ResourceLoader.Load<PackedScene>("uid://cu3d1jy5bxgfl");
    private PackedScene _road = ResourceLoader.Load<PackedScene>("uid://bfx3a1lqwleu8");

    private DrawModeState _activeDraw;

    private Timer _tickTimer;

    public void EndDay()
    {
        // int starvingPeople = Population - Food;
        // ConsumeFood();
        // Starvation(starvingPeople);

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
	public override void _Ready()
	{
		Instance = this;

		FinishDrawState(new DrawModeState
		{
			MouseDraw = null,
			Start = GetNode<Kingdom>("MainKingdom"),
			End = GetNode<Town>("Town"),
			Edges = [
				GetNode<Kingdom>("MainKingdom").GlobalPosition, 
				GetNode<Town>("Town").GlobalPosition
			],
		});

		_tickTimer = new Timer
		{
			Autostart = true,
			WaitTime = 2,
		};
		_tickTimer.Timeout += EmitSignalOnTick;
		AddChild(_tickTimer);

		OnTick += _Tick;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("draw_mode"))
			EnterDrawMode();

		GetNode<Label>("CanvasLayer/Control/VBoxContainer/Gold/Label").Text = $"Gold: {Gold}";
		GetNode<Label>("CanvasLayer/Control/VBoxContainer/Population/Label").Text = $"Population: {Population}";
	}

	private void _Tick()
	{
		// TODO more advanced trade route mechanics..
		// TODO currently ignores disjoint networks (missing trade route)
		
		var buildings = GetTree().GetNodesInGroup("Building").Cast<IBuilding>();
		var towns = GetTree().GetNodesInGroup("Towns").Cast<Town>();

		var production = towns
			.Select(t => t.Production)
			.SelectMany(dict => dict)
			.ToLookup(pair => pair.Key, pair => pair.Value)
			.ToDictionary(group => group.Key, group => group.Sum());

		foreach (var produce in production)
		{
			GD.Print($"production: {produce.Key} => {produce.Value}");
		}

		foreach (var building in buildings)
		{
			var missingProductionResult = new Dictionary<ResourceKind, float>();
			foreach (var resourceKind in building.Consumption.Keys)
			{
				production.TryAdd(resourceKind, 0);
				
				var availableProduction = production[resourceKind];
				var missingProduction = building.Consumption[resourceKind] - availableProduction;
				GD.Print($"{resourceKind}: avail={availableProduction} consume={ building.Consumption[resourceKind]}");
				
				availableProduction -= building.Consumption[resourceKind];
				production[resourceKind] = availableProduction;
				
				
				if (missingProduction > 0)
				{
					GD.Print($"We sad :( no {resourceKind} available");
					missingProductionResult.Add(resourceKind, missingProduction);
				}
			}
			
			building.OnMissingProductionResult(missingProductionResult);
		}
		
		var underproduction = production
			.Where(pair => pair.Value < 0)
			.ToDictionary(pair => pair.Key, pair => pair.Value);
		
		var overproduction = production
			.Where(pair => pair.Value > 0)
			.ToDictionary(pair => pair.Key, pair => pair.Value);

		var produceSale = overproduction.Sum(pair => (LookupGameResource(pair.Key)?.SellingPrice ?? 0) * pair.Value);
		GD.Print($"Total sale: {produceSale}");

		Gold += Mathf.RoundToInt(produceSale);

		// TODO display missingProductions?

		Population = GetTree().GetNodesInGroup("Building").OfType<Kingdom>().Sum(kingdom => kingdom.Population);
	}
	
    // private void ConsumeFood()
    // {
    //     Food = Math.Max(Food - Population, 0);
    // }

    private void Starvation(int starvingPeople)
    {
        const int starvationChance = 15;

        var deaths = 0;
        for (var i = 0; i < starvingPeople; i++)
        {
            var died = Random.Shared.Next(100) < starvationChance;
            if (died)
            {
                deaths += 1;
            }
        }

        Population -= deaths;
    }

    private void TaxIncome()
    {
        var taxRate = Happiness * 0.01;
        var taxedAmount = (int)Math.Floor(Population * taxRate);
        Gold += taxedAmount;
    }

    private void DragonVisitDayCheck()
    {
	    if (Day != DragonVisitDay) return;
	    
	    if (Gold < RequiredGold)
	    {
		    IsGameOver = true;
		    return;
	    }

	    Gold -= RequiredGold;
	    DragonVisitDay += 7;
	    RequiredGold *= 2;
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
        
        var roll = Random.Shared.Next(weights.Values.Sum());

        var cumulative = roll;
        foreach (var (k, v) in weights)
        {
            if (cumulative < v)
                return k;
				
            cumulative += v;
        }

        return weights.Keys.Last();
    }

	public GameResource LookupGameResource(ResourceKind kind) 
		=> GameResources.FirstOrDefault(r => r.Kind == kind);

	private bool IsDrawTarget(Vector2 pos)
	{
		var drawState = _activeDraw!;

		var isFirst = drawState.Start == null;
		
		var spaceState = GetWorld2D().DirectSpaceState;
		var intersections = spaceState.IntersectPoint(new PhysicsPointQueryParameters2D
		{
			Position = pos,
			CollideWithAreas = true,
			CollideWithBodies = true,
			CollisionMask = uint.MaxValue,
		});
		
		foreach (var intersection in intersections)
		{
			var collider = intersection["collider"].As<Area2D>();
			if (collider is IRoadConnectionPoint roadConnection && roadConnection != drawState.Start)
			{
				if (isFirst)
					drawState.Start = roadConnection;
				else
					drawState.End = roadConnection;
				
				return true;
			}
		}

		return false;
	}

	public void EnterDrawMode()
	{
		if (_activeDraw != null) return;

		var mouseDraw = _mouseDraw.Instantiate<MouseDraw>();
		
		mouseDraw.IsTarget = IsDrawTarget;
		mouseDraw.OnComplete += (edges) =>
		{
			var drawState = _activeDraw!;
			drawState.Edges = edges.ToArray();
			
			FinishDrawState(drawState);
			
			_activeDraw = null;
		};
		
		AddChild(mouseDraw);

		_activeDraw = new DrawModeState
		{
			MouseDraw = mouseDraw,
		};
	}

	private void FinishDrawState(DrawModeState drawState)
	{
		var previous = drawState.Start;
		for (var i = 0; i < drawState.Edges.Length - 1; i++)
		{
			var first = drawState.Edges[i];
			var second = drawState.Edges[i + 1];
			
			var road = _road.Instantiate<Road>();
			road.Position = first;
			road.Rotation = first.AngleToPoint(second) - Mathf.Pi / 2;
			road.Length = first.DistanceTo(second);

			previous.Connections.Add(road);
			road.Connections.Add(previous);
			
			AddChild(road);
		}
		
		previous.Connections.Add(drawState.End);
		drawState.End.Connections.Add(previous);

		foreach (var conn in drawState.End.GetAllConnectedBuilding())
		{
			GD.Print(conn);
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
}
