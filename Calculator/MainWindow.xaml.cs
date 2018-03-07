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


namespace Calculator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private AutoCompletor completor;
        private int lastLength = 0;
        public MainWindow()
        {
            completor = new AutoCompletor();
            InitializeComponent();

        }

        private void CalcButton_Click(object sender, RoutedEventArgs e)
        {
            Parser parser = new Parser();
            Interpreter interpreter = new Interpreter();
            string result = interpreter.Interprete(parser.Parse(InputBox.Text.Trim())).ToString();
            OutputBox.Text = result;
        }

        private bool lastChangeIsAuto = false;
        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(lastChangeIsAuto)
            {
                lastLength = InputBox.Text.Length;
                return;
            }
            else if (InputBox.Text.Length > lastLength)
            {
                lastChangeIsAuto = true;
                int selection = InputBox.SelectionStart;
                string tail = completor.Completion(InputBox.Text.Substring(selection - 1,1));
                InputBox.Text = InputBox.Text.Insert(selection,tail);
                InputBox.SelectionStart = selection + tail.Length;
                lastChangeIsAuto = false;
            }
            lastLength = InputBox.Text.Length;
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
