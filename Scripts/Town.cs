using Godot;
using System;
using System.Collections.Generic;

public partial class Town : Area2D, IBuilding, IRoadConnectionPoint
{
    public List<IRoadConnectionPoint> Connections { get; } = [];

    [Export] public ResourceKind BaseProduction;
    [Export] public float BaseProductionRate;

    [Export] public float Satisfaction = 1.0f;

    private float _remainingProductionRate;

    public IReadOnlyDictionary<ResourceKind, float> Production => new Godot.Collections.Dictionary<ResourceKind, float>
    {
        { BaseProduction, BaseProductionRate * Satisfaction }
    };
    
    public IReadOnlyDictionary<ResourceKind, float> Consumption => new Godot.Collections.Dictionary<ResourceKind, float>
    {
        { ResourceKind.Food, 20.0f }
    };

    public void OnMissingProductionResult(Dictionary<ResourceKind, float> result)
    {
        if (result.Count > 0)
        {
            GD.Print("Sad town");
            foreach (var kind in result.Keys)
            {
                var missingRatio = result[kind] / Consumption[kind];
                var importance = Game.Instance.LookupGameResource(kind)?.Importance ?? 0;
                
                var devastation = importance * missingRatio;
                
                var newSatisfaction =  Mathf.Lerp(Satisfaction, 0, devastation * 0.1f);
                if (newSatisfaction < 0) 
                    newSatisfaction = 0;
                
                Satisfaction = newSatisfaction;
            }
        }
        else
        {
            var newSatisfaction = Mathf.Lerp(Satisfaction, 1, 0.2f);
            if (newSatisfaction > 1)
                newSatisfaction = 1;

            Satisfaction = newSatisfaction;
        }
    }

    // public Dictionary<ResourceKind, int> ProductionStorage { get; } = [];

    // public override void _Ready()
    // {
    //     base._Ready();
    //
    //     // Game.Instance.OnProductionTick += ProductionTick;
    // }

    // private void ProductionTick()
    // {
    //     var tickProduction = Mathf.FloorToInt(BaseProductionRate + _remainingProductionRate);
    //     _remainingProductionRate += BaseProductionRate - tickProduction;
    //
    //     ProductionStorage.TryAdd(BaseProduction, 0);
    //     
    //     ProductionStorage[BaseProduction] += tickProduction;
    // }
}
