﻿using System.Diagnostics;
using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Environment;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;
using SharpGL;

namespace MysticEchoes.Core;

public class Game
{
    private readonly EcsWorld _world;

    private readonly EcsSystems _setupSystems;
    private readonly EcsSystems _gameplaySystems;
    private readonly EcsSystems _renderSystems;
    
    private readonly IMazeGenerator _mazeGenerator;
    private readonly EntityFactory _entityFactory;
    private readonly SystemExecutionContext _systemExecutionContext;
    private readonly Stopwatch _updateTimer;

    public Game(IMazeGenerator mazeGenerator)
    {
        _mazeGenerator = mazeGenerator;

        _world = new EcsWorld();
        _entityFactory = new EntityFactory(_world);
        
        _systemExecutionContext = new SystemExecutionContext();
        _updateTimer = new Stopwatch();
        
        _setupSystems = new EcsSystems(_world);
        _setupSystems
            .Add(new InitEnvironmentSystem());
        
        _gameplaySystems = new EcsSystems(_world);
        _gameplaySystems
            .Add(new TransformSystem());

        _renderSystems = new EcsSystems(_world);
        _renderSystems
            .Add(new RenderSystem());
    }

    public void Initialize(OpenGL gl)
    {
        _setupSystems
            .Inject(_entityFactory, _mazeGenerator)
            .Init();

        _gameplaySystems            
            .Inject(_systemExecutionContext)
            .Init();
        
        _renderSystems
            .Inject(gl)
            .Init();
    }

    public void Update()
    {
        // _updateTimer.Stop();
        _systemExecutionContext.DeltaTime = _updateTimer.ElapsedMilliseconds / 1e3f;
        _updateTimer.Restart();
        
        _gameplaySystems.Run();


        // _updateTimer.Start();
    }

    public void Render()
    {
        _renderSystems.Run();
    }
}