using System.Collections.Generic;

public interface IBuilding : IRoadConnectionPoint
{
    IReadOnlyDictionary<ResourceKind, float> Consumption { get; }
    
    void OnMissingProductionResult(Dictionary<ResourceKind, float> missingProduction);
}