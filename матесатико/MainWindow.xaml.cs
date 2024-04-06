using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace матесатико
{
    public partial class MainWindow : Window
    {
        private readonly Random random = new Random();
        private List<int> deck;
        private readonly List<Button> playerCells = new List<Button>();
        private readonly List<Button> botCells = new List<Button>();
        private int playerScore = 0;
        private int botScore = 0;
        private bool isPlayerTurn = true;
        private int currentCard;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            InitializeCells();
            GenerateNextCard();
        }

        private void InitializeGame()
        {
            deck = GenerateDeck();
        }

        private List<int> GenerateDeck()
        {
            var cardList = new List<int>();
            for (int i = 1; i <= 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    cardList.Add(i);
                }
            }
            return cardList.OrderBy(x => random.Next()).ToList();
        }

        private void InitializeCells()
        {
            playerCells.AddRange(new[] { cell00, cell01, cell02, cell03, cell04,
                                          cell10, cell11, cell12, cell13, cell14,
                                          cell20, cell21, cell22, cell23, cell24,
                                          cell30, cell31, cell32, cell33, cell34,
                                          cell40, cell41, cell42, cell43, cell44 });

            botCells.AddRange(new[] { botCell00, botCell01, botCell02, botCell03, botCell04,
                                      botCell10, botCell11, botCell12, botCell13, botCell14,
                                      botCell20, botCell21, botCell22, botCell23, botCell24,
                                      botCell30, botCell31, botCell32, botCell33, botCell34,
                                      botCell40, botCell41, botCell42, botCell43, botCell44 });
        }

        private void GenerateNextCard()
        {
            if (deck.Any())
            {
                currentCard = deck.First();
                deck.RemoveAt(0);
                deckLabel.Content = "Карта: " + currentCard;
            }
            else
            {
                FinishGame();
            }
        }

        private void DisplayCard(int cellIndex)
        {
            if (isPlayerTurn)
            {
                Button cell = playerCells[cellIndex];
                if (cell.Content == null)
                {
                    cell.Content = currentCard.ToString();
                    cell.IsEnabled = false;
                    playerScore += CalculatePoints(playerCells, currentCard);
                    playerScoreLabel.Content = "Очки игрока: " + playerScore;
                    isPlayerTurn = false;
                    BotMove();
                }
            }

            if (IsBoardFull())
            {
                FinishGame();
            }
        }

        private void BotMove()
        {
            int botCellIndex = random.Next(botCells.Count);
            Button botCell = botCells[botCellIndex];
            if (botCell.Content == null)
            {
                botCell.Content = currentCard.ToString();
                botCell.IsEnabled = false;
                botScore += CalculatePoints(botCells, currentCard);
                botScoreLabel.Content = "Очки бота: " + botScore;
                isPlayerTurn = true;
                GenerateNextCard();
            }
            else
            {
                // Если клетка уже занята, бот делает ход снова
                BotMove();
            }
        }

        private bool IsBoardFull()
        {
            foreach (Button cell in playerCells.Concat(botCells))
            {
                if (cell.Content == null)
                {
                    return false;
                }
            }
            return true;
        }

        private void FinishGame()
        {
            string winner = "";
            int winnerScore = Math.Max(playerScore, botScore);

            if (playerScore > botScore)
            {
                winner = "Игрок";
            }
            else if (playerScore < botScore)
            {
                winner = "Бот";
            }
            else
            {
                winner = "It's a tie!";
            }

            MessageBox.Show($"{winner} побеждает с {winnerScore} очками!", "Игра окончена", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int cellIndex = playerCells.IndexOf(button);
            DisplayCard(cellIndex);
        }

        private int CalculatePoints(List<Button> cells, int card)
        {
            int points = 0;

            // Сначала проверяем комбинации в рядах
            for (int row = 0; row < 5; row++)
            {
                int count = 0;
                for (int col = 0; col < 5; col++)
                {
                    Button cell = cells[row * 5 + col];
                    if (cell.Content?.ToString() == card.ToString())
                    {
                        count++;
                    }
                }
                points += CalculatePointsForCount(count);
            }

            // Затем проверяем комбинации в столбцах
            for (int col = 0; col < 5; col++)
            {
                int count = 0;
                for (int row = 0; row < 5; row++)
                {
                    Button cell = cells[row * 5 + col];
                    if (cell.Content?.ToString() == card.ToString())
                    {
                        count++;
                    }
                }
                points += CalculatePointsForCount(count);
            }

            // Наконец, проверяем комбинации по диагоналям
            int diagonalCount1 = 0;
            int diagonalCount2 = 0;
            for (int i = 0; i < 5; i++)
            {
                Button cell1 = cells[i * 5 + i]; // Главная диагональ
                Button cell2 = cells[i * 5 + (4 - i)]; // Побочная диагональ
                if (cell1.Content?.ToString() == card.ToString())
                {
                    diagonalCount1++;
                }
                if (cell2.Content?.ToString() == card.ToString())
                {
                    diagonalCount2++;
                }
            }
            points += CalculatePointsForCount(diagonalCount1);
            points += CalculatePointsForCount(diagonalCount2);

            return points;
        }

        private int CalculatePointsForCount(int count)
        {
            switch (count)
            {
                case 2: return 10;
                case 3: return 40;
                case 4: return 160;
                case 5: return 50;
                default: return 0;
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            RestartGame();
        }

        private void RestartGame()
        {
            // Сброс игрового состояния
            playerScore = 0;
            botScore = 0;
            isPlayerTurn = true;
            deck = GenerateDeck();
            // Очистка содержимого ячеек игрока и бота
            ClearCells(playerCells);
            ClearCells(botCells);
            // Обновление меток счета
            playerScoreLabel.Content = "Очки игрока: 0";
            botScoreLabel.Content = "Очки бота: 0";
            // Начать игру заново
            GenerateNextCard();
        }

        private void ClearCells(List<Button> cells)
        {
            foreach (var cell in cells)
            {
                cell.Content = null;
                cell.IsEnabled = true;
            }
        }

    }
}
