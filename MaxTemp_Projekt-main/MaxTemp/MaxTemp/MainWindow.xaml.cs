using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using static System.Windows.Media.Brush;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.ObjectModel;

namespace MaxTemp
{
    [ObservableObject]
    public class ViewModel
    {
        public ISeries[] Series { get; set; }
            = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    //Diagramm Werte
                    Values = MainWindow.werte,
                    IsVisible = true,

                    Name = "Temperatur",
                    TooltipLabelFormatter = (chartPoint) => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue}C°"
                },
            };

        //**********************SICHTBARKEIT SERVERS IN DIAGRAMM ICOMMANDS**************

        //S1 Toggle
        private RelayCommand toggleS1Command;
        public ICommand ToggleS1Command
        {
            get
            {
                if (toggleS1Command == null)
                {
                    toggleS1Command = new RelayCommand(ToggleS1);
                }

                return toggleS1Command;
            }
        }

        private void ToggleS1()
        {
            Series[0].IsVisible = !Series[0].IsVisible;
        } 
        
        //******************************************************************************

        public Axis[] XAxes { get; set; } =
   {
        new Axis
        {
            Name = "Servers",
            NamePaint = new SolidColorPaint { Color = SKColors.RoyalBlue },
            Labels = MainWindow.locations,
            ForceStepToMin = true
        },
    };
        public Axis[] YAxes { get; set; } =
{
        new Axis
        {
            Name = "Temperaturen",
            NamePaint = new SolidColorPaint { Color = SKColors.RoyalBlue },
            Labeler = (value) => value + "C°"
        }
    };
    }
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<double> werte = new List<double>();
        public static List<string> locations = new List<string>();
        public static List<string> time = new List<string>();
        public static List<string> locationBtnList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            StreamReader reader = new StreamReader(File.OpenRead("./temps.csv"));

            //CSV lesen
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                werte.Add(Convert.ToDouble(line[line.Length - 1].Replace('.', ',')));
                locations.Add(line[0]);
                time.Add(line[1]);
                double temp = Convert.ToDouble(line[line.Length - 1].Replace('.', ','));
            }
         
            reader.Close();

            this.CreateButtons();
        }

        //Höchstwert in MessageBox ausgeben
        private void BtnAuswerten_Click(object sender, RoutedEventArgs e)
        {

            double maxTemp = werte.Max();

            string location = locations[werte.IndexOf(maxTemp)];

            string timeOFMaxTemp = time[werte.IndexOf(maxTemp)];

            //Höchstwert auf Oberfläche ausgeben.
            MessageBox.Show($"{location} hat die maximal Temperatur von {maxTemp}°.\nDatum: {timeOFMaxTemp}");
        }

        private void CreateButtons()
        {
            foreach (string location in locations)
            {
                if (locationBtnList.IndexOf(location) == -1)
                {
                    locationBtnList.Add(location);
                }
            }

            int Y = 0;

            foreach (string i in locationBtnList)
            {
                Button btn = new Button();
                btn.Height = 15;
                btn.Width = 15;
                btn.FontSize = 8;
                btn.Content = i;
                Canvas.SetLeft(btn, Y);
                Y += 20;
                btn.Background = new SolidColorBrush(Colors.Orange);
                btn.Foreground = new SolidColorBrush(Colors.Black);
                btn.Click += ServerButtons_Click;
                LayoutRoot.Children.Add(btn);
            }
        }

        //Filtered Orte anzeigen
        private void ServerButtons_Click(object sender, RoutedEventArgs e)
        {
            var selectedButton = (e.Source as Button).Content.ToString();
            List<string> data = new List<string>();
            StreamReader reader = new StreamReader(File.OpenRead("./temps.csv"));
            Window ServerDetails = new Window();
            ListView table = new ListView();

            table.Height = 100;
            table.Width = 100;
            table.FontSize = 5;
            table.Margin = new Thickness(0, 38, -145, 19);
            table.Background = new SolidColorBrush(Colors.Orange);

            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                if (line.Contains(selectedButton))
                {
                    if (Convert.ToDouble(line[2].Replace('.', ',')) > 40)
                    {
                        table.Items.Add($"**{line[0]}----{line[1]}----{line[2]}**");
                    }
                    else
                    {
                        table.Items.Add($"{line[0]}----{line[1]}----{line[2]}");
                    }
                }
            }

            reader.Close();

            LayoutRoot2.Children.Add(table);

        }

        private void BtnDiagramm_Click(object sender, RoutedEventArgs e)
        {

            DiagrammWindow diagramm = new DiagrammWindow();
            diagramm.Show();
        }
    }

}
