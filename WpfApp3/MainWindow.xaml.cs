using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = "Text Editor";

            foreach (var item in Fonts.SystemFontFamilies)
            {
                fontsCB.Items.Add(item);
            }
            fontsCB.SelectedIndex = 0;


            for (int i = 9; i <= 72; i++)
                sizeCB.Items.Add(i);
            sizeCB.SelectedIndex = 0;


            Type colors = typeof(System.Drawing.Color);
            PropertyInfo[] colorInfo = colors.GetProperties(BindingFlags.Public |
                BindingFlags.Static);
            foreach (PropertyInfo info in colorInfo)
            {
                colorCB.Items.Add(info.Name);
            }
        }

        private string _filePath = "";
        private bool _autoSave = false;
        private bool _bold = false;
        private bool _italic = false;
        private bool _under = false;

        private void openFileClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Text|*.txt"
            };

            if (openFileDialog.ShowDialog() ?? false)
            {
                using StreamReader streamReader = new(openFileDialog.FileName);
                _filePath = openFileDialog.FileName;
                reloadFilePath();
                txtBox.Selection.Text = streamReader.ReadToEnd();
            }
        }

        private void saveFileClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() ?? false)
            {
                txtBox.SelectAll();
                using StreamWriter streamWriter = new(saveFileDialog.FileName);
                streamWriter.Write(txtBox.Selection);
                reloadFilePath();
            }
        }


        private void fontsCBselectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var font = new FontFamily(fontsCB.SelectedItem.ToString());

            if (!txtBox.Selection.IsEmpty)
                txtBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, font);
            else
                txtBox.FontFamily = font;
        }

        private void sizeCBselectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            double size = 0;
            double.TryParse(sizeCB.SelectedItem.ToString(), out size);

            if (size == 0)
                return;

            if (!txtBox.Selection.IsEmpty)
                txtBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            else
                txtBox.FontSize = size;
        }

        private void fontStyleClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {

                switch (btn.Tag.ToString())
                {
                    case "Bold":
                        _bold = !_bold;
                        break;
                    case "Italic":
                        _italic = !_italic;
                        break;
                    case "Under":
                        _under = !_under;
                        break;
                }

                if (!txtBox.Selection.IsEmpty)
                {
                    txtBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, _bold ? FontWeights.Bold : FontWeights.Normal);
                    txtBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, _italic ? FontStyles.Italic : FontStyles.Normal);
                    txtBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, _under ? TextDecorations.Underline : null);
                }
                else
                {
                    txtBox.FontWeight = _bold ? FontWeights.Bold : FontWeights.Normal;
                    txtBox.FontStyle = _italic ? FontStyles.Italic : FontStyles.Normal;
                }

            }
        }


        private void colorCBselectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MessageBox.Show("We are working on it...");
        }


        private void AlignClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                txtBox.HorizontalContentAlignment = btn.Tag.ToString() switch
                {
                    "left" => HorizontalAlignment.Left,
                    "center" => HorizontalAlignment.Center,
                    "right" => HorizontalAlignment.Right,
                    "justify" => HorizontalAlignment.Stretch,
                    _ => HorizontalAlignment.Center
                };
            }
        }



        private void autoSaveClicked(object sender, RoutedEventArgs e)
        {
            if (_filePath.Equals(""))
            {
                MessageBox.Show("Auto Save can only be used when the file is opened from the computer...");
                return;
            }

            autoSave.IsChecked = !autoSave.IsChecked;

            _autoSave = autoSave.IsChecked;
        }

        private void fielPathClick(object sender, RoutedEventArgs e)
        {
            filePath.IsChecked = !filePath.IsChecked;

            reloadFilePath();
        }



        private void paramClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {

                switch (btn.ToolTip.ToString())
                {
                    case "Cut":
                        paramControls(ApplicationCommands.Cut);
                        break;
                    case "Copy":
                        paramControls(ApplicationCommands.Copy);
                        break;
                    case "Paste":
                        paramControls(ApplicationCommands.Paste);
                        break;
                    case "Select All":
                        txtBox.Focus();
                        paramControls(ApplicationCommands.SelectAll);
                        //txtBox.SelectAll();
                        break;
                }

            }
        }


        private void txtBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_autoSave)
            {
                TextRange txtRange = new TextRange(
                    txtBox.Document.ContentStart,
                    txtBox.Document.ContentEnd
                );
                File.WriteAllText(_filePath, txtRange.Text);
            }
        }


        private void paramControls(RoutedUICommand rUIc)
        {
            if (rUIc.CanExecute(null, txtBox))
            {
                rUIc.Execute(null, txtBox);
            }
        }

        private void reloadFilePath()
        {
            if (filePath.IsChecked)
                Title = $"Text Editor ( {_filePath} )";
            else
                Title = "Text Editor";
        }
    }
}
