using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;


namespace Task8
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string targetWord;
        private char[] guessedWord;
        private int errorCount = 0;
        private const int MaxErrors = 6;

        private string[] words = {
    "акушёр", "аматёр", "абазия", "абляут", "абулия", "аварея", "авария", "агамия",
    "агония", "адская", "адуляр", "азалия", "азеять", "акация", "акулья", "алалуя",
    "альянс", "алякиш", "аляска", "аляски", "ананья", "анемия", "аномия", "анурия",
    "апатия", "бабёха", "балазё", "балдёж", "Башнёр", "белёсо", "берёга", "берёза",
    "блёкло", "боксёр", "бракёр", "бретёр", "бритьё", "брусьё", "бабатя", "бабняк",
    "бабуля", "бабуня", "бабуся", "бабьяк", "багуля", "бадьян", "бадяга", "баклея",
    "бакуня", "баляба", "вахтёр", "вдвоём", "вдомёк", "вдёжка", "взачёт", "винтёр",
    "вкрёпа", "вмётка", "внетьё", "водоём", "возлёт", "вояжёр", "вперёд", "впёхом",
    "враньё", "втроём", "выдёма", "высёха", "вёртко", "вальмя", "вальня", "вальян",
    "валява", "валять", "валяха", "галдёж", "гнильё", "гнутьё", "гольём", "гостёк",
    "грабёж", "гравёр", "гримёр", "гулёна", "гальян", "гальяс", "гаолян", "гарпия",
    "гачуля", "гиляки", "главня", "глупая", "глухня", "глушня", "глядка", "глядун",
    "глянец", "гмыдня", "гнедая", "гнетья", "далёко", "днёвка", "дождём", "долбёж",
    "доёнка", "драньё", "дрябьё", "дрёмно", "дублёр", "дурёха", "дёготь", "дёшево",
    "давяга", "данная", "даться", "дафния", "даяние", "двойня", "дворня", "двояки",
    "двояко", "двуряд", "девоня", "девуля", "девуня", "едунья", "ельняк", "есться",
    "единою", "еденье", "ездить", "езжать", "езовье", "екнуть", "елмань", "елчать",
    "елчить", "ельник", "ельшин", "ембель", "емлить", "енбель", "ербень", "ердань",
    "ерзать", "ерпыль", "ершить", "еханье", "евоный", "единый"
};
        private string animationFolder = "Animations"; // Папка с анимациями
        private DispatcherTimer animationTimer;
        private int currentFrame;
        private string[] currentAnimationFrames;

        public MainWindow()
        {
            InitializeComponent();
            StartGame();
            InitializeTimer();
        }

        private void StartGame()
        {
            Random random = new Random();
            targetWord = words[random.Next(words.Length)];
            guessedWord = new string('_', targetWord.Length).ToCharArray();
            UpdateWordDisplay();
            errorCount = 0;
            GallowsImage.Source = new BitmapImage(new Uri("Animations/1/Sprite-0001.png", UriKind.Relative)); ;
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
            string folderPath = System.IO.Path.Combine(animationFolder, partNumber.ToString());
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
}
