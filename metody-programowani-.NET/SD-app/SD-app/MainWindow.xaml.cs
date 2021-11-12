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

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

using System.Security.Cryptography;
public class Mysql_connect
{
    public List<string> mysql_select(string login, string pass)
    {
        List<string> return_value = new List<string>();
        string connStr = "server=localhost;user=root;database=ticket_system;port=3306;password=";
        MySqlConnection conn = new MySqlConnection(connStr);
        try
        {
            conn.Open();
            string sql = "SELECT ID, Login, Roles FROM users WHERE Login = @login AND Pass = @pass";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", login);
            cmd.Parameters.AddWithValue("@pass", pass);
            cmd.Prepare();
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                return_value.Add(rdr[0].ToString());
                return_value.Add(rdr[1].ToString());
                return_value.Add(rdr[2].ToString());
            }
            rdr.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        conn.Close();
        return return_value;
    }
}


namespace WpfApp1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Mysql_connect new_conn = new Mysql_connect();
            MD5 md5 = new MD5CryptoServiceProvider();
            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(pass_box.Password));
            //get hash result after compute it  
            byte[] hash_pass = md5.Hash;
            StringBuilder new_pass = new StringBuilder();
            for (int i = 0; i < hash_pass.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
               new_pass.Append(hash_pass[i].ToString("x2"));
            }

            List<string> result = new_conn.mysql_select(login_box.Text, new_pass.ToString());
            if (result.Count() == 3) 
            {
                HomePage new_home_page = new HomePage(new CurrentUser(result[0],result[1],result[2]));
                new_home_page.Show();
                this.Close();
            }
            else
            {
                error_label.Content = "Login error";
                error_label.Visibility = 0;
            }
                
        }  
    }
}
