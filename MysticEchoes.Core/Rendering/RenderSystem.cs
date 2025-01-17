﻿using Leopotam.EcsLite;
using MazeGeneration;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using SevenBoldPencil.EasyDi;
using SharpGL;
using Point = MysticEchoes.Core.Base.Geometry.Point;
using Rectangle = MysticEchoes.Core.Base.Geometry.Rectangle;
using Size = MysticEchoes.Core.Base.Geometry.Size;


namespace MysticEchoes.Core.Rendering;

public class RenderSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private OpenGL _gl;

    private EcsFilter _rendersFilter;
    private EcsPool<RenderComponent> _renders;
    
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<TileMapComponent> _tileMaps;

    private static readonly Dictionary<CellType, double[]> TileColors = new()
    {
        [CellType.Empty] = new[] { 64d/255, 64d/255, 64d/255 },
        [CellType.FragmentBound] = new[] { 0d,0d,0d },
        [CellType.Hall] = new[] { 0.8d, 0.8d, 0.1d },
        [CellType.ControlPoint] = new[] { 0.8d, 0.1d, 0.1d },
        [CellType.Wall] = new[] { 103d/255, 65d/255, 72d/255 },
        [CellType.Floor] = new[] {53d/255,25d/255,48d/255}
    };

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        _renders = world.GetPool<RenderComponent>();
        _rendersFilter = world.Filter<RenderComponent>().End();

        _transforms = world.GetPool<TransformComponent>();
        _tileMaps = world.GetPool<TileMapComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        if (_gl is null)
            throw new InvalidOperationException("Open Gl wasn't initialized");

        _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _gl.LoadIdentity();
        _gl.Ortho(0, 2, 0, 2, -1, 1);

        foreach (var entityId in _rendersFilter)
        {
            ref RenderComponent render = ref _renders.Get(entityId);

            if (render.Type is RenderingType.TileMap)
            {
                ref TileMapComponent map = ref _tileMaps.Get(entityId);
                {
                    var color = TileColors[CellType.Empty];
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                    _gl.Color(color[0], color[1], color[2]);
                    _gl.Vertex(0d, 0d);
                    _gl.Vertex(2d, 0d);
                    _gl.Vertex(2d, 2d);
                    _gl.Vertex(0d, 2d);
                    _gl.End();
                }
                
                foreach (var floor in map.Tiles.FloorTiles)
                {
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                
                    var color = TileColors[CellType.Floor];
                
                    var rect = new Rectangle(
                        new Point(floor.X * map.TileSize.Width, floor.Y * map.TileSize.Height),
                        new Size(map.TileSize.Width, map.TileSize.Height)
                    );
                
                    _gl.Color(color[0], color[1], color[2]);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y);
                    _gl.End();
                }
                foreach (var floor in map.Tiles.WallTiles)
                {
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);

                    var color = TileColors[CellType.Wall];

                    var rect = new Rectangle(
                        new Point(floor.X * map.TileSize.Width, floor.Y * map.TileSize.Height),
                        new Size(map.TileSize.Width, map.TileSize.Height)
                    );

                    _gl.Color(color[0], color[1], color[2]);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y);
                    _gl.End();
                }
            }
            else if (render.Type is RenderingType.DebugUnitView)
            {
                ref TransformComponent transform = ref _transforms.Get(entityId);

                _gl.Begin(OpenGL.GL_QUADS);
                _gl.Color(1f, 0f, 0f);

                const float halfSize = 0.05f;

                _gl.Vertex(transform.Position.X - halfSize, transform.Position.Y + halfSize);
                _gl.Vertex(transform.Position.X - halfSize, transform.Position.Y - halfSize);
                _gl.Vertex(transform.Position.X + halfSize, transform.Position.Y - halfSize);
                _gl.Vertex(transform.Position.X + halfSize, transform.Position.Y + halfSize);
                _gl.End();
            }
            else if (render.Type is not RenderingType.None)
            {
                throw new NotImplementedException();
            }
        }
    }
}