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

using System.Security.Cryptography;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for EditUser.xaml
    /// </summary>
    public partial class EditUser : Window
    {
        private bool create_user = true;
        public string id;
        private string pass = "";
        public event EventHandler<Dictionary<string, string>> CreateUser;
        public event EventHandler<Dictionary<string, string>> EditThatUser;
        public EditUser()
        {

            InitializeComponent();
        }
        public void initialize_edit_user(Dictionary<string, string> user_value, List<string> roles)
        {
            foreach (string cur_role in roles)
            {
                box_role.Items.Add(cur_role);
            }

            if (user_value == null)
            {
                this.Title = "New user";
                label_id.Content = "Create new user";
            }
            else
            {
                create_user = false;
                pass = user_value["@password"];
                label_id.Content = "Edit user of ID " + id;
                this.Title = "Edit user id - " + id;
                box_name.Text = user_value["@name"];
                box_login.Text = user_value["@login"];
                box_pass.Password = user_value["@password"];
                box_role.SelectedItem = user_value["@role"];
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (box_name.Text.Equals(""))
            {
                label_error.Content = "Missing full name";
                label_error.Visibility = 0;
            }
            else if (box_login.Text.Equals(""))
            {
                label_error.Content = "Missing login";
                label_error.Visibility = 0;
            }
            else if (box_pass.Password.Equals(""))
            {
                label_error.Content = "Missing password";
                label_error.Visibility = 0;
            }
            else if (box_role.SelectedItem == null)
            {
                label_error.Content = "Missing type";
                label_error.Visibility = 0;
            }
            else
            {
                Dictionary<string, string> new_values = new Dictionary<string, string>() {
                    {"@name", box_name.Text},
                    {"@login", box_login.Text},
                    {"@role", box_role.SelectedItem.ToString()}
                };
                if (!pass.Equals(box_pass.Password.ToString()))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    //compute hash from the bytes of text  
                    md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(box_pass.Password));
                    //get hash result after compute it  
                    byte[] hash_pass = md5.Hash;
                    StringBuilder new_pass = new StringBuilder();
                    for (int i = 0; i < hash_pass.Length; i++)
                    {
                        //change it into 2 hexadecimal digits  
                        //for each byte  
                        new_pass.Append(hash_pass[i].ToString("x2"));
                    }
                    new_values["@pass"] = new_pass.ToString();
                }
                else
                    new_values["@pass"] = pass;
                if (create_user)
                {
                    CreateUser?.Invoke(e, new_values);
                }
                else
                {
                    new_values["@id"] = id;
                    EditThatUser?.Invoke(e, new_values);
                }
                this.Close();
            }
        }
    }
}
