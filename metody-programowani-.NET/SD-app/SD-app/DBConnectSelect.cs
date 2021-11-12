using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Windows;

namespace WpfApp1
{
    // Klasa do wyciągania danych z bazy
    class DBConnectSelect
    {
        public event EventHandler ChangeContentToIssue;
        public event EventHandler ChangeContentToFilter;
        private TopPanel top_panel;
        private ticket_viewer view_tic;
        private OneWayTicket show_ticket;
        private readonly CurrentUser cur_user;
        string connStr = "server=localhost;user=root;database=ticket_system;port=3306;password=";
        public DBConnectSelect(CurrentUser new_user, TopPanel top_panel, ticket_viewer view_tic, OneWayTicket show_ticket)
        {
            this.cur_user = new_user;
            this.top_panel = top_panel;
            this.view_tic = view_tic;
            this.show_ticket = show_ticket;
        }
        public List<List<string>> mysql_select(string query, Dictionary<string, string> query_values)
        {
            List<List<string>> return_value = new List<List<string>>();
            query += " LIMIT 100";
            
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
        public void init_top_panel(object new_sender, EventArgs e)
        {
            top_panel.initialize_top_panel(cur_user.roles);
        }
        private List<string> get_name_of_column(List<List<string>> raw_data)
        {
            List<string> return_list = new List<string>();
            foreach (List<string> one_row in raw_data)
                return_list.Add(one_row[0]);
            return return_list;
        }
        public void open_new_issue(object new_sender, string id)
        {
            top_panel.go_to_Click(id);
        }
        public void WindowOpenFilter(object sender, EventArgs args)
        {
            List<string> tytles = get_name_of_column(this.mysql_select("SHOW COLUMNS FROM issues --", null));
            List<List<string>> new_values = this.mysql_select("SELECT * FROM issues", null);

            int where_ass = tytles.IndexOf("Assigned");
            int where_rep = tytles.IndexOf("Reporter");
            int where_state = tytles.IndexOf("Current_state");
            foreach(List<string> one_row in new_values)
            {
                one_row[where_ass] = get_full_name(one_row[where_ass]);
                one_row[where_rep] = get_full_name(one_row[where_rep]);
                one_row[where_state] = this.mysql_select("SELECT Current_state FROM flow WHERE ID='" + one_row[where_state] + "'", null)[0][0];
            }

            view_tic.Initialize_viewer(tytles, new_values);
            //view_tic.Initialize_viewer(get_name_of_column(this.mysql_select("SHOW COLUMNS FROM issues --", null)), this.mysql_select("SELECT * FROM issues",null));
            ChangeContentToFilter?.Invoke(this, args);
        }
        public void WindowOpenIssue(object sender, string id)
        {
            List<string> raw_data = this.mysql_select("SELECT ID, Title, Description, Assigned, Reporter, " +
                "Type, Current_state, Priority, Create_date, Edit_date FROM issues WHERE ID='"+id+"'", null)[0];
            Dictionary<string, string> values = new Dictionary<string, string>() {
                {"@id",raw_data[0] },
                {"@title",raw_data[1] },
                {"@desc",raw_data[2] },
                {"@type",raw_data[5] },
                {"@curr_state",this.mysql_select("SELECT Current_state FROM flow WHERE ID='"+ raw_data[6] + "'",null)[0][0] },
                {"@prio",raw_data[7] },
                {"@date_created",raw_data[8] },
                {"@date_updated",raw_data[9] }
            };
            values["@full_name_ass"] = get_full_name(raw_data[3]);
            values["@full_name_rep"] = get_full_name(raw_data[4]);


            Dictionary<string, string> next_steps = new Dictionary<string, string>();
            foreach (List<string> current_state in this.mysql_select("SELECT Next_state FROM flow WHERE Current_state='" +
                 values["@curr_state"] + "' AND Type_ticket='" + raw_data[5] + "'", null))
            {
                List<List<string>> new_id = this.mysql_select("SELECT ID FROM flow WHERE Current_state='" +
                        current_state[0] + "' AND Type_ticket='" + raw_data[5] + "'", null);
                if (new_id.Count > 0)
                    next_steps[current_state[0]] = new_id[0][0];
            }


            List<List<string>> raw_comm = this.mysql_select("SELECT Text, Reporter, Create_date FROM comments WHERE Ticket_id='"+ raw_data[0] + "'",null);
            List<Dictionary<string, string>> all_comm = new List<Dictionary<string, string>>();
            foreach(List<string> cur_com in raw_comm) 
            {
                Dictionary<string, string> temp_dic = new Dictionary<string, string>() {
                    {"@text_com", cur_com[0]},
                    {"@add_time", cur_com[2]}
                };
                temp_dic["@owner"] = get_full_name(cur_com[1]);
                all_comm.Add(temp_dic);
            }

            show_ticket.initialize_issue_page(values,next_steps,all_comm);
            ChangeContentToIssue?.Invoke(sender, new EventArgs());
        }
        public void open_admin_panel(object sender, AdminPanel cur_panel)
        {
            cur_panel.initialize_admin_page(this.get_name_of_column(this.mysql_select("SHOW COLUMNS FROM users --",null)), 
                this.mysql_select("SELECT ID, Full_name, Login, case when 1=1 then '*****' end, Roles FROM users", null));
            cur_panel.Show();
        }
        public void open_edit_ticket(object sender, Edit_ticket new_edit_ticket)
        {
            Dictionary<string, string> values;
            if (!new_edit_ticket.id.Equals(""))
            {
                List<string> raw_data = this.mysql_select("SELECT Title, Description, Assigned, Type, Priority " +
                    "FROM issues WHERE ID='" + new_edit_ticket.id + "'", null)[0];
                values = new Dictionary<string, string>() {
                    {"@title",raw_data[0] },
                    {"@desc",raw_data[1] },
                    {"@type",raw_data[3] },
                    {"@prio",raw_data[4] },
                };
                List<string> temp = this.mysql_select("SELECT Full_name, Login FROM users WHERE ID='" + raw_data[2] + "'", null)[0];
                values["@assigment"] = temp[0] + " (" + temp[1] + ")";
            }
            else
                values = null;

            Dictionary<string, string> assigment = new Dictionary<string, string>();
            foreach (List<string> cur_ass in this.mysql_select("SELECT ID, Full_name, Login FROM users WHERE Roles not like '%customer%'", null))
            {
                assigment[cur_ass[1] + " (" + cur_ass[2] + ")"] = cur_ass[0];
            }

            new_edit_ticket.initialize_edit_ticket(values,assigment,this.get_name_of_column(this.mysql_select("SELECT Name FROM types",null)));
            new_edit_ticket.Show();
        }
        public void open_edit_user(object sender, EditUser new_edit_user)
        {
            Dictionary<string, string> values;
            if (!new_edit_user.id.Equals(""))
            {
                List<string> raw_data = this.mysql_select("SELECT Full_name, Login, Pass, Roles " +
                    "FROM users WHERE ID='" + new_edit_user.id + "'", null)[0];
                values = new Dictionary<string, string>() {
                    {"@name",raw_data[0] },
                    {"@login",raw_data[1] },
                    {"@password",raw_data[2] },
                    {"@role",raw_data[3] }
                };
            }
            else
                values = null;

            new_edit_user.initialize_edit_user(values, this.get_name_of_column(this.mysql_select("SELECT Role FROM users_roles", null)));
            new_edit_user.Show();
        }
        public void show_search_result(object new_sender, string text)
        {
            text = "%" + text + "%";

            List<string> tytles = get_name_of_column(this.mysql_select("SHOW COLUMNS FROM issues --", null));
            List<List<string>> new_values = this.mysql_select("SELECT * FROM issues WHERE ID like @text OR Title like @text OR Description like " +
                "@text", new Dictionary<string, string>() { { "@text", text } });

            int where_ass = tytles.IndexOf("Assigned");
            int where_rep = tytles.IndexOf("Reporter");
            int where_state = tytles.IndexOf("Current_state");
            foreach (List<string> one_row in new_values)
            {
                one_row[where_ass] = get_full_name(one_row[where_ass]);
                one_row[where_rep] = get_full_name(one_row[where_rep]);
                one_row[where_state] = this.mysql_select("SELECT Current_state FROM flow WHERE ID='" + one_row[where_state] + "'", null)[0][0];
            }

            view_tic.Initialize_viewer(tytles, new_values);
        }
        public string get_full_name (string id)
        {
            List<string> temp = this.mysql_select("SELECT Full_name, Login FROM users WHERE ID='" + id + "'", null)[0];
            return temp[0] + " (" + temp[1] + ")";
        }


    }
}
