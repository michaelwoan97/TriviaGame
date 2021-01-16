/**
 *      FILE            :       MainWindow.xaml.cs
 *      PROJECT         :       Relational Databases Assignment #4 
 *      PROGRAMMER      :       NGHIA NGUYEN 8616831
 *      DESCRIPTION     :       The user interface of Answer Questions Game
 *      FIRST VERSION   :       2020-12-08
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace nnguyen6831_A4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer = new DispatcherTimer();
        GameDatabase gameDatabase = null;
        DataTable StorageQuestion = null;
        int TimeStop = 0;
        int TimeStart = 20;
        private string sPlayerName = null;
        private int Points;
        private int iQuestionNumber;
        private bool isTimeOut = false;
        private int QuestionLimit = 9;

        public MainWindow()
        {
            InitializeComponent();
            gameDatabase = new GameDatabase();
            StorageQuestion = gameDatabase.RetrieveStorageQuestions();
            Timer.Tick += new EventHandler(CountPoints_Timer);
            Timer.Interval = new TimeSpan(0, 0, 1);
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;
            sPlayerName = PlayerName.Text;
           
            // check whether the player name is valid
            if(sPlayerName.IndexOf(' ') != -1)
            {
                ErrorMsg.Text = "Sorry, no spaces in your player name!";
                return;
            }
            ErrorMsg.Text = "";

            if(Int32.TryParse(sPlayerName,out result) == true )
            {
                ErrorMsg.Text = "Sorry, player name is not valid (characters or characters plus numbers no spaces in between)!";
                return;
            }
            ErrorMsg.Text = "";

            if (gameDatabase.CreatePlayerTable(sPlayerName) == false)
            {
                ErrorMsg.Text = "Sorry, your player name is already existed!";
                return;
            }
            ErrorMsg.Text = "";


            Welcome.Visibility = Visibility.Collapsed;
            GamePlay.Visibility = Visibility.Visible;
            WelcomePlayer.Text = sPlayerName;

            // game begins
            iQuestionNumber = 0;
            GameBegin(iQuestionNumber);
            
        }

        private void GameBegin(int iQuestionNumber)
        {
            QuestionNumber.Content = $"Question {iQuestionNumber+1}:";
        
            // Question and potential answers
            Question.Text = $"{StorageQuestion.Rows[iQuestionNumber]["Question"]}";
            Answers.Items.Add($"{StorageQuestion.Rows[iQuestionNumber]["FirstAnswer"]}");
            Answers.Items.Add($"{StorageQuestion.Rows[iQuestionNumber]["SecondAnswer"]}");
            Answers.Items.Add($"{StorageQuestion.Rows[iQuestionNumber]["ThirdAnswer"]}");
            Answers.Items.Add($"{StorageQuestion.Rows[iQuestionNumber]["FourthAnswer"]}");
    
            // set up timer
            Timer.Start();
            TimeStart = 20;
            PointCount.Text = $"{TimeStart}";
        }

        private void CountPoints_Timer(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            // check whether the elapsed time in seconds is greater than or equal to the time need to stop
            TimeStart--;
            if (TimeStart <= TimeStop)
            {
                // stop timer 
                Timer.Stop();
                PointCount.Text = $"{TimeStart} (Time out)";
                PointCount.Foreground = Brushes.Red;

                // record answers
                isTimeOut = true;
                Points = TimeStop; // 0 points because time out
                TimeStart = 20;
                return;
            }
            PointCount.Text = $"{TimeStart}";
            
        }

        private void AnswerSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(Answers.SelectedIndex == -1)
            {
                SelectedErrorMsg.Text = "Please, select an answer and submit!";
                return;
            }
            SelectedErrorMsg.Text = "";
            RecordAnswers();

            // Re-start the timer and stopwatch
            PointCount.Text = "";
            PointCount.Foreground = Brushes.Black;
            Timer.Start();
            TimeStart = 20;
            PointCount.Text = $"{TimeStart}";

            RemovePotentialAnswers();
            /*Go to next question*/
            iQuestionNumber++;
            // check whether the question is at number 10
            if (iQuestionNumber == QuestionLimit)
            {
                SubmitAnswer.Visibility = Visibility.Collapsed;
                CompleteGame.Visibility = Visibility.Visible;
            }
            GameBegin(iQuestionNumber);


        }

        private void RecordAnswers()
        {
            
            Points = CheckAnswers();
            // insert the answer and points
            gameDatabase.RecordPlayerAnswers(sPlayerName, Convert.ToInt32(StorageQuestion.Rows[iQuestionNumber]["QuestionID"]), Answers.SelectedItem.ToString(), Points);
        }

        private int CheckAnswers()
        {
            string[] answerDelim = { "." };
            string[] sCorrectAnswer = null;
            string[] sSelectedAnswer = null;
            string correctAnswer = null;
            string selectedAnswer = null;
            if (TimeStart <= TimeStop)
            {
                Timer.Stop();
                // record answers
                isTimeOut = true;
            }

            if (isTimeOut == true)
            {
                Points = TimeStop; // 0 points because time out
                return Points;
            }

            // split answers to elements
            correctAnswer = StorageQuestion.Rows[iQuestionNumber]["CorrectAnswer"].ToString();
            sCorrectAnswer = correctAnswer.Split(answerDelim, StringSplitOptions.RemoveEmptyEntries);
            selectedAnswer = Answers.SelectedItem.ToString();
            sSelectedAnswer = selectedAnswer.Split(answerDelim, StringSplitOptions.RemoveEmptyEntries);

            /*
             * Index 0 : is the answer
             */
            correctAnswer = sCorrectAnswer[0].ToString().ToLower();
            selectedAnswer = selectedAnswer[0].ToString().ToLower();

            if(correctAnswer != selectedAnswer)
            {
                Points = TimeStop;
                return Points;
            }
            Points = TimeStart;
            return Points;


        }

        private void RemovePotentialAnswers()
        {
          
            for (int i = (Answers.Items.Count - 1); i >= 0; i--)
            {
                Answers.Items.RemoveAt(i);
            }
        }

        private void CompleteGame_Click(object sender, RoutedEventArgs e)
        {
            DataTable PlayerAnswersPoints = null;
            TextBlock totalPoints = new TextBlock();
            int iTotalPlayerPoints = 0;
            double dWidthWelcomePlayer = 500;
            double marginTopBottom = 30;
            double marginTopBotQues = 10;
            ScrollViewer scrollViewer = new ScrollViewer();
            Button getLeaderBoard = new Button();

            RecordAnswers();
            GamePlay.Visibility = Visibility.Collapsed;
            AnswersForQuestions.Visibility = Visibility.Visible;
            AnswersForQuestionsViewer.Visibility = Visibility.Visible;
            StackPanel welcomePlayer = new StackPanel();
            TextBlock helloPlayer = new TextBlock();
            TextBlock answerLabel = new TextBlock();
           

            helloPlayer.Text = sPlayerName;
            helloPlayer.Foreground = Brushes.White;
            helloPlayer.HorizontalAlignment = HorizontalAlignment.Center;
            answerLabel.Text = "Here are the answers: ";
            answerLabel.HorizontalAlignment = HorizontalAlignment.Center;
            answerLabel.Margin = new Thickness(0,marginTopBottom,0,marginTopBottom);
    
            welcomePlayer.Width = dWidthWelcomePlayer;
            welcomePlayer.Background = new SolidColorBrush(Color.FromRgb(41, 128, 185));
            welcomePlayer.Children.Add(helloPlayer);
            AnswersForQuestions.Children.Add(welcomePlayer);
            AnswersForQuestions.Children.Add(answerLabel);
           
            PlayerAnswersPoints = gameDatabase.RetrievePlayerAnswers(sPlayerName);

            // display answers for questions
            for(int i = 0; i < StorageQuestion.Rows.Count; i++)
            {
                StackPanel questionID = new StackPanel();
                StackPanel questionSection = new StackPanel();
                StackPanel potentialAnswers = new StackPanel();
                TextBlock question = new TextBlock();
                TextBlock correctAnswer = new TextBlock();
                TextBlock userAnswer = new TextBlock();
                TextBlock userPoints = new TextBlock();
                Label questionLabel = new Label();
                ListBox pAnswers = new ListBox();
                

                questionLabel.Content = $"Question {StorageQuestion.Rows[i]["QuestionID"]}:";
                question.Text = $"{StorageQuestion.Rows[i]["Question"]}";

                // potential answers
                pAnswers.BorderThickness = new Thickness(0);
                pAnswers.Items.Add($"{StorageQuestion.Rows[i]["FirstAnswer"]}");
                pAnswers.Items.Add($"{StorageQuestion.Rows[i]["SecondAnswer"]}");
                pAnswers.Items.Add($"{StorageQuestion.Rows[i]["ThirdAnswer"]}");
                pAnswers.Items.Add($"{StorageQuestion.Rows[i]["FourthAnswer"]}");
                pAnswers.IsEnabled = false;

                userAnswer.Text = $"Your answer is: {PlayerAnswersPoints.Rows[i]["Answer"]}";
                correctAnswer.Text = $"The correct answer is: {StorageQuestion.Rows[i]["CorrectAnswer"]}";
                userPoints.Text = $"You got: {PlayerAnswersPoints.Rows[i]["Points"]}/20 Points";

                // question section
                questionSection.Margin = new Thickness(marginTopBottom, 0, marginTopBottom, 0);
                questionSection.Children.Add(question);
                questionSection.Children.Add(pAnswers);
                questionSection.Children.Add(userAnswer);
                questionSection.Children.Add(correctAnswer);
                questionSection.Children.Add(userPoints);
               
                // questionID section
                questionID.Name = $"QuestionNumber{StorageQuestion.Rows[i]["QuestionID"]}";
                questionID.HorizontalAlignment = HorizontalAlignment.Center;
                questionID.Width = dWidthWelcomePlayer;
                questionID.Margin = new Thickness(0, marginTopBotQues, 0, marginTopBotQues);
                questionID.Children.Add(questionLabel);
                questionID.Children.Add(questionSection);
                
                AnswersForQuestions.Children.Add(questionID);
            }

            // count the total points
            iTotalPlayerPoints = gameDatabase.TotalPoints(sPlayerName);
            totalPoints.Text = $"Your total points are: {iTotalPlayerPoints}";
            totalPoints.HorizontalAlignment = HorizontalAlignment.Center;
            totalPoints.Margin = new Thickness(0, marginTopBotQues, 0, marginTopBotQues);
          
            AnswersForQuestions.Children.Add(totalPoints);

            getLeaderBoard.Content = "Continue";
            getLeaderBoard.HorizontalAlignment = HorizontalAlignment.Center;
            getLeaderBoard.Name = "GetLeaderboard";
            getLeaderBoard.Click += RetrieveLeaderBoard;
            getLeaderBoard.Margin = new Thickness(0, marginTopBotQues, 0, marginTopBotQues);

            AnswersForQuestions.Children.Add(getLeaderBoard);

            gameDatabase.RecordPlayerPoints(sPlayerName, iTotalPlayerPoints);

        }

        private void RetrieveLeaderBoard(object sender, RoutedEventArgs e)
        {
            DataTable leaderBoard = null;
            TextBlock player = new TextBlock();
            Button close = new Button();
            double marginTopBotQues = 10;
            int iTopFive = 5;

            AnswersForQuestions.Visibility = Visibility.Collapsed;
            AnswersForQuestionsViewer.Visibility = Visibility.Collapsed;
            LeaderBoard.Visibility = Visibility.Visible;

            leaderBoard = gameDatabase.GameLeaderBoard();
           

            for (int i = 0; i< leaderBoard.Rows.Count;i++)
            {
                if(i == iTopFive)
                {
                    break;
                }
                player = new TextBlock();
                player.Text = $"{i + 1}. {leaderBoard.Rows[i]["PlayerName"]} Points: {leaderBoard.Rows[i]["Points"]}";
                player.HorizontalAlignment = HorizontalAlignment.Center;
                PlayersLeaderBoard.Children.Add(player);
            }

            close.Content = "Close";
            close.HorizontalAlignment = HorizontalAlignment.Center;
            close.Margin = new Thickness(0, marginTopBotQues, 0, marginTopBotQues);
            close.Click += GameEnd_Click;

            LeaderBoard.Children.Add(close);

        }

        private void GameEnd_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
