using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json; // Не забудьте добавить ссылку на Newtonsoft.Json

namespace Task8
{
    public partial class MainWindow : Window
    {
        private string targetWord;
        private char[] guessedWord;
        private int errorCount = 0;
        private const int MaxErrors = 6;

        private List<string> words; // Список слов
        private string animationFolder = "Animations"; // Папка с анимациями
        private DispatcherTimer animationTimer;
        private int currentFrame;
        private string[] currentAnimationFrames;

        public MainWindow()
        {
            InitializeComponent();
            LoadWordsFromJson("russian_words_lower.json"); // Загружаем слова из JSON-файла
            StartGame();
            InitializeTimer();
        }

        private void LoadWordsFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                // Читаем и десериализуем JSON
                var json = File.ReadAllText(filePath);
                var wordList = JsonConvert.DeserializeObject<WordList>(json);
                words = wordList.Words.ToList();
            }
            else
            {
                MessageBox.Show($"Файл '{filePath}' не найден!");
                words = new List<string>(); // Если файл не найден, инициализируем пустой список
            }
        }

        private void StartGame()
        {
            Random random = new Random();
            if (words.Count == 0)
            {
                MessageBox.Show("Нет доступных слов для игры.");
                return;
            }
            targetWord = words[random.Next(words.Count)];
            guessedWord = new string('_', targetWord.Length).ToCharArray();
            UpdateWordDisplay();
            errorCount = 0;
            GallowsImage.Source = new BitmapImage(new Uri("Animations/1/Sprite-0001.png", UriKind.Relative));
        }

        private void InitializeTimer()
        {
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(100); // Интервал между кадрами
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void UpdateWordDisplay()
        {
            WordDisplay.Text = string.Join(" ", guessedWord);
        }

        private void GuessButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LetterInput.Text)) return;

            char guessedLetter = LetterInput.Text.ToLower()[0];
            LetterInput.Clear();

            if (targetWord.Contains(guessedLetter))
            {
                for (int i = 0; i < targetWord.Length; i++)
                {
                    if (targetWord[i] == guessedLetter)
                    {
                        guessedWord[i] = guessedLetter;
                    }
                }
                UpdateWordDisplay();

                if (!new string(guessedWord).Contains('_'))
                {
                    MessageBox.Show("Вы выиграли!");
                    StartGame();
                }
            }
            else
            {
                errorCount++;
                LoadAnimationFrames(errorCount);
                animationTimer.Start();

                if (errorCount >= MaxErrors)
                {
                    MessageBox.Show($"Слово не отгадано. Было загадано: '{targetWord}'");
                    StartGame();
                }
            }
        }

        private void LoadAnimationFrames(int partNumber)
        {
            // Загружаем все файлы из папки с текущим номером стадии
            string folderPath = Path.Combine(animationFolder, partNumber.ToString());
            if (Directory.Exists(folderPath))
            {
                currentAnimationFrames = Directory.GetFiles(folderPath, "*.png");
                Array.Sort(currentAnimationFrames); // Сортируем файлы по имени
                currentFrame = 0;
            }
            else
            {
                MessageBox.Show($"Animation folder for stage {partNumber} not found!");
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (currentFrame < currentAnimationFrames.Length)
            {
                GallowsImage.Source = new BitmapImage(new Uri(currentAnimationFrames[currentFrame], UriKind.Relative));
                currentFrame++;
            }
            else
            {
                animationTimer.Stop(); // Останавливаем таймер после показа всех кадров
            }
        }
    }

    public class WordList
    {
        public string[] Words { get; set; }
    }
}
