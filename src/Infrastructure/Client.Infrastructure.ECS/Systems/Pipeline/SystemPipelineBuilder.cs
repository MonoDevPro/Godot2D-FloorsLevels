using Arch.Core;
using Arch.System;

namespace Client.Infrastructure.ECS.Systems.Pipeline;

public class SystemsPipelineBuilder : ISystemsPipelineBuilder
{
    private readonly List<SystemRegistration> _systems = [];

    public ISystemsPipelineBuilder AddSystem<T>(T instance) where T : BaseSystem<World, float>
    {
        return AddSystem<T>(instance, typeof(T).Name);
    }
    
    public ISystemsPipelineBuilder AddSystem<T>(T instance, string description) where T : BaseSystem<World, float>
    {
        _systems.Add(new SystemRegistration(instance, description));
        return this;
    }
    
    public Group<float> Build()
    {
        var systems = _systems
            .Select(i => i.Instance)
            .ToArray();
            
        return new Group<float>("ECS Systems Pipeline", systems);
    }
    
    private record SystemRegistration(ISystem<float> Instance, string Description);
}