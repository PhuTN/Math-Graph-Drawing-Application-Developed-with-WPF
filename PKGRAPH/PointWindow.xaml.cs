using PKGRAPH.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
namespace PKGRAPH
{
    public partial class PointWindow : Window
    {
        public System.Drawing.Color color = System.Drawing.Color.Black;
        public PointWindow(ObservableCollection<GraphModel> fixedGraphModels)
        {
            InitializeComponent();
            listBox.SelectionChanged += TextBox_GotFocus;
            listBox2.SelectionChanged += DeleteTextBox;
            ColorButton.Foreground = System.Windows.Media.Brushes.Black;
            foreach (var graphModel in fixedGraphModels)
            {
                System.Windows.Controls.TextBlock textBox = new System.Windows.Controls.TextBlock();
                textBox.Text = graphModel.Expression;
                listBox.Items.Add(textBox);
            }
        }

       
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBlock textBox = listBox.SelectedItem as System.Windows.Controls.TextBlock;
            foreach(System.Windows.Controls.TextBlock item in listBox2.Items)
            {
                if (textBox.Text == item.Text) return;
            }
            System.Windows.Controls.TextBlock textBox2 = new System.Windows.Controls.TextBlock();
            textBox2.Text = textBox.Text;
            listBox2.Items.Add(textBox2);
        }
        private void DeleteTextBox(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBlock textBox = listBox2.SelectedItem as System.Windows.Controls.TextBlock;
            listBox2.Items.Remove(textBox);

        }
        private void ColorButtonClick(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            DialogResult result = colorDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                color = colorDialog.Color;
                System.Drawing.Color selectedColor = colorDialog.Color;
                System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);
                SolidColorBrush myBrush = new SolidColorBrush(wpfColor);
                ColorButton.Foreground = myBrush;
            }
        }
        private void OkeButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
