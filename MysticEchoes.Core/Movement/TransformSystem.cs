﻿using Leopotam.EcsLite;
using MysticEchoes.Core.Environment;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Movement;

public class TransformSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _context;
    
    private EcsPool<TransformComponent> _transforms;
    private EcsFilter _transformsFilter;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        _transforms = world.GetPool<TransformComponent>();
        _transformsFilter = world.Filter<TransformComponent>().End();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _transformsFilter)
        {
            ref TransformComponent transform = ref _transforms.Get(entityId);
            transform.Position += transform.Velocity * _context.DeltaTime;
        }
    }
}