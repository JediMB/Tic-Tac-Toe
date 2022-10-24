using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tic_Tac_Toe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region PrivateMembers
        private readonly MarkType[] marks = new MarkType[9];
        private bool player1Active;
        private bool gameEnded;
        #endregion

        #region PublicMembers
        public enum MarkType { Free, Nought, Cross }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            NewGame();
        }

        private void NewGame()
        {
            for (int i = 0; i < marks?.Length; i++)
                marks[i] = MarkType.Free;

            GameArea.Style = this.FindResource("gridDefaultStyle") as Style ?? new Style();
            headerText.Text = "Welcome to Tic-Tac-Toe!";
            player1Active = true;
            gameEnded = false;

            GameArea.Children.Cast<dynamic>().ToList().ForEach(child =>
            {
                if (child is Button button)
                {
                    button.Content = string.Empty;
                    button.Background = Brushes.White;
                    button.Foreground = Brushes.Black;
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameEnded) { NewGame(); return; }

            if (sender is Button button)
            {
                int index = (Grid.GetColumn(button) - 1) + (Grid.GetRow(button) - 1) * 3;

                if (marks[index] != MarkType.Free)
                    return;

                marks[index]            = (player1Active ? MarkType.Cross : MarkType.Nought);
                button.Foreground       = (player1Active ? Brushes.Blue : Brushes.Red);
                button.Content          = (player1Active ? "X" : "O");

                (int, int, int) winningIndices = CheckResults(index);
                if (winningIndices != (0, 0, 0))
                {
                    gameEnded = true;

                    if (winningIndices == (-1, -1, -1))
                    {
                        GameArea.Style = this.FindResource("gridTieStyle") as Style ?? new Style();
                        headerText.Text = $"It's a tie!";
                     
                        for (int i = 0; i < GameArea.Children.Count; i++)
                            if (GameArea.Children[i] is Button tieButton)
                            {
                                tieButton.Background = Brushes.Gray;
                                tieButton.Foreground = Brushes.Black;
                            }

                        return;
                    }

                    if (winningIndices == (9, 9, 9))
                    {
                        GameArea.Style = this.FindResource($"gridP{(player1Active ? "1" : "2")}WinStyle") as Style ?? new Style();
                        headerText.Text = $"Player {(player1Active ? "1" : "2")} wins!";

                        btnTopLeft.Background = btnTopRight.Background = btnMidCenter.Background =
                            btnMidCenter.Background = btnBottomLeft.Background = btnBottomRight.Background = Brushes.LimeGreen;

                        btnTopLeft.Foreground = btnTopRight.Foreground = btnMidCenter.Foreground =
                            btnMidCenter.Foreground = btnBottomLeft.Foreground = btnBottomRight.Foreground = Brushes.White;

                        return;
                    }
                    else
                    {
                        GameArea.Style = this.FindResource($"gridP{(player1Active ? "1" : "2")}WinStyle") as Style ?? new Style();
                        headerText.Text = $"Player {(player1Active ? "1" : "2")} wins!";

                        for (int i = 0; i < 9; i++)
                        {
                            if ((i == winningIndices.Item1 || i == winningIndices.Item2 || i == winningIndices.Item3) && GameArea.Children[i] is Button winButton)
                            {
                                winButton.Background = Brushes.LimeGreen;
                                winButton.Foreground = Brushes.White;
                            }
                        }

                        return;
                    }
                }

                player1Active = !player1Active;
            }
        }

        private (int, int, int) CheckResults(int index)
        {
            if (index / 3 == 0) // row 0
                if ((marks[0] & marks[1] & marks[2]) == marks[index])
                    return (0, 1, 2);

            if (index / 3 == 1) // row 1
                if ((marks[3] & marks[4] & marks[5]) == marks[index])
                    return (3, 4, 5);

            if (index / 3 == 2) // row 2
                if ((marks[6] & marks[7] & marks[8]) == marks[index])
                    return (6, 7, 8);

            if (index % 2 == 0) // diagonal
            {
                if (index == 4 && (marks[0] & marks[2] & marks[4] & marks[6] & marks[8]) == marks[index])
                    return (9, 9, 9);

                if (index % 4 == 0) // TL to BR
                    if ((marks[0] & marks[4] & marks[8]) == marks[index])
                        return (0, 4, 8);

                // TR to BL
                if ((marks[2] & marks[4] & marks[6]) == marks[index])
                    return (2, 4, 6);
            }

            if (index % 3 == 0) // col 0
                if ((marks[0] & marks[3] & marks[6]) == marks[index])
                    return (0, 3, 6);

            if (index % 3 == 1) // col 1
                if ((marks[1] & marks[4] & marks[7]) == marks[index])
                    return (1, 4, 7);

            if (index % 3 == 2) // col 2
                if ((marks[2] & marks[5] & marks[8]) == marks[index])
                    return (2, 5, 8);

            foreach (MarkType mark in marks)
                if (mark == MarkType.Free)
                    return (0, 0, 0);

            return (-1, -1, -1);
        }
    }
}
