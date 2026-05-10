using Godot;
using System;
using System.Collections.Generic;

public partial class Kingdom : Area2D, IBuilding, IRoadConnectionPoint
{
    public List<IRoadConnectionPoint> Connections { get; } = [];

    public IReadOnlyDictionary<ResourceKind, float> Consumption => new Dictionary<ResourceKind, float>()
    {
        { ResourceKind.Food, Population }
    };
    
    [Export] public int Population { get; set; } = 25;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Population <= 0)
        {
            GD.Print("Kingdom exploded :(");
            QueueFree();
        }
    }

    public void OnMissingProductionResult(Dictionary<ResourceKind, float> result)
    {
        if (result.Count > 0)
        {
            GD.Print("Sad kingdom");
            
            var missingFood = result.GetValueOrDefault(ResourceKind.Food);
            if (missingFood > 0)
            {
                var newPopulation =  Population - (int)Mathf.Max(1, missingFood / 3);
                if (newPopulation < 0) 
                    newPopulation = 0;
                
                Population = newPopulation;
            }
        }
        else
        {
            Population += (int)Mathf.Max(Population * 0.04f, 1);
        }
    }

    public override void _ExitTree()
    {
        foreach (var conn in Connections)
        {
            conn.Connections.Remove(this);
        }
        
        base._ExitTree();
    }
}
