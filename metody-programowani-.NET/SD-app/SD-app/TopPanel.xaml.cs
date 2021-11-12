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
    /// Interaction logic for TopPanel.xaml
    /// </summary>
    public partial class TopPanel : Page
    {
        public event EventHandler OpenAdmin;
        public event EventHandler CloseIssuePage;
        public event EventHandler<string> OpenCreateTicket;
        public event EventHandler OpenFilter;
        public event EventHandler<string> OpenIssue;
        readonly Brush clicked_button = new SolidColorBrush(Color.FromRgb(210, 210, 210));
        readonly Brush no_clicked_button = new SolidColorBrush(Color.FromRgb(161, 161, 161));
        private List<Button> all_button_on_panel = new List<Button>();
        public TopPanel()
        {
            InitializeComponent();
        }
        public void initialize_top_panel(string roles)
        {
            if (roles.Contains("admin"))
            {
                Button new_but_admin = new Button
                {
                    Content = "Admin panel",
                    Width = 40,
                    Height = 20,
                    HorizontalAlignment = 0,
                    Background = no_clicked_button

                };
                new_but_admin.Click += OpenAdmin.Invoke;
                top_panel.Children.Add(new_but_admin);
            }

            Button new_but_create_ticket = new Button
            {
                Content = "Create new ticket",
                Width = 40,
                Height = 20,
                HorizontalAlignment = 0,
                Background = no_clicked_button,
                Margin = new Thickness(0, 0, 10, 0)

            };
            new_but_create_ticket.Click += (sender, EventArgs) => { OpenCreateTicket?.Invoke(this, ""); };
            top_panel.Children.Add(new_but_create_ticket);


            Button new_but = new Button
            {
                Content = "Filter",
                Width = 40,
                Height = 20,
                HorizontalAlignment = 0,
                Background = clicked_button

            };
            new_but.Click += (sender, EventArgs) => { go_to_filter(this, "Filter"); };
            all_button_on_panel.Add(new_but);
            top_panel.Children.Add(new_but);

            OpenFilter?.Invoke(this, new EventArgs());
        }

        public void go_to_Click(string id)
        {
            int is_availabe = -1;
            foreach (Button cur_butt in all_button_on_panel)
            {
                if (cur_butt.Content.Equals(id))
                {
                    cur_butt.Background = clicked_button;
                    is_availabe = all_button_on_panel.IndexOf(cur_butt);
                }
                else
                    cur_butt.Background = no_clicked_button;

            }
            if (is_availabe.Equals(-1))
            {
                Button new_but = new Button
                {
                    Content = id,
                    Width = 50,
                    Height = 20,
                    HorizontalAlignment = 0,
                    Background = clicked_button

                };
                new_but.Click += (sender, EventArgs) => { go_to_new_issue(this, id); };
                Button new_but_x = new Button
                {
                    Content = "X",
                    Width = 12,
                    Height = 12,
                    HorizontalAlignment = (HorizontalAlignment)2,
                    Margin = new Thickness(-12, -10, 0, 0)
                };
                new_but_x.Click += (sender, EventArgs) => { delete_button_on_top_panel(sender, EventArgs, new_but, new_but_x); };

                all_button_on_panel.Add(new_but);
                top_panel.Children.Add(new_but);
                top_panel.Children.Add(new_but_x);
            }
            go_to_new_issue(this, id);
        }

        private void go_to_new_issue(object sender, string id)
        {
            change_color_button(id);
            OpenIssue?.Invoke(this, id);
        }

        private void go_to_filter(object sender, string id)
        {
            change_color_button(id);
            OpenFilter?.Invoke(this, new EventArgs());
        }

        private void change_color_button(string id)
        {
            foreach (Button cur_but in all_button_on_panel)
            {
                if (cur_but.Content.ToString().Equals(id))
                    cur_but.Background = clicked_button;
                else
                    cur_but.Background = no_clicked_button;
            }
        }


        private void delete_button_on_top_panel(object senderr, RoutedEventArgs e, Button but, Button but_x)
        {
            if (but.Background.Equals(clicked_button))
                CloseIssuePage?.Invoke(this, new EventArgs());

            all_button_on_panel.Remove(but);
            top_panel.Children.Remove(but);
            top_panel.Children.Remove(but_x);
        }
    }
}
