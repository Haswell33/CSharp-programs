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
    /// Interaction logic for Edit_ticket.xaml
    /// </summary>
    public partial class Edit_ticket : Window
    {
        private bool create_ticket = true;
        public event EventHandler<Dictionary<string, string>> CreateTicket;
        public event EventHandler<Dictionary<string, string>> EditTicket;
        Dictionary<string, string> all_users_to_assigned;
        public string id;
        public Edit_ticket()
        {
            InitializeComponent();
        }
        public void initialize_edit_ticket(Dictionary<string, string> all_value, Dictionary<string, string> all_users_to_assigned, List<string> types)
        {
            this.all_users_to_assigned = all_users_to_assigned;
            
            foreach (string current_user in all_users_to_assigned.Keys)
            {
                box_assigned.Items.Add(current_user);
            }

            foreach (string current_type in types)
            {
                box_type.Items.Add(current_type);
            }

            for (int i = 1; i < 4; i++)
            {
                box_priority.Items.Add("" + i);
            }

            if (all_value == null)
            {
                this.Title = "New ticket";
                label_id.Content = "Create new ticket";
            }
            else
            {
                create_ticket = false;
                label_id.Content = "Edit ticket of ID " + id;
                this.Title = "Edit ticket id - " + id;
                box_title.Text = all_value["@title"];
                box_desc.Text = all_value["@desc"];
                box_assigned.SelectedItem = all_value["@assigment"];
                box_type.SelectedItem = all_value["@type"];
                box_priority.SelectedItem = all_value["@prio"];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (box_title.Text.Equals("")) 
            {
                label_error.Content = "Missing Title";
                label_error.Visibility = 0;
            }
            else if (box_desc.Text.Equals(""))
            {
                label_error.Content = "Missing Description";
                label_error.Visibility = 0;
            }
            else if (box_assigned.SelectedItem == null)
            {
                label_error.Content = "Missing assigned";
                label_error.Visibility = 0;
            }
            else if (box_type.SelectedItem == null)
            {
                label_error.Content = "Missing role";
                label_error.Visibility = 0;
            }
            else if (box_priority.SelectedItem == null)
            {
                label_error.Content = "Missing priority";
                label_error.Visibility = 0;
            }
            else
            {
                Dictionary<string, string> new_values = new Dictionary<string,string>() {
                    {"@title", box_title.Text},
                    {"@desc", box_desc.Text },
                    {"@assigned", all_users_to_assigned[box_assigned.SelectedItem.ToString()]},
                    {"@type", box_type.SelectedItem.ToString()},
                    {"@prio", box_priority.SelectedItem.ToString()}
                };
                if (create_ticket)
                {
                    CreateTicket?.Invoke(this, new_values);
                }
                else
                {
                    new_values["@id"] = id;
                    EditTicket?.Invoke(this, new_values);
                }
                this.Close();
            }

        }
    }
}
