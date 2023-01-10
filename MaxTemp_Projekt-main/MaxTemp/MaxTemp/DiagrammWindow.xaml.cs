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
using System.Windows.Shapes;


namespace MaxTemp
{
    /// <summary>
    /// Interaction logic for DiagrammWindow.xaml
    /// </summary>

    //Diagram Werte kommen von MainWindow ViewModel class. Livecharts 2 Bibliothek gebraucht
    public partial class DiagrammWindow : Window
    {
        public DiagrammWindow()
        {
            InitializeComponent();
        }
    }
}
