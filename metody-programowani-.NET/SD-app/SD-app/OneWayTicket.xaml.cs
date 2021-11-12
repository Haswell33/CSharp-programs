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
    /// Interaction logic for OneWayTicket.xaml
    /// </summary>
    public partial class OneWayTicket : Page
    {
        public event EventHandler<string> OpenEditTicket;
        public event EventHandler<Dictionary<string,string>> AddComm;
        public event EventHandler<List<string>> UpdateState;
        readonly Brush red = new SolidColorBrush(Color.FromRgb(251, 0, 0));
        public string id;
        public OneWayTicket()
        {
            InitializeComponent();
        }
        public void initialize_issue_page(Dictionary <string, string> all_values, Dictionary<string, string> next_steps, List<Dictionary<string, string>> comments)
        {
            dock_top_panel_grid.Children.Clear();
            dock_comment_text.Children.Clear();
            dock_comment_add.Children.Clear();
            id = all_values["@id"];
            label_tite.Content = all_values["@title"];
            label_description.Text = all_values["@desc"];
            label_assignee.Content = all_values["@full_name_ass"];
            label_reporter.Content = all_values["@full_name_rep"];
            label_type.Content = all_values["@type"];
            label_created.Content = all_values["@date_created"];
            label_updated.Content = all_values["@date_updated"];
            label_status.Content = all_values["@curr_state"];
            label_priority.Content = all_values["@prio"];

            Button new_button_edit = new Button
            {
                Content = "Edit ticket",
                Width = 105,
                Height = 30,
                HorizontalAlignment = (HorizontalAlignment)0,
                Margin = new Thickness(5, 0, 0, 0)

            };
            new_button_edit.Click += (sender, EventArgs) => { OpenEditTicket?.Invoke(this, id); };
            dock_top_panel_grid.Children.Add(new_button_edit);

            int margin = 115;
            foreach (string step in next_steps.Keys)
            {
                Button new_button_to_flow = new Button
                {
                    Content = "Change status to " + step,
                    Width = 150,
                    Height = 30,
                    HorizontalAlignment = (HorizontalAlignment)0,
                    Margin = new Thickness(margin, 0, 0, 0)
                };
                new_button_to_flow.Click += (sender, EventArgs) => { UpdateState?.Invoke(this, new List<string>() { id, next_steps[step] }); };
                margin += 160;
                dock_top_panel_grid.Children.Add(new_button_to_flow);
            }

            //Comments

            if (comments.Count.Equals(0))
            {
                Label new_label_top = new Label
                {
                    Content = "No comments.",
                    Background = new SolidColorBrush(Color.FromRgb(255, 181, 120)),
                    Height = 30,
                    Margin = new Thickness(0, 50, 0, 0)
                };
                dock_comment_text.Children.Add(new_label_top);
            }
            else
            {
                bool firt_comm = false;
                foreach (Dictionary<string, string> one_comment in comments)
                {
                    StackPanel new_stack_for_one_comment = new StackPanel
                    {
                        VerticalAlignment = 0,
                        Background = new SolidColorBrush(Color.FromRgb(255, 181, 120)),
                        Orientation = (Orientation)1
                    };
                    if (firt_comm)
                    {
                        new_stack_for_one_comment.Margin = new Thickness(0, 50, 0, 0);
                        firt_comm = false;
                    }
                    else
                        new_stack_for_one_comment.Margin = new Thickness(0, 10, 0, 0);

                    Label new_label_top = new Label
                    {
                        Content = one_comment["@owner"]+" added a comment at " + one_comment["@add_time"],
                        Height = 30,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    new_stack_for_one_comment.Children.Add(new_label_top);

                    TextBlock new_label_text = new TextBlock
                    {
                        Text = one_comment["@text_com"]
                    };
                    new_stack_for_one_comment.Children.Add(new_label_text);
                    dock_comment_text.Children.Add(new_stack_for_one_comment);
                }
            }
            Button add_comment_button = new Button
            {
                Content = "Add comment",
                Height = 30,
                Width = 100,
                HorizontalAlignment = (HorizontalAlignment)2,
                Margin = new Thickness(0, 15, 80, 10)
            };
            add_comment_button.Click += (sender, EventArgs) => { add_comm(sender, EventArgs, id); };
            dock_comment_add.Children.Add(add_comment_button);
            dock_comment_text.Margin = new Thickness(0, 0, 0, 65);
        }
        private void add_comm(object senderr, RoutedEventArgs e, string id)
        {
            dock_comment_add.Children.Clear();
            dock_comment_text.Margin = new Thickness(0, 0, 0, 240);
            Label label_comm = new Label
            {
                Content = "Add comment:",
                Height = 30,
                HorizontalAlignment = 0,
                VerticalAlignment = 0
            };
            TextBox box_comm = new TextBox
            {
                TextWrapping = (TextWrapping)2,
                Height = 100,
                Margin = new Thickness(10, 0, 80, 0),
                AcceptsReturn = true,
                AcceptsTab = true
            };
            Button button_comm = new Button
            {
                Content = "Commit",
                Height = 30,
                Width = 100,
                HorizontalAlignment = (HorizontalAlignment)2,
                Margin = new Thickness(0, 170, 80, 30)
            };

            button_comm.Click += (sender, EventArgs) => { commit_comm(sender, EventArgs, id, box_comm.Text); };
            dock_comment_add.Children.Add(label_comm);
            dock_comment_add.Children.Add(box_comm);
            dock_comment_add.Children.Add(button_comm);
        }
        private void commit_comm(object sender, RoutedEventArgs e, string id, string text)
        {
            if (text.Equals(""))
            {
                Label label_validator = new Label
                {
                    Content = "The comment cannot be empty",
                    Height = 30,
                    HorizontalAlignment = (HorizontalAlignment)0,
                    Margin = new Thickness(20, 130, 0, 0),
                    Foreground = red
                };
                dock_comment_add.Children.Add(label_validator);
            }
            else
            {
                AddComm?.Invoke(this, new Dictionary<string, string> { { "@text",text},{"@id",id } }) ;
            }
        }
        public void WindowOnSizeChanged(object sender, List<double> new_size)
        {
            if (new_size[0] > 90 && new_size[1] > 20)
            {
                scroll_issue.Height = new_size[0] - 95;
                scroll_issue.Width = new_size[1] - 40;

                dock_issue.Width = new_size[1];
                dock_top_panel_grid.Width = new_size[1];
                dock_title_grid.Width = new_size[1];
                dock_left_grid.Width = new_size[1] * 0.75;
                dock_right_grid.Width = new_size[1] * 0.25;
                dock_desc_gird.Width = new_size[1];
                dock_comment_gird.Width = new_size[1];
            }

        }
    }
}
