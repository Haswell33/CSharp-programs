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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        private List<List<Label>> all_labels = new List<List<Label>>();
        private int count_column = 5;
        public event EventHandler<string> OpenEditPanel;
        public AdminPanel()
        {
            InitializeComponent();
            SizeChanged += WindowOnSizeChanged;
            button_create_user.Click += (sender, EventArgs) => { OpenEditPanel(sender, ""); };
        }
        public void initialize_admin_page(List<string> title_list, List<List<string>> row_list)
        {
            new Filter(title_list,row_list,allUsers, OpenEditPanel);
        }
        private void WindowOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            scroll_name.Height = (double)this.ActualHeight;
            scroll_name.Width = (double)this.ActualWidth;

            foreach (List<Label> cur_labels_list in all_labels)
            {
                double margin_in_row = (double)this.ActualWidth / count_column;
                foreach (Label cur_label in cur_labels_list)
                {
                    cur_label.Width = margin_in_row;
                }
            }
        }
    }
}
