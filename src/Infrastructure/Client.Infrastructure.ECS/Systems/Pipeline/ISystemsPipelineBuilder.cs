using Arch.Core;
using Arch.System;

namespace Client.Infrastructure.ECS.Systems.Pipeline;

// 1. Builder para configurar pipeline
public interface ISystemsPipelineBuilder
{
    ISystemsPipelineBuilder AddSystem<T>(T instance) where T : BaseSystem<World, float>;
    ISystemsPipelineBuilder AddSystem<T>(T instance, string description) where T : BaseSystem<World, float>;
    Group<float> Build();
}