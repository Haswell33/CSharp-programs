//Cezary Culepa i Karol Siedlaczek

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    //Klasa do edytowania danych w bazie danych
    class DBConnectEdit
    {

        public event EventHandler UpdateView;
        private CurrentUser cur_user;
        string connStr = "server=localhost;user=root;database=ticket_system;port=3306;password=";
        
        public DBConnectEdit(CurrentUser cur_user)
        {
            this.cur_user = cur_user;
        }
        public void mysql_edit_data(string query, Dictionary<string, string> query_values)
        {
            MySqlConnection my_conn = new MySqlConnection(connStr);
            MySqlCommand my_Command = new MySqlCommand(query, my_conn);
            foreach (KeyValuePair<string, string> one_value in query_values)
            {
                my_Command.Parameters.AddWithValue(one_value.Key, one_value.Value);
            }
            my_conn.Open();
            my_Command.Prepare();
            MySqlDataReader my_reader;
            my_reader = my_Command.ExecuteReader();
            my_conn.Close();
        }
        public List<List<string>> mysql_select(string query, Dictionary<string, string> query_values)
        {
            List<List<string>> return_value = new List<List<string>>();

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                if (query_values != null)
                {
                    foreach (KeyValuePair<string, string> one_value in query_values)
                    {
                        cmd.Parameters.AddWithValue(one_value.Key, one_value.Value);
                    }
                    cmd.Prepare();
                }

                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    List<string> new_row = new List<string>();
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        new_row.Add(rdr[i].ToString());
                    }
                    return_value.Add(new_row);
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
        public void update_ticket_db(object sender, Dictionary<string, string> new_data)
        {
            if(this.mysql_select("SELECT Type FROM issues WHERE ID = @id", new Dictionary<string, string> { { "@id", new_data["@id"] } })[0][0] == new_data["@type"])
            {
                this.mysql_edit_data("UPDATE issues SET Title = @title, Description = @desc, Assigned = @assigned," +
                    " Type = @type, Priority = @prio WHERE ID = @id", new_data);
            }
            else
            {
                new_data["@cur_state"] = this.mysql_select("SELECT ID FROM flow WHERE Type_ticket = '" + new_data["@type"] + "'", null)[0][0];
                this.mysql_edit_data("UPDATE issues SET Title = @title, Description = @desc, Assigned = @assigned," +
                    " Type = @type, Priority = @prio, Current_state = @cur_state WHERE ID = @id", new_data);
            }

            UpdateView?.Invoke(this, new EventArgs());
        }
        public void insert_ticket_db(object sender, Dictionary<string, string> new_data)
        {
            new_data["@reporter"] = cur_user.id;
            new_data["@cur_state"] = this.mysql_select("SELECT ID FROM flow WHERE Type_ticket = '"+new_data["@type"]+"'", null)[0][0];
            this.mysql_edit_data("INSERT INTO issues (Title, Description, Assigned, Reporter, Type, Current_state, " +
                "Priority) VALUES (@title, @desc, @assigned, @reporter, @type, @cur_state, @prio)", new_data);
            UpdateView?.Invoke(this, new EventArgs());
        }
        public void update_user_db(object sender, Dictionary<string, string> new_data)
        {
            this.mysql_edit_data("UPDATE users SET Full_name = @name, Login = @login, Pass = @pass, " +
                "Roles = @role WHERE ID = @id", new_data);
            UpdateView?.Invoke(this, new EventArgs());
        }
        public void insert_user_db(object sender, Dictionary<string, string> new_data)
        {
            this.mysql_edit_data("INSERT INTO users (Full_name, Login, Pass, Roles) VALUES (@name, @login, @pass, " +
                "@role)", new_data);
            UpdateView?.Invoke(this, new EventArgs());
        }
        public void insert_comm_db(object sender, Dictionary<string, string> new_data)
        {
            new_data["@reporter"] = cur_user.id;
            this.mysql_edit_data("INSERT INTO comments (Ticket_id, Text, Reporter) VALUES (@id, @text, @reporter)", new_data);
            UpdateView?.Invoke(this, new EventArgs());
        }
        public void move_state(object sender, List<string> new_data)
        {
            this.mysql_edit_data("UPDATE issues SET Current_state = @state WHERE ID = @id", 
                new Dictionary<string, string> { { "@id", new_data[0]},{"@state",new_data[1] } });
            UpdateView?.Invoke(this, new EventArgs());
        }
    }
}
