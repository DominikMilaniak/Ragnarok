using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using SQLite;
using System.Threading;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Timers;
using System.Windows.Threading;

namespace GraphicsGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        private MediaPlayer mediaPlayer = new MediaPlayer();

        string installPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        SpeechSynthesizer speech = new SpeechSynthesizer();

        Choices list = new Choices();

        int HP = 200;

        private DispatcherTimer dispatcherTimer;
        private DispatcherTimer spellTimer;


        public MainWindow()
        {
            InitializeComponent();



            Uri GroundUri = new Uri(installPath + @"\assets\ground1.png", UriKind.Absolute);
            Uri MainPlayerUri = new Uri(installPath + @"\assets\animations/MainPlayer_Standby.png", UriKind.Absolute);
            Uri NPCUri = new Uri(installPath + @"\assets\char2.png", UriKind.Absolute);
            Uri TreeUri = new Uri(installPath + @"\assets\Tree1.png", UriKind.Absolute);
            Uri DialogUri1 = new Uri(installPath + @"\assets\dialogbox2.png", UriKind.Absolute);
            Uri DialogUri2 = new Uri(installPath + @"\assets\dialogbox.png", UriKind.Absolute);
            Uri DaggerUri = new Uri(installPath + @"\assets\items\Dagger32.png", UriKind.Absolute);


            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(installPath + @"\assets\rain.gif");
            image.EndInit();
            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Rain, image);


            BitmapImage imageBitmap = new BitmapImage(GroundUri);
            BitmapImage imageBitmap2 = new BitmapImage(MainPlayerUri);
            BitmapImage imageBitmap3 = new BitmapImage(NPCUri);
            BitmapImage imageBitmap4 = new BitmapImage(TreeUri);
            BitmapImage imageBitmap6 = new BitmapImage(DialogUri1);
            BitmapImage imageBitmap7 = new BitmapImage(DialogUri2);
            BitmapImage imageBitmap8 = new BitmapImage(DaggerUri);


            MainPlayer.Source = imageBitmap2;
            Ground.Source = imageBitmap;
            NPC.Source = imageBitmap3;
            Tree.Source = imageBitmap4;
            Tree2.Source = imageBitmap4;
            Tree3.Source = imageBitmap4;
            Tree4.Source = imageBitmap4;
            Tree5.Source = imageBitmap4;
            Tree6.Source = imageBitmap4;
            DialogBoxBG.Source = imageBitmap6;
            DialogBox2BG.Source = imageBitmap7;
            Dagger.Source = imageBitmap8;

            HPBar.Value = HP;
            HPLabel.Content = HP.ToString();


        }

        //Hudba
        System.Media.SoundPlayer player = new System.Media.SoundPlayer(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\assets\RainMusic.wav");


        public void LoadDB()
        {
            var databasePath = installPath + @"\Database.db";
            var db = new SQLiteConnection(databasePath);
            var conn = new SQLiteConnection(databasePath);
            var query = conn.Table<Character>();

        }


        bool isPickable;


        public void CollisionSystem(UIElement Item, string Type)
        {
            Rect Collider1 = new Rect(Canvas.GetLeft(MainPlayer), Canvas.GetTop(MainPlayer), MainPlayer.Width, MainPlayer.Height);
            Rect Collider2 = new Rect(Canvas.GetLeft(Item), Canvas.GetTop(Item), NPC.Width, NPC.Height);


            if (Collider1.IntersectsWith(Collider2))
            {

                if (Type == "Item")
                {
                    PickUpLabel.Visibility = Visibility.Visible;
                    Canvas.SetLeft(PickUpLabel, Canvas.GetLeft(Dagger));
                    Canvas.SetTop(PickUpLabel, Canvas.GetTop(Dagger));
                    isPickable = true;
                }
                else if (Type == "NPC")
                {
                    TalkTriggerLabel.Visibility = Visibility.Visible;
                    TalkTriggerLabel.Content = "Talk to " + "Tidio";
                }
                else if (Type == "TalkingNPC")
                {
                    isTalking = true;
                    Quest();
                }
                else if (Type == "DropItem")
                {
                    if (inventory[0] == "Dagger")
                    {
                        inventory[0] = null;
                        Dagger.Visibility = Visibility.Hidden;
                        QuestID = 2;
                        Quest();
                    }
                }
                else if (Type == "Spell")
                {
                    HP -= 50;
                    HPBar.Value = HP - 50;
                    HPLabel.Content = HPBar.Value;
                }
                else if (Type == "Enemy")
                {
                    spellTimer.IsEnabled = false;
                    Enemy.Visibility = Visibility.Hidden;
                    Spell.Visibility = Visibility.Hidden;
                }
                else
                {
                    TalkTriggerLabel.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                isPickable = false;
                PickUpLabel.Visibility = Visibility.Hidden;
                TalkTriggerLabel.Visibility = Visibility.Hidden;

            }
        }
        


        bool isTalking = false;
        int isEscMenuShowing = 0;


        // MOVEMENT

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (isTalking != true)
            {
                if (Keyboard.IsKeyDown(Key.Down))
                {
                    Uri MainPlayerUri = new Uri(installPath + @"\assets\characters\viking01_Forward.png", UriKind.Absolute);
                    BitmapImage imageBitmap2 = new BitmapImage(MainPlayerUri);
                    MainPlayer.Source = imageBitmap2;
                    foreach (UIElement child in MainGame.Children)
                    {
                        if (child.Uid != "GameObject")
                        {
                            continue;
                        }
                        else
                        {
                            Canvas.SetTop(child, Canvas.GetTop(child) - 10);
                        }
                    }

                    CollisionSystem(NPC, "NPC");
                    CollisionSystem(Dagger, "Item");
                    CollisionSystem(Enemy, "Enemy");
                }
                if (Keyboard.IsKeyDown(Key.Up))
                {
                    Uri MainPlayerUri = new Uri(installPath + @"\assets\characters\viking01_Up.png", UriKind.Absolute);
                    BitmapImage imageBitmap2 = new BitmapImage(MainPlayerUri);
                    MainPlayer.Source = imageBitmap2;

                    foreach (UIElement child in MainGame.Children)
                    {
                        if (child.Uid != "GameObject")
                        {
                            continue;
                        }
                        else
                        {
                            Canvas.SetTop(child, Canvas.GetTop(child) + 10);
                        }
                    }

                    CollisionSystem(NPC, "NPC");
                    CollisionSystem(Dagger, "Item");
                    CollisionSystem(Enemy, "Enemy");


                }
                if (Keyboard.IsKeyDown(Key.Left))
                {
                    foreach (UIElement child in MainGame.Children)
                    {
                        if (child.Uid != "GameObject")
                        {
                            continue;
                        }
                        else
                        {
                            Uri MainPlayerUri = new Uri(installPath + @"\assets\characters\viking01_Right.png", UriKind.Absolute);
                            BitmapImage imageBitmap2 = new BitmapImage(MainPlayerUri);
                            MainPlayer.Source = imageBitmap2;
                            Canvas.SetLeft(child, Canvas.GetLeft(child) + 10);
                            MainPlayer.RenderTransformOrigin = new Point(0.5, 0.5);
                            ScaleTransform flipTrans = new ScaleTransform();
                            flipTrans.ScaleX = -1;
                            MainPlayer.RenderTransform = flipTrans;
                        }
                    }

                    CollisionSystem(NPC, "NPC");
                    CollisionSystem(Dagger, "Item");
                    CollisionSystem(Enemy, "Enemy");


                }
                if (Keyboard.IsKeyDown(Key.Right))
                {
                    foreach (UIElement child in MainGame.Children)
                    {
                        if (child.Uid != "GameObject")
                        {
                            continue;
                        }
                        else
                        {
                            Uri MainPlayerUri = new Uri(installPath + @"\assets\characters\viking01_Right.png", UriKind.Absolute);
                            BitmapImage imageBitmap2 = new BitmapImage(MainPlayerUri);
                            MainPlayer.Source = imageBitmap2;
                            Canvas.SetLeft(child, Canvas.GetLeft(child) - 10);
                            MainPlayer.RenderTransformOrigin = new Point(0.5, 0.5);
                            ScaleTransform flipTrans2 = new ScaleTransform();
                            flipTrans2.ScaleX = +1;
                            MainPlayer.RenderTransform = flipTrans2;
                        }
                    }

                    CollisionSystem(NPC, "NPC");
                    CollisionSystem(Dagger, "Item");
                    CollisionSystem(Enemy, "Enemy");


                }

                if (Keyboard.IsKeyDown(Key.Space))
                {
                    CollisionSystem(NPC, "TalkingNPC");
                }


            }



            if (Keyboard.IsKeyDown(Key.Escape))
            {
                if (isEscMenuShowing == 1)
                {
                    MainMenuButton.Visibility = Visibility.Hidden;
                    ExitButton.Visibility = Visibility.Hidden;
                    isEscMenuShowing = 0;
                }
                else
                {
                    MainMenuButton.Visibility = Visibility.Visible;
                    ExitButton.Visibility = Visibility.Visible;
                    isEscMenuShowing = 1;
                }

            }

            if(Keyboard.IsKeyDown(Key.C))
            {
                CollisionSystem(NPC, "DropItem");
            }
            if(Keyboard.IsKeyDown(Key.X))
            {
                if (isPickable == true)
                {
                    Canvas.SetLeft(Dagger, Canvas.GetLeft(Inv1));
                    Canvas.SetTop(Dagger, Canvas.GetTop(Inv1));
                    Dagger.Width = 32;
                    Dagger.Height = 32;
                    Dagger.Uid = "Not Moving";
                    inventory[0] = "Dagger";
                    if(inventory[0] == "Dagger")
                    {
                        //Quest completed
                        dispatcherTimer = new DispatcherTimer();
                        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                        dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
                        QuestCompleted.Visibility = Visibility.Visible;
                        QuestCompletedLabel.Visibility = Visibility.Visible;
                        QuestName.Content = "Bring Axe | Completed (1/2)";
                        dispatcherTimer.Start();
                    }
                }
            }
           
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Things which happen after 1 timer interval
            QuestCompleted.Visibility = Visibility.Hidden;
            QuestCompletedLabel.Visibility = Visibility.Hidden;

            //Disable the timer
            dispatcherTimer.IsEnabled = false;
        }

        private void dispatcherTimer_Tick2(object sender, EventArgs e)
        {
            //Things which happen after 1 timer interval
            QuestCompleted.Visibility = Visibility.Hidden;
            QuestCompletedLabel.Visibility = Visibility.Hidden;

            //Disable the timer
            dispatcherTimer.IsEnabled = false;
        }



        private void spellTimer_Tick(object sender, EventArgs e)
        {
            if (Canvas.GetLeft(Spell) < Canvas.GetLeft(Enemy) - 150)
            {
                spellTimer.IsEnabled = false;
                //Spell.Visibility = Visibility.Hidden;
                Canvas.SetLeft(Spell, Canvas.GetLeft(Enemy));
                spellTimer.Start();
            }
            else
            {
                //Things which happen after 1 timer interval
                //Canvas.SetLeft(Enemy, Canvas.GetLeft(Enemy) - 50);
                Canvas.SetLeft(Spell, Canvas.GetLeft(Spell) - 50);
                CollisionSystem(Spell, "Spell");
            }
        }

        string[] inventory = new string[2];

        int QuestID = 1;
        bool AcceptQuest = false;

        public void Quest()
        {
            if (QuestID == 1)
            {
                if (AcceptQuest == true)
                {
                    //Thing to show/do after accepted quest
                    Dagger.Visibility = Visibility.Visible;
                    Enemy.Visibility = Visibility.Visible;
                    QuestName.Content = "Bring Axe | Completed (0/2)";
                    spellTimer = new DispatcherTimer();
                    spellTimer.Tick += new EventHandler(spellTimer_Tick);
                    spellTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
                    Spell.Visibility = Visibility.Visible;
                    spellTimer.Start();
                }
                else
                {
                    DialogBox2BG.Visibility = Visibility.Visible;
                    DialogBox.Visibility = Visibility.Visible;
                    AgreeButton.Visibility = Visibility.Visible;
                    DeclineButton.Visibility = Visibility.Visible;
                    EndTalk.Visibility = Visibility.Visible;
                    Rozhovor();
                }
            }

            if (QuestID == 2)
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick2);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
                QuestCompleted.Visibility = Visibility.Visible;
                QuestCompletedLabel.Visibility = Visibility.Visible;
                QuestCompletedLabel.Content = "Quest completed";
                QuestName.Content = "No quest";
                dispatcherTimer.Start();
            }
        }

        int Stage = 1;

        public void Rozhovor()
        {

            if (Stage == 1)
            {
                string dialog = "Hello Thoras, I need you to help me with something, can you ?";
                DialogBox.Text = dialog;
                AgreeButton.Content = "Yea, what do you need ?";
                DeclineButton.Content = "I'm sorry, I'm kinda busy now.";
                Speak(dialog);
                Talk();
            }
            if (Stage == 2)
            {
                string dialog = "Oh, nice, I forgot my axe in the forest, can you please bring it to me ? I can't leave my shop right now.";
                DialogBox.Text = dialog;
                AgreeButton.Content = "Ok, cool, I'll go for it. See ya soon";
                DeclineButton.Content = "Oh, I forgot, I gotta do something different";
                Speak(dialog);
                Talk();
            }
            if (Stage == 3)
            {
                string dialog = "Ah, thank you very much !";
                DialogBox.Text = dialog;
                AgreeButton.Visibility = Visibility.Hidden;
                DeclineButton.Visibility = Visibility.Hidden;
                Speak(dialog);
                AcceptQuest = true;
                Quest();
            }
            //Decline
            if (Stage == 4)
            {
                string dialog = "No problem, see ya soon.";
                DialogBox.Text = dialog;
                AgreeButton.Visibility = Visibility.Hidden;
                DeclineButton.Visibility = Visibility.Hidden;
                Speak(dialog);
                Stage = 1;
                AgreeButtonStage = 1;
            }
            //Go away
            if (Stage == 5)
            {
                AgreeButton.Visibility = Visibility.Hidden;
                DeclineButton.Visibility = Visibility.Hidden;
                EndTalk.Visibility = Visibility.Hidden;
                DialogBox.Visibility = Visibility.Hidden;
                DialogBox2BG.Visibility = Visibility.Hidden;
                isTalking = false;
            }
        }

        public void Talk()
        {

            SpeechRecognitionEngine recognition = new SpeechRecognitionEngine();
            list.Add(new String[] { "hello", "what do you need", "i will do it" });

            Grammar gr = new Grammar(new GrammarBuilder(list));

            try
            {
                recognition.RequestRecognizerUpdate();
                recognition.LoadGrammar(gr);
                recognition.SpeechRecognized += rec_SpeechRecognized;
                recognition.SetInputToDefaultAudioDevice();
                recognition.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                return;
            }
        }

        public void Speak(string say)
        {
            speech.SpeakAsync(say);
        }

        private void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;

            if(r == "what do you need")
            {
                DialogBox.Text = "Forgot in woods";
                speech.SpeakAsync("Receiving");
                Stage = 2;
            }
            if (r == "i will do it")
            {
                DialogBox.Text = "Thanks a lot";
                Stage = 3;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainMenu window1 = new MainMenu();
            window1.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        int AgreeButtonStage = 1;

        private void AgreeButton_Click(object sender, RoutedEventArgs e)
        {
            if (AgreeButtonStage == 1)
            {
                Stage = 2;                
            }

            if (AgreeButtonStage == 2)
            {
                Stage = 3;
                AgreeButtonStage = 1;
            }

            AgreeButtonStage++;
            Rozhovor();



        }

        private void DeclineButton_Click(object sender, RoutedEventArgs e)
        {
            Stage = 4;
            Rozhovor();
        }

        private void EndTalk_Click(object sender, RoutedEventArgs e)
        {
            Stage = 5;
            Rozhovor();
        }
    }
}