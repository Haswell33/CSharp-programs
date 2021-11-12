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



namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ticket_viewer.xaml
    /// </summary>
    public partial class ticket_viewer : Page
    {


        public event EventHandler<string> OpenNewIssue;
        public event EventHandler<string> ShowSearchResult;
        Filter new_filter;
        public ticket_viewer()
        {
            InitializeComponent();
        }
        public void Initialize_viewer(List<string> title_list, List<List<string>> row_list)
        {
            allTickets.Children.Clear();
            new_filter = new Filter(title_list, row_list, allTickets, OpenNewIssue);
            WindowOnSizeChanged(this, new List<double>() { this.ActualHeight + 95, this.ActualWidth + 30 });
        }

        public void WindowOnSizeChanged(object sender, List<double> new_size)
        {
            if (new_size[0] >90 && new_size[1] > 20)
            {
                scroll_name.Height = new_size[0] - 95;
                scroll_name.Width = new_size[1] - 30;
                new_filter.change_window_size(new_size[0], new_size[1]);
                search.Width = new_size[1] - 30;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShowSearchResult?.Invoke(this, search.Text);
        }
    }
}
