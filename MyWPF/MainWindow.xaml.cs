using System.ComponentModel;
using System.Windows;

namespace MyWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {

        private string boundText;

        public event PropertyChangedEventHandler? PropertyChanged;
        public MainWindow() {
            DataContext = this;
            InitializeComponent();
        }

        public string BoundText {
            get { return boundText; }
            set {
                boundText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BoundText"));
            }
        }
    }
}