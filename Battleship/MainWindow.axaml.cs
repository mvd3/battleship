using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Battleship.Platform;

namespace Battleship;

public partial class MainWindow : Window
{
    private Platform.Platform _platform;
    private Grid? _leftBoardGrid, _rightBoardGrid;
    private TextBlock? _leftPlayerInfo, _rightPlayerInfo;
    private Button[,] _leftBoardButtons, _rightBoardButtons;
    
    public MainWindow()
    {
        InitializeComponent();

        _leftBoardButtons = new Button[Data.TABLE_SIZE, Data.TABLE_SIZE];
        _rightBoardButtons = new Button[Data.TABLE_SIZE, Data.TABLE_SIZE];

        _platform = new();

        DrawBoards();
    }

        private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void DrawBoards()
    {
        _leftBoardGrid = this.FindControl<Grid>("LeftBoard");
        _rightBoardGrid = this.FindControl<Grid>("RightBoard");
        _leftPlayerInfo = this.FindControl<TextBlock>("LeftPlayerInfo");
        _rightPlayerInfo = this.FindControl<TextBlock>("RightPlayerInfo");

        if (_leftBoardGrid == null 
            || _rightBoardGrid == null
            || _leftPlayerInfo == null
            || _rightPlayerInfo == null)
            return;

        AddFieldsToBoard(_leftBoardGrid, _leftBoardButtons);
        AddFieldsToBoard(_rightBoardGrid, _rightBoardButtons);

        _leftPlayerInfo.Text = _platform.GetLeftBotName();
        _rightPlayerInfo.Text = _platform.GetRightBotName();

        _leftBoardGrid.IsEnabled = false;
        _rightBoardGrid.IsEnabled = false;
    }

    private static void AddFieldsToBoard(Grid grid, Button[,] buttons)
    {
        for (int i = 0; i < Data.TABLE_SIZE; ++i)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        for (int i = 0; i < Data.TABLE_SIZE; ++i)
        {
            for (int j = 0; j < Data.TABLE_SIZE; ++j)
            {
                var button = new Button
                {
                    Width = 50,
                    Height = 50,
                    Background = new SolidColorBrush(Color.Parse(Data.REGULAR_FIELD_BACKGROUND_COLOR_HEX)),
                    BorderBrush = new SolidColorBrush(Color.Parse(Data.BORDER_COLOR_HEX)),
                    BorderThickness = new(1)
                };

                Grid.SetRow(button, i);
                Grid.SetColumn(button, j);
                grid.Children.Add(button);

                if (i == 5 && j == 5)
                {
                    button.Classes.Clear();
                    button.Classes.Add("NoHoverClickEffect");
                    button.Background = new SolidColorBrush(Color.Parse(Data.DESTROYED_SHIP_BACKGROUND_COLOR_HEX));
                }
                if (i == 3 && j == 3)
                {
                    button.Classes.Clear();
                    button.Classes.Add("NoHoverClickEffect");
                    button.Background = new SolidColorBrush(Color.Parse(Data.DAMAGED_SHIP_BACKGROUND_COLOR_HEX));
                }
                if (i == 7 && j == 7)
                {
                    button.Classes.Clear();
                    button.Classes.Add("NoHoverClickEffect");
                    button.Background = new SolidColorBrush(Color.Parse(Data.MISS_BACKGROUND_COLOR_HEX));
                }
                if (i == 4 && j > 4 && j < 8)
                {
                    button.Classes.Clear();
                    button.Classes.Add("NoHoverClickEffect");
                    button.Background = new SolidColorBrush(Color.Parse(Data.SHIP_BACKGROUND_COLOR_HEX));
                }

                buttons[i, j] = button;
            }
        }
    }
 
    public void StartGame(object sender, RoutedEventArgs args)
    {
        if (_leftBoardGrid == null
            || _rightBoardGrid == null
            || _leftPlayerInfo == null
            || _rightPlayerInfo == null
            || args.Source == null)
            return;
        
        Button button = (Button) args.Source;
        Data.GameType mode = (Data.GameType) int.Parse(button.Tag?.ToString() ?? "-1");

        _platform.StartGame(mode);
        ClearField(_leftBoardButtons);
        ClearField(_rightBoardButtons);

        switch (mode)
        {
            case Data.GameType.PlayerVersusLeftBot:
            case Data.GameType.PlayerVersusRightBot:
            {
                _rightBoardGrid.IsEnabled = true;
                _leftPlayerInfo.Text = Data.HUMAN_PLAYER;
                _rightPlayerInfo.Text = mode == Data.GameType.PlayerVersusLeftBot ? _platform.GetLeftBotName() : _platform.GetRightBotName();
                break;
            }
            case Data.GameType.BotVersusBot:
            {
                _leftBoardGrid.IsEnabled = false;
                _rightBoardGrid.IsEnabled = false;
                _leftPlayerInfo.Text = _platform.GetLeftBotName();
                _rightPlayerInfo.Text = _platform.GetRightBotName();
                break;
            }
            default: break;
        };
    }

    private static void ClearField(Button[,] buttons)
    {
        foreach (Button button in buttons)
        {
            button.Classes.Clear();
            button.Background = new SolidColorBrush(Color.Parse(Data.REGULAR_FIELD_BACKGROUND_COLOR_HEX));
        }
    }
}