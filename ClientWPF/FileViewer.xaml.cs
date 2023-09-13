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

namespace ClientWPF
{
    public partial class FileViewer : Window
    {
        private string[] textData;
        public FileViewer(string[] textData)
        {
            InitializeComponent();
            this.textData = textData;
            presentData();
        }

        private void presentData()
        {
            foreach(string text in textData)
            {
                TextBox_TextPresenter.Text += text + "\n";
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
