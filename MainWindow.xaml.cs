using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Packaging;
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

namespace Alien_Attacks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer gameTimer = new DispatcherTimer();
        bool moveLeft, moveRight;
        List<Rectangle> itemRemover = new List<Rectangle>();

        Random rand = new Random();

        int enemySpriteCounter = 0;
        int enemyCounter = 100;
        int playerSpeed = 10;
        int limit = 50;
        int score = 0;
        int damage = 0;
        int enemySpeed = 10;

        Rect playerHitBox;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            MyCanvas.Focus();

            ImageBrush bg = new ImageBrush();

            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/mars.png"));
            bg.Stretch = Stretch.UniformToFill; //bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0, 0, 0.15, 0.15);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            MyCanvas.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
            recplayer.Fill = playerImage;

            MessageBox.Show("Press space to shoot, Left Right keys to move");

        }

        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(recplayer), Canvas.GetTop(recplayer), recplayer.Width, recplayer.Height);

            enemyCounter -= 1;

            lblscoreText.Content = "Score: " + score;
            lbldamageText.Content = "Damage " + damage;

            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            if (moveLeft == true && Canvas.GetLeft(recplayer) > 0)
            {
                Canvas.SetLeft(recplayer, Canvas.GetLeft(recplayer) - playerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(recplayer) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(recplayer, Canvas.GetLeft(recplayer) + playerSpeed);
            }


            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemRemover.Add(x);
                    }

                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(enemyHit))
                            {
                                itemRemover.Add(x);
                                itemRemover.Add(y);
                                score++;
                            }
                        }
                    }

                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);

                    if (Canvas.GetTop(x) > 750)
                    {
                        itemRemover.Add(x);
                        damage += 10;
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        itemRemover.Add(x);
                        damage += 5;
                    }

                }
            }

            foreach (Rectangle i in itemRemover)
            {
                MyCanvas.Children.Remove(i);
            }


            if (score > 5)
            {
                limit = 20;
                enemySpeed = 15;
            }

            if (damage > 99)
            {
                gameTimer.Stop();
                lbldamageText.Content = "Damage: 100";
                lbldamageText.Foreground = Brushes.Red;
                MessageBox.Show("You stopped " + score + " Alien Ships" + Environment.NewLine + "Press Ok to Play Again");

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();

            }


        }

        private void MyCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight = true;
            }
        }

        private void MyCanvas_KeyUp(object sender, KeyEventArgs e)
        {
           
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(recplayer) + recplayer.Width / 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(recplayer) - newBullet.Height);

                MyCanvas.Children.Add(newBullet);

            }
        }
            //end here

        //Generate enemies on screen. Make enemies appear

        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();

            enemySpriteCounter = rand.Next(1, 5);

            switch (enemySpriteCounter)
            {
                case 1:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/1.png"));
                    break;
                case 2:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/2.png"));
                    break;
                case 3:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/3.png"));
                    break;
                case 4:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/4.png"));
                    break;
                case 5:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/5.png"));
                    break;
            }

            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 50,
                Width = 56,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rand.Next(30, 430));
            MyCanvas.Children.Add(newEnemy);

        }
    }
}
