using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MathGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var list = new LanguageList();
            xnLanguage.DataContext = list;
            xnLanguage.DisplayMemberPath = "Name";
            xnLanguage.SelectedValuePath = "Code";
            if (!string.IsNullOrEmpty(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride))
            {
                //Select the selected language from previous session
                xnLanguage.SelectedIndex = list.IndexOf(list.Single(l => l.Code.Substring(0, 2).Equals(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride.Substring(0, 2))));
            }
            t.Interval = new TimeSpan(0, 0, 1); //Make timer "tick" every 1 second
            t.Tick += T_Tick; //Process time every 1 second
        }

        private void T_Tick(object sender, object e)
        {
            xnTime.Text = (int.Parse(xnTime.Text)-1).ToString();
            if (xnTime.Text == "0")
            {
                t.Stop(); 
                EndGame();
            }
        }
        int answer = 0; //Number of  answer generated
        DispatcherTimer t = new DispatcherTimer();
        private void Language_Changed(object sender, SelectionChangedEventArgs e)
        {
            //In case deselect
            if (xnLanguage.SelectedIndex < 0)
                return;
            //Else, set language and reset view
            string lang = xnLanguage.SelectedValue as string;
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = lang;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(lang);
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(lang);
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();
            Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Reset();
            //Reload the page for new language
            (Window.Current.Content as Frame).Navigate(typeof(MainPage));
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //Restart all the information on form to play
            answer = 0;
            xnPoint.Text = "0";
            xnTime.Text = "3";
            xnTrue.IsEnabled = true;
            xnFalse.IsEnabled = true;
            xnStart.IsEnabled = false;
            //Start the game by generate new formula
            GenerateFormula();
        }
        bool kq = false; //This will hold the current answer
        void GenerateFormula()
        {
            answer++;
            Random r = new Random();
            //Base on number of answer, generate harder game
            int so1 = r.Next(0+(int)Math.Pow(5,(int)Math.Floor((double)answer/5)-1), 9+ (int)Math.Pow(5, (int)Math.Ceiling((double)answer / 5))-1);
            int so2 = r.Next(0 + (int)Math.Pow(5, (int)Math.Floor((double)answer / 5)-1), 9 + (int)Math.Pow(5, (int)Math.Ceiling((double)answer / 5))-1); 
            string dau = "+-*"; //Not play with divide as many problem may occurs with real number.
            int index = 0;
            index = r.Next(0, 3); //Random between 0,1,2
            if (index == 3) index = 2;
            xnFormula.Text = string.Format("{0}{1}{2}", so1, dau[index], so2);
            //Calculate the result using Jace engine
            int ketqua = (int)(new Jace.CalculationEngine()).Calculate(xnFormula.Text);
            //Ketqua is "true" result, but we need either true / false. We'll random between these 2.
            index = r.Next(0, 2);
            if (index == 2) index = 1;
            if(index==1)
            {
                //in case true
                xnFormula.Text = xnFormula.Text + "=" + ketqua;
                kq = true;
            }
            else
            {
                //We'll generate false result for false case.
                int ketquasai = ketqua;
                while(ketquasai==ketqua)
                {
                    ketquasai = r.Next(ketqua - 10, ketqua + 10); //Generate ketquasai = false value, in case it equal to true value, generate again until they mismatch
                }
                xnFormula.Text = xnFormula.Text + "=" + ketquasai;
                kq = false;
            }
            xnTime.Text = "3"; //Reset time
            t.Start();
        }

        private void True_Click(object sender, RoutedEventArgs e)
        {
            t.Stop();
            if (kq)
            {
                //Increase point & generate new formula
                xnPoint.Text = (int.Parse(xnPoint.Text) + 1).ToString();
                GenerateFormula();
            }
            else
                EndGame();
        }

        private void False_Click(object sender, RoutedEventArgs e)
        {
            t.Stop();
            if (!kq)
            {
                //Increase point & generate new formula
                xnPoint.Text = (int.Parse(xnPoint.Text) + 1).ToString();
                GenerateFormula();
            }
            else
                EndGame();
        }

        void EndGame()
        {
            Windows.UI.Popups.MessageDialog m = new Windows.UI.Popups.MessageDialog("You lose! Point: " + xnPoint.Text);
            m.ShowAsync();
            xnPoint.Text = "0";
            xnTime.Text = "3";
            xnStart.IsEnabled = true;
            xnTrue.IsEnabled = false;
            xnFalse.IsEnabled = false;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            t.Stop();
            Application.Current.Exit();
        }
    }
}
