using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Platform.Helper;

namespace Battleship.Platform.Bot;

public class NBot(string name) : IBot
{
    private string _name = name;
    private Random _randomNumberGenerator = new();
    private List<int> _shipsSailing = [5, 4, 3, 3, 2];
    private List<Coordinate> _shipsDestroyed = [];
    private List<Coordinate> _positionAdjecentToDamaged = [];
    private List<Coordinate> _missedPositions = [];

    public string GetName()
    {
        return _name;
    }

    public Coordinate NextPosition(in FieldState[,] board)
    {
        Coordinate coordinate = new()
        {
            X = 0,
            Y = 0
        };

        SeekDestroyedShips(in board);
        SeekDamagedShips(in board);

        if (_positionAdjecentToDamaged.Count > 0)
            return _positionAdjecentToDamaged.First();

        SeekMissedPositions(in board);

        coordinate = FindBestRandomPosition(board);

        return coordinate;
    }

    public void NewGame()
    {
        _shipsSailing = [5, 4, 3, 3, 2];
    }

    private void SeekDestroyedShips(in FieldState[,] board)
    {
        for (int i = 0; i < 10; ++i)
            for (int j = 0; j < 10; ++j)
            {
                if (board[i, j] == FieldState.Destroyed
                    && !_shipsDestroyed.Contains(new Coordinate() 
                    {
                        X = i, 
                        Y = j
                    }))
                    {
                        int size = 1;
                        int xDirection = 0;
                        int yDirection = 1;

                        if (j + 1 < 10
                            && ((board[i, j + 1] == FieldState.Destroyed
                                    && _shipsDestroyed.Contains(new Coordinate()
                                    {
                                        X = i,
                                        Y = j + 1
                                    }))
                                || board[i, j + 1] != FieldState.Destroyed)) 
                            {
                                xDirection = 1;
                                yDirection = 0;
                            }
                        
                        _shipsDestroyed.Add(new Coordinate()
                        {
                            X = i,
                            Y = j
                        });

                        Coordinate nextCoordinate = new()
                        {
                            X = i + xDirection,
                            Y = j + yDirection
                        };

                        while (nextCoordinate.X < 10
                            && nextCoordinate.Y < 10
                            && board[nextCoordinate.X, nextCoordinate.Y] == FieldState.Destroyed
                            && !_shipsDestroyed.Contains(nextCoordinate))
                        {
                            size++;
                            _shipsDestroyed.Add(nextCoordinate);

                            nextCoordinate = new()
                            {
                                X = nextCoordinate.X + xDirection,
                                Y = nextCoordinate.Y + yDirection
                            };
                        }

                        _shipsSailing.Remove(size);
                    }
            }
    }

    private void SeekDamagedShips(in FieldState[,] board)
    {
        _positionAdjecentToDamaged = [];
        
        int[] spaningFactorX = [0, 1];
        int[] spaningFactorY = [1, 0];

        for (int i = 0; i < 10; ++i)
            for (int j = 0; j < 10; ++j)
                if (board[i, j] == FieldState.Damaged)
                {
                    int index = 0;

                    while (_positionAdjecentToDamaged.Count == 0)
                    {
                        int size = 1;

                        int nextX = i - spaningFactorX[index];
                        int nextY = j - spaningFactorY[index];

                        if (nextX >= 0
                            && nextY >= 0
                            && board[nextX, nextY] == FieldState.Empty)
                            _positionAdjecentToDamaged.Add(new Coordinate()
                            {
                                X = nextX,
                                Y = nextY
                            });

                        nextX = i + size * spaningFactorX[index];
                        nextY = j + size * spaningFactorY[index];

                        while (nextX < 10
                            && nextY < 10
                            && board[nextX, nextY] == FieldState.Damaged)
                        {
                            size++;
                            nextX = i + size * spaningFactorX[index];
                            nextY = j + size * spaningFactorY[index];
                        }

                        if (nextX < 10
                            && nextY < 10
                            && board[nextX, nextY] == FieldState.Empty)
                            _positionAdjecentToDamaged.Add(new Coordinate()
                            {
                                X = nextX,
                                Y = nextY
                            });
                        
                        if (index++ > 0)
                            break;
                    }

                    return;
                }
    }

    private void SeekMissedPositions(in FieldState[,] board)
    {
        _missedPositions = [];

        for (int i = 0; i < 10; ++i)
            for (int j = 0; j < 10; ++j)
                if (board[i, j] == FieldState.Missed)
                    _missedPositions.Add(new Coordinate()
                    {
                        X = i,
                        Y = j
                    });
    }

    private Coordinate FindBestRandomPosition(in FieldState[,] board)
    {
        Coordinate bestCoordinate = new()
        {
            X = 0,
            Y = 0
        };

        List<Coordinate> allEmptyFields = GetAllEmptyFields(in board);
        int nextField;

        if (_missedPositions.Count == 0)
        {
            nextField = _randomNumberGenerator.Next(allEmptyFields.Count);
            return allEmptyFields.ElementAt(nextField);
        }

        int[] spaningFactorX = [-1, 0, 1, 0];
        int[] spaningFactorY = [0, 1, 0, -1];
        int nextX, nextY;

        foreach (int biggestShip in _shipsSailing)
        {
            allEmptyFields = GetAllEmptyFields(in board);

            foreach (Coordinate coordinate in _missedPositions)
            {
                for (int i = 0; i < 4; ++i)
                    for (int j = 1; j < biggestShip; ++j)
                    {
                        nextX = coordinate.X + spaningFactorX[i] * j;
                        nextY = coordinate.Y + spaningFactorY[i] * j;
                        
                        if (nextX >= 0 && nextX < 10
                            && nextY >= 0 && nextY < 10)
                            allEmptyFields.Remove(new Coordinate()
                            {
                                X = nextX,
                                Y = nextY
                            });
                    }
            }

            if (allEmptyFields.Count > 0)
            {
                int nextElement = _randomNumberGenerator.Next(allEmptyFields.Count);
                return allEmptyFields.ElementAt(nextElement);
            }
        }

        allEmptyFields = GetAllEmptyFields(in board);
        nextField = _randomNumberGenerator.Next(allEmptyFields.Count);
        bestCoordinate = allEmptyFields.ElementAt(nextField);

        return bestCoordinate;
    }

    private List<Coordinate> GetAllEmptyFields(in FieldState[,] board)
    {
        List<Coordinate> allEmptyFields = [];

        for (int i = 0; i < 10; ++i)
            for (int j = 0; j < 10; ++j)
                if (board[i, j] == FieldState.Empty)
                    allEmptyFields.Add(new Coordinate()
                    {
                        X = i,
                        Y = j
                    });

        return allEmptyFields;
    }
}