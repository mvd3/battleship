using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Battleship.Platform;

namespace Battleship;

public partial class MainWindow : Window
{
    private Platform.Platform _platform;
    public MainWindow()
    {
        InitializeComponent();

        _platform = new();

        DrawBoards();
    }

        private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void DrawBoards()
    {
        var _leftBoardGrid = this.FindControl<Grid>("LeftBoard");
        var _rightBoardGrid = this.FindControl<Grid>("RightBoard");

        if (_leftBoardGrid == null || _rightBoardGrid == null)
            return;

        for (int i = 0; i < Data.NUMBER_OF_ROWS; ++i)
        {
            _leftBoardGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            _leftBoardGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            
            _rightBoardGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            _rightBoardGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        AddFieldsToBoard(_leftBoardGrid);
        AddFieldsToBoard(_rightBoardGrid, false);
    }

    private void AddFieldsToBoard(Grid grid, bool isLeftBoard = true)
    {
        for (int i = 0; i < Data.NUMBER_OF_ROWS; ++i)
        {
            for (int j = 0; j < Data.NUMBER_OF_COLUMNS; ++j)
            {
                var button = new Button
                {
                    Content = $"{i},{j}",
                    Width = 50,
                    Height = 50,
                    Background = new SolidColorBrush(Color.Parse(Data.REGULAR_FIELD_BACKGROUND_COLOR)),
                    BorderBrush = new SolidColorBrush(Color.Parse(Data.BORDER_COLOR_HEX)),
                    BorderThickness = new(1)
                };

                Grid.SetRow(button, i);
                Grid.SetColumn(button, j);
                grid.Children.Add(button);

                _platform.enlistCreatedButton(button, i, j, isLeftBoard);
            }
        }
    }
}