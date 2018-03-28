using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Calculator
{
    /// <summary>
    /// CalculatorSettings.xaml 的交互逻辑
    /// </summary>
    public partial class CalculatorSettings : Window
    {
        private Interpreter interpreter;
        private double intPrecision;
        private double diffDelta;
        public CalculatorSettings(Interpreter _interpreter)
        {
            InitializeComponent();
            interpreter = _interpreter;
            //窗口生成的时候自动从静态类Settings里读取积分分割次数和求导增量，并且刷新滑动条
            Slider_Integral.Value = Math.Log10(interpreter.intPrecision / Settings.IntegralPrecision) + 3;
            Slider_Diff.Value = Math.Log10(interpreter.diffDelta / Settings.DiffDelta) + 5;
            TextBox_Integral.Text = (Settings.IntegralPrecision * Math.Pow(10, Slider_Integral.Value - 3)).ToString("f0");
            TextBox_Diff.Text = (Settings.DiffDelta * Math.Pow(10, Slider_Diff.Value - 5)).ToString("f15");
        }

        private bool ignoreSliderValueChange = false; //是否忽略滑动条的移动事件
        //积分分割次数滑动条改变事件
        private void Slider_Integral_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(ignoreSliderValueChange)
            {
                return;
            }
            //计算出滑动条对应的值得大小
            intPrecision = Settings.IntegralPrecision * Math.Pow(10, Slider_Integral.Value - 3);
            ignoreTextChange = true;
            //直接同步文本框，但是同时不触发“文本框改动”事件，否则文本框改动又会触发更改滑动条的事件。
            TextBox_Integral.Text = intPrecision.ToString("f0");
            ignoreTextChange = false;
        }
        //求导增量滑动条改变事件
        private void Slider_Diff_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(ignoreSliderValueChange)
            {
                return;
            }
            diffDelta = Settings.DiffDelta * Math.Pow(10, Slider_Diff.Value - 5);
            ignoreTextChange = true;
            TextBox_Diff.Text = diffDelta.ToString("f15");
            ignoreTextChange = false;
        }

        private bool ignoreTextChange = false;

        private void TextBox_Integral_KeyDown(object sender, KeyEventArgs e)
        {
            if(ignoreTextChange)
            {
                return;
            }
            if(e.Key == Key.Enter)
            {
                try
                {
                    double i = double.Parse(TextBox_Integral.Text);
                    ignoreSliderValueChange = true;
                    Slider_Integral.Value = Math.Log10(i / Settings.IntegralPrecision) + 3;
                    ignoreSliderValueChange = false;
                    intPrecision = Settings.IntegralPrecision * Math.Pow(10, Slider_Integral.Value - 3);
                    TextBox_Integral.Text = intPrecision.ToString("f0");
                }
                catch
                { }
            }
        }

        private void TextBox_Diff_KeyDown(object sender, KeyEventArgs e)
        {
            if (ignoreTextChange)
            {
                return;
            }
            if (e.Key == Key.Enter)
            {
                try
                {
                    double i = double.Parse(TextBox_Diff.Text);
                    ignoreSliderValueChange = true;
                    Slider_Diff.Value = Math.Log10(i / Settings.DiffDelta) + 5;
                    ignoreSliderValueChange = false;
                    diffDelta = Settings.DiffDelta * Math.Pow(10, Slider_Diff.Value - 5);
                    TextBox_Diff.Text = diffDelta.ToString("f15");
                }
                catch
                { }
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            interpreter.intPrecision = intPrecision;
            interpreter.diffDelta = diffDelta;
            this.Hide();
        }

        private void Button_Discard_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
