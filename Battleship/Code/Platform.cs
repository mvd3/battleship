using Avalonia.Controls;

namespace Battleship.Platform;

public class Platform
{
    private Button[,] _leftBoardButtons, _rightBoardButtons;
    public Platform()
    {
        _leftBoardButtons = new Button[Data.NUMBER_OF_ROWS, Data.NUMBER_OF_COLUMNS];
        _rightBoardButtons = new Button[Data.NUMBER_OF_ROWS, Data.NUMBER_OF_COLUMNS];
    }

    public void enlistCreatedButton(Button button, int row, int column, bool isLeftBoard = true)
    {
        if (isLeftBoard)
            _leftBoardButtons[row, column] = button;
        else
            _rightBoardButtons[row, column] = button;
    }
}