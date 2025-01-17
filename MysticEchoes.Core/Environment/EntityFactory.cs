﻿using Leopotam.EcsLite;

namespace MysticEchoes.Core.Environment;

public class EntityFactory
{
    private int _currentEntity = -1;
    private readonly EcsWorld _world;

    public EntityFactory(EcsWorld world)
    {
        _world = world;
    }

    public EntityFactory Create() {
        _currentEntity = _world.NewEntity();
        return this;
    }
    
    public EntityFactory Add<T>(T component) where T : struct {
        if (_currentEntity == -1)
        {
            throw new System.Exception("No current entity. You must call Create() before adding components.");
        }

        var poolWithTemplateComponent = _world.GetPool<T>();
        poolWithTemplateComponent.Add(_currentEntity) = component;
        
        return this;
    }
    
    public int End() {
        if (_currentEntity == -1)
            throw new System.Exception("No current entity. You must call Create() before calling End().");

        var result = _currentEntity;
        _currentEntity = -1;
        return result;
    }
}