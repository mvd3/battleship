using System;
using System.Collections.Generic;
using System.Diagnostics;
using Battleship.Platform.Bot;
using Battleship.Platform.Helper;

namespace Battleship.Platform;

public class Platform
{
    private FieldState[,] _leftBoard, _rightBoard;
    private Ship[] _leftPlayerShips, _rightPlayerShips;
    private Random _randomNumberGenerator;
    private NBot _leftBot, _rightBot;
    private readonly int[] _shipSize;
    private readonly int[] _orientationSpaningFactorX;
    private readonly int[] _orientationSpaningFactorY;
    public bool _currentlyPlaying;

    public Platform()
    {
        _leftBoard = new FieldState[Data.TABLE_SIZE, Data.TABLE_SIZE];
        _rightBoard = new FieldState[Data.TABLE_SIZE, Data.TABLE_SIZE];
        _leftPlayerShips = new Ship[Data.NUMBER_OF_SHIPS];
        _rightPlayerShips = new Ship[Data.NUMBER_OF_SHIPS];
        _randomNumberGenerator = new();
        _leftBot = new(Data.LEFT_DEFAULT_PLAYER);
        _rightBot = new(Data.RIGHT_DEFAULT_PLAYER);
        _currentlyPlaying = false;
        _shipSize = [5, 4, 3, 3, 2];
        _orientationSpaningFactorX = [-1, 0, 1, 0];
        _orientationSpaningFactorY = [0, 1, 0, -1];
    }

    public void StartGame(GameType gameType)
    {
        for (int i = 0; i < Data.TABLE_SIZE; ++i)
            for (int j = 0; j < Data.TABLE_SIZE; ++j)
            {
                _leftBoard[i, j] = FieldState.Empty;
                _rightBoard[i, j] = FieldState.Empty;
            }

        _leftPlayerShips = ArrangeShipsOnBoard();
        _rightPlayerShips = ArrangeShipsOnBoard();

        _currentlyPlaying = true;
    }

    public string GetLeftBotName()
    {
        return _leftBot.GetName() ?? Data.LEFT_DEFAULT_PLAYER;
    }

    public string GetRightBotName()
    {
        return _rightBot.GetName() ?? Data.RIGHT_DEFAULT_PLAYER;
    }
    
    public (Coordinate, FieldState) BotNextMove(bool isLeftBot = true, bool botVersusBot = false)
    {
        FieldState[,] board = botVersusBot && isLeftBot ? _rightBoard : _leftBoard;
        Ship[] ships = botVersusBot && isLeftBot ? _rightPlayerShips : _leftPlayerShips;

        Coordinate coordinate = isLeftBot ? _leftBot.NextPosition(board, ships) : _rightBot.NextPosition(board, ships);
        FieldState fieldState = FieldState.Missed;

        if (board[coordinate.X, coordinate.Y] != FieldState.Empty)
            coordinate = GetFirstEmptyField(board);

        bool isHit = false;

        foreach (Ship ship in ships)
        {
            if (ship.Position.Contains(coordinate))
            {
                isHit = true;
                break;
            }
        }

        if (isHit)
        {
            fieldState = FieldState.Damaged;

            foreach (Ship ship in ships)
                if (ship.Position.Contains(coordinate))
                {
                    if (--ship.FieldsUndamaged == 0)
                    {
                        fieldState = FieldState.Destroyed;

                        foreach (Coordinate shipCoordinate in ship.Position)
                            board[shipCoordinate.X, shipCoordinate.Y] = fieldState;
                    }
                    else
                        board[coordinate.X, coordinate.Y] = fieldState;

                    break;
                }
        } else
            board[coordinate.X, coordinate.Y] = fieldState;

        return (coordinate, fieldState);
    }
    
    public Coordinate[] GetAllShipPositions(bool defaultLeft = true)
    {
        List<Coordinate> shipPositions = [];
        
        foreach(Ship ship in defaultLeft ? _leftPlayerShips : _rightPlayerShips)
            shipPositions.AddRange(ship.Position);
        
        return [.. shipPositions];
    }

    public Coordinate[] GetPositionOfDestroyedShip(Coordinate coordinate, bool leftPlayer = true)
    {
        Coordinate[] position = [];

        foreach (Ship ship in leftPlayer ? _rightPlayerShips : _leftPlayerShips)
            if (ship.Position.Contains(coordinate))
                return [.. ship.Position];

        return position;
    }

    private Ship[] ArrangeShipsOnBoard()
    {
        FieldState[,] board = new FieldState[Data.TABLE_SIZE, Data.TABLE_SIZE];
        Ship[] ships = new Ship[Data.NUMBER_OF_SHIPS];
        int x, y, orientation;
        int shipIndex = 0;

        for (int i = 0; i < Data.TABLE_SIZE; ++i)
            for (int j = 0; j < Data.TABLE_SIZE; ++j)
            {
                board[i, j] = FieldState.Empty;
            }

        foreach(int shipLength in _shipSize)
        {
            while (true)
            {
                x = _randomNumberGenerator.Next(Data.TABLE_SIZE);
                y = _randomNumberGenerator.Next(Data.TABLE_SIZE);
                orientation = _randomNumberGenerator.Next(4); // towards: 0 - north, 1 - east, 2 - south, 3 - west

                if (x + (shipLength - 1) * _orientationSpaningFactorX[orientation] < 0
                    || x + (shipLength - 1) * _orientationSpaningFactorX[orientation] >= Data.TABLE_SIZE
                    || y + (shipLength - 1) * _orientationSpaningFactorY[orientation] < 0
                    || y + (shipLength - 1) * _orientationSpaningFactorY[orientation] >= Data.TABLE_SIZE)
                    continue;

                bool allFieldsAreEmpty = true;

                for (int i = 0; i < shipLength; ++i)
                    if (board[x + i * _orientationSpaningFactorX[orientation], y + i * _orientationSpaningFactorY[orientation]] != FieldState.Empty)
                    {
                        allFieldsAreEmpty = false;
                        break;
                    }

                if (!allFieldsAreEmpty)
                    continue;

                // This condition reduces grouping of ships, it checks if there is any ship in neighbour cells 
                if ((x > 0 && board[x - 1, y] == FieldState.Ship)
                    || (x < Data.TABLE_SIZE - 1 && board[x + 1, y] == FieldState.Ship)
                    || (y > 0 && board[x, y - 1] == FieldState.Ship)
                    || (y < Data.TABLE_SIZE - 1 && board[x, y + 1] == FieldState.Ship)
                    || (x > 0 && y > 0 && board[x - 1, y - 1] == FieldState.Ship)
                    || (x > 0 && y < Data.TABLE_SIZE - 1 && board[x - 1, y + 1] == FieldState.Ship)
                    || (x < Data.TABLE_SIZE - 1 && y > 0 && board[x + 1, y - 1] == FieldState.Ship)
                    || (x < Data.TABLE_SIZE - 1 && y < Data.TABLE_SIZE - 1 && board[x + 1, y + 1] == FieldState.Ship))
                    continue;

                List<Coordinate> positionList = [];
                
                for (int i = 0; i < shipLength; ++i)
                {
                    Coordinate coordinate = new()
                    {
                        X = x + i * _orientationSpaningFactorX[orientation],
                        Y = y + i * _orientationSpaningFactorY[orientation]
                    };
                    
                    board[coordinate.X, coordinate.Y] = FieldState.Ship;
                    positionList.Add(coordinate);
                }

                ships[shipIndex++] = new()
                {
                    FieldsUndamaged = shipLength,
                    Size = shipLength,
                    Position = positionList
                };
                
                break;
            }
        }

        return ships;
    }

    private static Coordinate GetFirstEmptyField(FieldState[,] board)
    {
        for (int i = 0; i < Data.TABLE_SIZE; ++i)
            for (int j = 0; j < Data.TABLE_SIZE; ++j)
                if (board[i, j] == FieldState.Empty)
                    return new Coordinate
                    {
                        X = i,
                        Y = j
                    };
        
        return new Coordinate
        {
            X = 0,
            Y = 0
        };
    }
}