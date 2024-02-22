using System;
using System.Collections.Generic;
using Battleship.Platform.Bot;
using Battleship.Platform.Helper;

namespace Battleship.Platform;

public class Platform
{
    private Field[,] _leftBoard, _rightBoard;
    private Ship[] _leftPlayerShips, _rightPlayerShips;
    private Random _randomNumberGenerator;
    private NBot _leftBot, _rightBot;
    private readonly int[] _shipSize;
    private readonly int[] _orientationSpaningFactorX;
    private readonly int[] _orientationSpaningFactorY;
    public bool _currentlyPlaying;

    public Platform()
    {
        _leftBoard = new Field[Data.TABLE_SIZE, Data.TABLE_SIZE];
        _rightBoard = new Field[Data.TABLE_SIZE, Data.TABLE_SIZE];
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
                _leftBoard[i, j] = new()
                {
                    State = FieldState.Empty,
                    IsAvailable = true
                };
                _rightBoard[i, j] = new()
                {
                    State = FieldState.Empty,
                    IsAvailable = true
                };
            }

        _leftPlayerShips = ArrangeShipsOnBoard(ref _leftBoard);
        _rightPlayerShips = ArrangeShipsOnBoard(ref _rightBoard);

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
        Coordinate coordinate = GetFirstEmptyField(isLeftBot && botVersusBot ? _leftBoard : _rightBoard);
        FieldState fieldState = FieldState.Missed;

        coordinate = isLeftBot 
            ? _leftBot.NextPosition(botVersusBot ? _rightBoard : _leftBoard, botVersusBot ? _rightPlayerShips : _leftPlayerShips) 
            : _rightBot.NextPosition(_leftBoard, _leftPlayerShips);

        return (coordinate, fieldState);
    }
    
    public Coordinate[] GetAllShipPositions(bool defaultLeft = true)
    {
        List<Coordinate> shipPositions = [];
        
        foreach(Ship ship in defaultLeft ? _leftPlayerShips : _rightPlayerShips)
            shipPositions.AddRange(ship.Position);
        
        return [.. shipPositions];
    }

    private Ship[] ArrangeShipsOnBoard(ref Field[,] board)
    {
        Ship[] ships = new Ship[Data.NUMBER_OF_SHIPS];
        int x, y, orientation;
        int shipIndex = 0;

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
                    if (board[x + i * _orientationSpaningFactorX[orientation], y + i * _orientationSpaningFactorY[orientation]].State != FieldState.Empty)
                    {
                        allFieldsAreEmpty = false;
                        break;
                    }

                if (!allFieldsAreEmpty)
                    continue;

                // This condition reduces grouping of ships, it checks if there is any ship in neighbour cells 
                if ((x > 0 && board[x - 1, y].State == FieldState.Ship)
                    || (x < Data.TABLE_SIZE - 1 && board[x + 1, y].State == FieldState.Ship)
                    || (y > 0 && board[x, y - 1].State == FieldState.Ship)
                    || (y < Data.TABLE_SIZE - 1 && board[x, y + 1].State == FieldState.Ship)
                    || (x > 0 && y > 0 && board[x - 1, y - 1].State == FieldState.Ship)
                    || (x > 0 && y < Data.TABLE_SIZE - 1 && board[x - 1, y + 1].State == FieldState.Ship)
                    || (x < Data.TABLE_SIZE - 1 && y > 0 && board[x + 1, y - 1].State == FieldState.Ship)
                    || (x < Data.TABLE_SIZE - 1 && y < Data.TABLE_SIZE - 1 && board[x + 1, y + 1].State == FieldState.Ship))
                    continue;

                List<Coordinate> positionList = [];
                
                for (int i = 0; i < shipLength; ++i)
                {
                    Coordinate coordinate = new()
                    {
                        X = x + i * _orientationSpaningFactorX[orientation],
                        Y = y + i * _orientationSpaningFactorY[orientation]
                    };
                    
                    board[coordinate.X, coordinate.Y].State = FieldState.Ship;
                    positionList.Add(coordinate);
                }

                ships[shipIndex++] = new()
                {
                    IsDestroyed = false,
                    Size = shipLength,
                    Position = positionList
                };
                
                break;
            }
        }

        return ships;
    }

    private static Coordinate GetFirstEmptyField(Field[,] board)
    {
        for (int i = 0; i < Data.TABLE_SIZE; ++i)
            for (int j = 0; j < Data.TABLE_SIZE; ++j)
                if (board[i, j].IsAvailable)
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