using System;
using Avalonia.Controls;
using Battleship.Platform.Helper;

namespace Battleship.Platform;

public class Platform
{
    private Data.FieldState[,] _leftBoard, _rightBoard;
    private Ship[] _leftPlayerShips, _rightPlayerShips;
    private Random _randomNumberGenerator;
    
    public Platform()
    {
        _leftBoard = new Data.FieldState[Data.TABLE_SIZE, Data.TABLE_SIZE];
        _rightBoard = new Data.FieldState[Data.TABLE_SIZE, Data.TABLE_SIZE];
        _leftPlayerShips = new Ship[Data.NUMBER_OF_SHIPS];
        _rightPlayerShips = new Ship[Data.NUMBER_OF_SHIPS];
        _randomNumberGenerator = new();
    }

    public void StartGame(Data.GameType gameType)
    {
        for (int i = 0; i < Data.TABLE_SIZE; ++i)
            for (int j = 0; j < Data.TABLE_SIZE; ++j)
            {
                _leftBoard[i, j] = Data.FieldState.Empty;
                _rightBoard[i, j] = Data.FieldState.Empty;
            }

        _leftPlayerShips = new Ship[Data.NUMBER_OF_SHIPS];
        _rightPlayerShips = new Ship[Data.NUMBER_OF_SHIPS];
    }

    public string GetLeftBotName()
    {
        return Data.LEFT_DEFAULT_PLAYER;
    }

    public string GetRightBotName()
    {
        return Data.RIGHT_DEFAULT_PLAYER;
    }
}