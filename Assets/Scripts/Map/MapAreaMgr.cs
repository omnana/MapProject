using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAreaMgr : MonoBehaviour
{
    private MapArea root;

    private System.Random random;

    /// <summary> 房间数据 /// </summary>
    public List<RoomData> RoomDatas { get; private set; }

    /// <summary> 走廊数据 </summary>
    public List<RoomData> CorridorDatas { get; private set; }

    public List<RoomData> DoorDatas { get; private set; }

    public static MapAreaMgr Instance;

    private int areaIndex;

    private int gridIndex;

    private int corridorWidth;

    private GridData[,] gridMap;

    public GridData[,] AllGrids
    {
        get
        {
            return gridMap;
        }
    }


    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 
    /// </summary>
    public void LoadData()
    {
        gridIndex = 0;

        corridorWidth = Config.CorridorWidth;

        random = Const.Random;

        RoomDatas = new List<RoomData>();

        CorridorDatas = new List<RoomData>();

        DoorDatas = new List<RoomData>();

        gridMap = new GridData[Config.MapAreaWidth, Config.MapAreaHeight];

        root = new MapArea(Coordinate.Zero, Config.MapAreaWidth, Config.MapAreaHeight);

        CreateMapAreaGroup(root, Config.MapAreaDepth);

        CreateCorridors(root);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public GridData GetGrid(Coordinate coord)
    {
        if (coord.X < 0 || coord.Y < 0 || coord.X >= Config.MapAreaWidth || coord.Y >= Config.MapAreaHeight) return null;

        return gridMap[coord.X, coord.Y];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetGridId()
    {
        return gridIndex++;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coord"></param>
    public void SetGrid(GridData data)
    {
        gridMap[data.WorldCoord.X, data.WorldCoord.Y] = data;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public bool HasGrid(GridData data)
    {
        return gridMap[data.WorldCoord.X, data.WorldCoord.Y] != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool HasGridInWorldCoord(int x, int y)
    {
        return gridMap[x, y] != null;
    }

    /// <summary>
    /// 初始化区域
    /// </summary>
    /// <param name="area"></param>
    /// <param name="depth"></param>
    private void CreateMapAreaGroup(MapArea area, int depth)
    {
        if (depth == 0) // 到叶子端
        {
            CreateRoomData(area);

            return;
        }

        var w = area.Wdith;

        var h = area.Height;

        var isVertical = w < h;

        var addSub = random.Next(4, 6) / 10f;

        if (isVertical)
        {
            var half = (int)(addSub * h);

            var upCoord = new Coordinate(0, half) + area.Coordinate;

            area.LeftChild = new MapArea(area.Coordinate, area.Wdith, half) { Id = areaIndex++ };

            area.RightChild = new MapArea(upCoord, area.Wdith, h - half) { Id = areaIndex++ };
        }
        else
        {
            var half = (int)(addSub * w);

            var rightCoord = new Coordinate(half, 0) + area.Coordinate;

            area.LeftChild = new MapArea(area.Coordinate, half, area.Height) { Id = areaIndex++ };

            area.RightChild = new MapArea(rightCoord, w - half, area.Height) { Id = areaIndex++ };
        }

        CreateMapAreaGroup(area.LeftChild, depth - 1);

        CreateMapAreaGroup(area.RightChild, depth - 1);
    }

    /// <summary>
    /// 创建房间数据
    /// </summary>
    /// <param name="area"></param>
    private void CreateRoomData(MapArea area)
    {
        if (area == null) return;

        var maxW = area.Wdith - 2;

        var maxH = area.Height - 2;

        var minW = maxW / 2;

        var minH = maxH / 2;

        var roomData = new RoomData()
        {
            RoomType = RoomType.Room,

            Id = RoomDatas.Count,

            Width = Const.Random.Next(minW, maxW),

            Height = Const.Random.Next(minH, maxH),
        };

        var xMax = area.Wdith - roomData.Width;

        var yMax = area.Height - roomData.Height;

        roomData.Coordinate = new Coordinate(Const.Random.Next(0, xMax), Const.Random.Next(0, yMax)) + area.Coordinate;

        roomData.Build();

        RoomDatas.Add(roomData);

        area.RoomData = roomData;
    }

    /// <summary>
    /// 寻找连个房间的门
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    /// <param name="isVertical"></param>
    /// <returns></returns>
    private Coordinate[] FindRoomsDoor(RoomData room1, RoomData room2, bool isVertical)
    {
        var doors = new Coordinate[2];
        if (isVertical)
        {
            var x1 = 0;
            var x2 = 0;

            // 有交集
            if (room1.LeftDown.X >= room2.LeftDown.X && room1.RightUp.X <= room2.RightUp.X)
            {
                x1 = random.Next(room1.LeftDown.X, room1.RightUp.X - corridorWidth);
                x2 = x1;
            }
            else if (room1.LeftDown.X <= room2.LeftDown.X && room1.RightUp.X >= room2.RightUp.X)
            {
                x1 = random.Next(room2.LeftDown.X, room2.RightUp.X - corridorWidth);
                x2 = x1;
            }
            else if(room1.LeftDown.X >= room2.LeftDown.X && room1.LeftDown.X <= room2.RightUp.X)
            {
                x1 = random.Next(room1.LeftDown.X, room2.RightUp.X - corridorWidth);
                x2 = x1;
            }
            else if (room1.RightUp.X >= room2.LeftDown.X && room1.RightUp.X <= room2.RightUp.X)
            {
                x1 = random.Next(room2.LeftDown.X, room1.RightUp.X - corridorWidth);
                x2 = x1;
            }
            else
            {
                x1 = random.Next(room1.LeftDown.X, room1.RightUp.X - corridorWidth);
                x2 = random.Next(room2.LeftDown.X, room2.RightUp.X - corridorWidth);
            }
            doors[0] = new Coordinate(x1, room1.RightUp.Y + 1);
            doors[1] = new Coordinate(x2, room2.LeftDown.Y - 1);
        }
        else
        {
            var y1 = 0;
            var y2 = 0;

            // 有交集
            if (room1.LeftDown.Y >= room2.LeftDown.Y && room1.RightUp.Y <= room2.RightUp.Y)
            {
                y1 = random.Next(room1.LeftDown.Y, room1.RightUp.Y - corridorWidth);
                y2 = y1;
            }
            else if (room1.LeftDown.Y <= room2.LeftDown.Y && room1.RightUp.Y >= room2.RightUp.Y)
            {
                y1 = random.Next(room2.LeftDown.Y, room2.RightUp.Y - corridorWidth);
                y2 = y1;
            }
            else if(room1.LeftDown.Y >= room2.LeftDown.Y && room1.LeftDown.Y <= room2.RightUp.Y)
            {
                y1 = random.Next(room1.LeftDown.Y, room2.RightUp.Y - corridorWidth);
                y2 = y1;
            }
            else if (room1.RightUp.Y >= room2.LeftDown.Y && room1.RightUp.Y <= room2.RightUp.Y)
            {
                y1 = random.Next(room2.LeftDown.Y, room1.RightUp.Y - corridorWidth);
                y2 = y1;
            }
            else
            {
                y1 = random.Next(room1.LeftDown.Y, room1.RightUp.Y - corridorWidth);
                y2 = random.Next(room2.LeftDown.Y, room2.RightUp.Y - corridorWidth);
            }

            doors[0] = new Coordinate(room1.RightUp.X + 1, y1);
            doors[1] = new Coordinate(room2.LeftDown.X - 1, y2);
        }

        return doors;
    }

    /// <summary>
    /// 创建走廊数据
    /// </summary>
    /// <param name="area"></param>
    private void CreateCorridors(MapArea area)
    {
        if (area == null || area.LeftChild == null || area.RightChild == null) return;

        var area1 = area.LeftChild;

        var area2 = area.RightChild;

        CreateCorridors(area1);

        CreateCorridors(area2);

        var room1 = area1.GetRoom();

        var room2 = area2.GetRoom();

        if (room1 == null || room2 == null) return;

        var id = CorridorDatas.Count;

        var isVertical = area1.Coordinate.X == area2.Coordinate.X;

        var doors = FindRoomsDoor(room1, room2, isVertical);

        var w = Mathf.Abs(doors[1].X - doors[0].X) + 1;

        var h = Mathf.Abs(doors[1].Y - doors[0].Y) + 1;

        if (isVertical) // 垂直
        {
            if (w > corridorWidth)
            {
                var startX = doors[0].X < doors[1].X ? doors[0].X : doors[1].X;

                var endX = doors[0].X < doors[1].X ? doors[1].X - corridorWidth + 1 : doors[1].X;

                var roomData = new RoomData()
                {
                    RoomType = RoomType.Corridor,

                    Id = id,

                    Width = w,

                    Height = h >= corridorWidth ? corridorWidth : h,

                    Coordinate = new Coordinate(startX, doors[0].Y),

                    Start = room1.Id,

                    End = room2.Id,
                };

                roomData.Build();

                CorridorDatas.Add(roomData);

                if (h > corridorWidth)
                {
                    roomData = new RoomData()
                    {
                        RoomType = RoomType.Corridor,

                        Id = id,

                        Width = corridorWidth,

                        Height = h - corridorWidth,

                        Coordinate = new Coordinate(endX, doors[0].Y + corridorWidth),

                        Start = room1.Id,

                        End = room2.Id,
                    };

                    roomData.Build();

                    CorridorDatas.Add(roomData);
                }
            }
            else
            {
                if (h > 0)
                {
                    var roomData = new RoomData()
                    {
                        RoomType = RoomType.Corridor,

                        Id = id,

                        Width = corridorWidth,

                        Height = h,

                        Coordinate = doors[0],

                        Start = room1.Id,

                        End = room2.Id,
                    };

                    roomData.Build();

                    CorridorDatas.Add(roomData);
                }
            }
        }
        else // 水平
        {
            if (h > corridorWidth)
            {
                var startY = doors[0].Y < doors[1].Y ? doors[0].Y : doors[1].Y;

                var endY = doors[0].Y < doors[1].Y ? doors[1].Y - corridorWidth + 1 : doors[1].Y;

                var roomData = new RoomData()
                {
                    RoomType = RoomType.Corridor,

                    Id = id,

                    Width = w,

                    Height = h >= corridorWidth ? corridorWidth : h,

                    Coordinate = new Coordinate(doors[0].X, startY),

                    Start = room1.Id,

                    End = room2.Id,
                };

                roomData.Build();

                CorridorDatas.Add(roomData);

                if (h > corridorWidth)
                {
                    roomData = new RoomData()
                    {
                        RoomType = RoomType.Corridor,

                        Id = id,

                        Width = corridorWidth,

                        Height = h - corridorWidth,

                        Coordinate = new Coordinate(doors[0].X, endY),

                        Start = room1.Id,

                        End = room2.Id,
                    };

                    roomData.Build();

                    CorridorDatas.Add(roomData);
                }
            }
            else
            {
                if (w > 0)
                {
                    var roomData = new RoomData()
                    {
                        RoomType = RoomType.Corridor,

                        Id = id,

                        Width = w,

                        Height = corridorWidth,

                        Coordinate = doors[0],

                        Start = room1.Id,

                        End = room2.Id,
                    };

                    roomData.Build();

                    CorridorDatas.Add(roomData);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public RoomData RandomARoom()
    {
        var roomIndex = random.Next(0, RoomDatas.Count);

        return RoomDatas[roomIndex];
    }
}
