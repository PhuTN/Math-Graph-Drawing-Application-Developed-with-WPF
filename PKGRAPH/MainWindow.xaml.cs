using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PKGRAPH.Models;
using System.Windows.Forms;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.IO;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;


namespace PKGRAPH
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string tempFile = "";
        System.Drawing.Color color = System.Drawing.Color.Black;
        bool isSaveRow = true;
        bool isWrite = false;
       
        System.Windows.Controls.TextBox focus = null;
   
        
        private ObservableCollection<GraphModel> fixedGraphModels;
        private PlotModel plotModel ;
        private List<string> deletedExpression;
        private ListBoxItem pointGraph;
        public MainWindow()
        {
            InitializeComponent();
            deletedExpression = new List<string>();
            ColorButton.Foreground = System.Windows.Media.Brushes.Black;
       

            plotModel = new PlotModel();
            
            var xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "X", Minimum = -10, Maximum = 10, IsZoomEnabled = true, IsPanEnabled = true };
            var yAxis = new LinearAxis { Position = AxisPosition.Left, Title = "Y", Minimum = -10, Maximum = 10, IsZoomEnabled = true, IsPanEnabled = true };

            xAxis.MajorGridlineStyle = OxyPlot.LineStyle.Solid;
            yAxis.MajorGridlineStyle = OxyPlot.LineStyle.Solid;

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            plotModel.Legends.Add(new Legend()
            {
                LegendTitle = "Đồ thị",
                LegendPosition = LegendPosition.RightBottom,
                LegendBackground = OxyColor.FromRgb(215, 236, 255)
            }) ;

            myPlot.Model = plotModel;
            fixedGraphModels = new ObservableCollection<GraphModel>();
           
           

        }

        private void ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            
                DrawerHost.IsLeftDrawerOpen = !DrawerHost.IsLeftDrawerOpen;
               
            
        }

        private void MouseLeftDown(object sender, RoutedEventArgs e)
        {
            
           if(true){
                if (DrawerHost.IsLeftDrawerOpen)
                {
                    DrawerHost.IsLeftDrawerOpen = !DrawerHost.IsLeftDrawerOpen;
                    Toggle.IsChecked = !Toggle.IsChecked;
                  
                }
            }
           
        }

        private void PlusButtonClick(object sender, RoutedEventArgs e)
        {
            if (isSaveRow)
            {
                System.Windows.Controls.TextBox newTextBox = new System.Windows.Controls.TextBox
                {
                    Width = 5000,
                    Height = 25,

                };
                graphsListView.Items.Add(newTextBox);
                newTextBox.Focus();

                newTextBox.BorderBrush = System.Windows.Media.Brushes.Wheat;
                newTextBox.GotFocus += TextGotFocus;
                isWrite = true;
                CanNotFocus(graphsListView, newTextBox);
                isSaveRow = false;
            }
            else System.Windows.MessageBox.Show("Bạn phải lưu phương trình đang nhập đã!");
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (!isSaveRow)
            {
                if (focus != null)
                {
                    int dem = 0;
                    foreach (System.Windows.Controls.TextBox textBox in graphsListView.Items)
                    {
                        if (textBox.Text == focus.Text)
                        {
                            dem++;
                        }
                        if (dem > 1)
                        {
                            System.Windows.MessageBox.Show("Đã có phương trình này rồi!");
                            return;
                        }

                    }

                    dem = 0;
                    foreach (GraphModel graphModel in fixedGraphModels)
                    {
                        if (graphModel.Expression == focus.Text) dem++;
                    }
                    if (dem == 0)
                    {
                        ExpressionModel expressionModel = new ExpressionModel(focus.Text,color);

                        if (expressionModel.IsValidated == false)
                        {
                            System.Windows.MessageBox.Show("Phương trình nhập bị lỗi, mời nhập lại phương trình!");
                            return;
                        }

                        fixedGraphModels.Add(new GraphModel(expressionModel.ExpressionString, expressionModel.Lines));
                        RedrawScatterPlot();
                    }
                    CanFocus(graphsListView);
                    isSaveRow = true;
                    isWrite = false;
                }

            }
        }
        private void UndoButtonClick(object sender, RoutedEventArgs e)
        {
            if(deletedExpression.Count != 0)
            {
                
                int dem = 0;
                foreach (System.Windows.Controls.TextBox textBox in graphsListView.Items)
                {
                    if (textBox.Text == deletedExpression[deletedExpression.Count-1])
                    {
                        dem++;
                    }
                    if (dem > 1)
                    {
                        System.Windows.MessageBox.Show("Đã có phương trình này rồi!");
                        return;
                    }

                }
                
                dem = 0;
                foreach (GraphModel graphModel in fixedGraphModels)
                {
                    if (graphModel.Expression == deletedExpression[deletedExpression.Count - 1]) dem++;
                }
                if (dem == 0)
                {
                    ExpressionModel expressionModel = new ExpressionModel(deletedExpression[deletedExpression.Count - 1], color);

                    if (expressionModel.IsValidated == false)
                    {
                        return;
                    }
                    System.Windows.Controls.TextBox newTextBox = new System.Windows.Controls.TextBox
                    {
                        Width = 5000,
                        Height = 25,

                    };
                    newTextBox.Text = deletedExpression[deletedExpression.Count - 1];
                    newTextBox.GotFocus += TextGotFocus;
                    graphsListView.Items.Add(newTextBox);
                    fixedGraphModels.Add(new GraphModel(expressionModel.ExpressionString, expressionModel.Lines));
                    deletedExpression.Remove(deletedExpression[deletedExpression.Count - 1]);
                    RedrawScatterPlot();
                }
                
            }
        }
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (!isWrite)
            {
                if (focus != null)
                {
                    foreach (GraphModel graphModel in fixedGraphModels)
                    {
                        if (focus.Text == graphModel.Expression)
                        {
                            fixedGraphModels.Remove(graphModel);
                            break;
                        }
                    }
                    graphsListView.Items.Remove(focus);
                    deletedExpression.Add(focus.Text);

                    focus = null;
                    isSaveRow = true;
                    RedrawScatterPlot();
                }
            }
            else System.Windows.MessageBox.Show("Bạn phải nhập xong phương trình đã!");
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

        private void PointButtonClick(object sender, RoutedEventArgs e)
        {
            PointWindow window1 = new PointWindow(fixedGraphModels);
            this.IsEnabled = false;
            window1.Owner = this;
            window1.ShowDialog();
            if(window1.DialogResult == true)
            {
                RedrawScatterPlot();
                OxyColor color = OxyPlot.OxyColor.FromArgb(window1.color.A, window1.color.R, window1.color.G, window1.color.B);
                for (int i = 0; i < window1.listBox2.Items.Count -1 ;i++) {
                    for (int j = i+1; j < window1.listBox2.Items.Count; j++)
                    {
                        TextBlock textBlock = (TextBlock)window1.listBox2.Items[i];
                        TextBlock textBlock2 = (TextBlock)window1.listBox2.Items[j];
                        drawIntersectionPoint(textBlock.Text, textBlock2.Text,color);
                    }
                }
            }
            this.IsEnabled = true;


        }

        private void drawIntersectionPoint(string y1, string y2,OxyColor color) 
        {
            
            List<DataPoint> points1 = new List<DataPoint>() ;
            List<DataPoint> points2 = new List<DataPoint>();
            
            foreach (var graph in fixedGraphModels)
            {
                if (graph.Expression == y1) points1 = graph.Lines.Points;
                if(graph.Expression == y2) points2 = graph.Lines.Points;
            }
            for (int i = 0; i < points1.Count; i++)
            {
                // Kiểm tra xem có điểm giống nhau không
                if (Math.Abs(points1[i].Y - points2[i].Y) <= 0.02)
                {
                    plotModel.Series.Add(new ScatterSeries
                    {
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 5,
                        MarkerFill = color,
                        Points = { new ScatterPoint(points1[i].X, (points1[i].Y + points2[i].Y) / 2) }
                    });
                }
            }
            
            myPlot.InvalidatePlot(true);

        }

        

        private void OpenNewFileButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        private void OpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Bạn có muốn lưu file hiện hành?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SaveFileButtonClick(sender, e);
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool canConvert = true;
                string selectedFilePath = openFileDialog.FileName;
                tempFile = selectedFilePath;

                if (!File.Exists(selectedFilePath))
                {
                    canConvert = false;
                }
                string jsonNotation = File.ReadAllText(selectedFilePath);
                IEnumerable<SavedGraphModel> savedGraphModels;
                try
                {
                    savedGraphModels = JsonConvert.DeserializeObject<IEnumerable<SavedGraphModel>>(jsonNotation);
                }
                catch
                {
                    System.Windows.MessageBox.Show("File không đúng định dạng!");
                    canConvert=false;
                    savedGraphModels=null;
                }
                if (savedGraphModels == null)
                {
                    canConvert = false;
                }
                if (canConvert)
                {
                    fixedGraphModels = new ObservableCollection<GraphModel>();
                    foreach(SavedGraphModel savedGraphModel in savedGraphModels)
                    {
                        ExpressionModel expression = new ExpressionModel(savedGraphModel.Expression, savedGraphModel.Color);
                        fixedGraphModels.Add(new GraphModel(expression.ExpressionString, expression.Lines));
                    }
                }
                
                graphsListView.Items.Clear();
                for (int i = 0; i < fixedGraphModels.Count; i++)
                {
                    System.Windows.Controls.TextBox newTextBox = new System.Windows.Controls.TextBox
                    {
                        Width = 5000,
                        Height = 25,
                    };
                    newTextBox.Text = fixedGraphModels[i].Expression;
                    graphsListView.Items.Add(newTextBox);
                    newTextBox.BorderBrush = System.Windows.Media.Brushes.Wheat;
                    newTextBox.GotFocus += TextGotFocus;
                }
                RedrawScatterPlot();
                isSaveRow = true;
                isWrite = false;
                deletedExpression.Clear();
                this.Title = "PKGRAPH - " + GetStringUntilDot(tempFile);
            }
        }

        private void SaveFileButtonClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(tempFile))
            {
                ObservableCollection<SavedGraphModel> savedGraphModels = new ObservableCollection<SavedGraphModel>();
                foreach (GraphModel graphModel in fixedGraphModels)
                {
                    System.Drawing.Color savedColor = System.Drawing.Color.FromArgb(graphModel.Lines.Color.A, graphModel.Lines.Color.R, graphModel.Lines.Color.G, graphModel.Lines.Color.B);
                    savedGraphModels.Add(new SavedGraphModel(graphModel.Expression, savedColor));
                }
                string jsonNotation = JsonConvert.SerializeObject(savedGraphModels, Formatting.Indented);
                File.WriteAllText(tempFile, jsonNotation);
            }
            else
            {
                SaveNewFileButtonClick(sender, e);
            }
        }

        private void SaveNewFileButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON Files (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFilePath = saveFileDialog.FileName;
                ObservableCollection<SavedGraphModel> savedGraphModels = new ObservableCollection<SavedGraphModel>();
                foreach (GraphModel graphModel in fixedGraphModels)
                {
                    System.Drawing.Color savedColor = System.Drawing.Color.FromArgb(graphModel.Lines.Color.A, graphModel.Lines.Color.R, graphModel.Lines.Color.G,graphModel.Lines.Color.B);
                    savedGraphModels.Add(new SavedGraphModel(graphModel.Expression, savedColor));
                }
                string jsonNotation = JsonConvert.SerializeObject(savedGraphModels, Formatting.Indented);


                File.WriteAllText(selectedFilePath, jsonNotation);
                tempFile = selectedFilePath;
                this.Title = "PKGRAPH - " + GetStringUntilDot(tempFile);
            }
        }
        private void RedrawScatterPlot()
        {
            plotModel.Series.Clear();
            foreach (GraphModel graphModel in fixedGraphModels)
            {
                LineSeries line = graphModel.Lines;
                line.Title = graphModel.FullExpression;


                plotModel.Series.Add(line);
            }
            
            
            myPlot.InvalidatePlot(true);
        }
 
        void CanNotFocus(System.Windows.Controls.ListBox listBox, System.Windows.Controls.TextBox textBox)
        {
            foreach (System.Windows.Controls.TextBox tb in listBox.Items)
            {
                if (textBox != tb)
                {
                    tb.Focusable = false;
                }
            }
        }

        void CanFocus(System.Windows.Controls.ListBox listBox)
        {
            foreach (System.Windows.Controls.TextBox tb in listBox.Items)
            {
                tb.Focusable = true;
            }
        }
        
        private void TextGotFocus(object sender, RoutedEventArgs e)
        {
            focus = (System.Windows.Controls.TextBox)sender;
            if (!isWrite)
            {
                if (isSaveRow == true)
                {

                    foreach (GraphModel graphModel in fixedGraphModels)
                    {

                        if (graphModel.Expression == focus.Text)
                        {
                            fixedGraphModels.Remove(graphModel);

                            isSaveRow = false;
                            return;
                        }
                    }
                }
            }
        }

        static string GetStringUntilDot(string input)
        {
            string result = "";
            int begin = 0;
            for(int i = input.Length - 1; i > 0; i--)
            {
                if (input[i] == '\\')
                {
                    begin = i + 1;
                    break;
                }
                
            }
            for(int i = begin; i < input.Length - 5; i++)
            {
                result += input[i];
            }

            return result;
        }
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            MessageBoxResult result = System.Windows.MessageBox.Show("Bạn có muốn lưu file không?", "Xác nhận đóng", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveFileButtonClick(sender, new RoutedEventArgs());
            }
            else if(result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            e.Cancel = false;
  
        }
    }
}

