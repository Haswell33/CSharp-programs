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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Window
    {
        public event EventHandler UpdateTopPanel; // Kiedy do panelu dojdzie nowy ticket
        public event EventHandler<List<double>> UpdateSize; // Zmiana rozmiaru okna
        public event EventHandler<AdminPanel> OpenAdminPanel; // Otwarcie panelu admina
        public event EventHandler<Edit_ticket> OpenEditTicket; // Otwarcie panelu edycji ticketa
        public event EventHandler<EditUser> OpenEditUser; // Otwarcie panelu edycji użytkownika
        public event EventHandler<Dictionary<string, string>> CreateIssue; // Wylanie danych nowego ticketa
        public event EventHandler<Dictionary<string, string>> EditIssue; // Wyslanie danych do edycji ticketa
        public event EventHandler<Dictionary<string, string>> CreateUser; // Wysłanie danych do stwożenia nowgo użytkownika
        public event EventHandler<Dictionary<string, string>> EditUser;  // Wysłanie danyh do edycji użytkownika
        public event EventHandler<Dictionary<string, string>> CreateComm; // Wysłanie danych nowego komentarza

        private TopPanel top_panel = new TopPanel(); //Górny panel
        private ticket_viewer view_tic = new ticket_viewer(); // Panel podgladu ticketów
        private OneWayTicket show_ticket = new OneWayTicket(); // Panel podglądu konkretnego ticketa
        private DBConnectSelect mysql_connect_select; // Klasa do wyjmowania danych z bazy
        private DBConnectEdit mysql_connect_edit; // Klasa do edycji danych w bazie
        public HomePage(CurrentUser cur_user)
        {
            InitializeComponent();
            // Podpięcie wydarzeń zmiany rozmiaru okna
            SizeChanged += WindowOnSizeChanged;
            UpdateSize += view_tic.WindowOnSizeChanged;
            UpdateSize += show_ticket.WindowOnSizeChanged;

            // Stworzenie nowego łącznika do bazy danych i podpięcie jego wydarzeń
            mysql_connect_select = new DBConnectSelect(cur_user, top_panel, view_tic, show_ticket);
            UpdateTopPanel += mysql_connect_select.init_top_panel;
            mysql_connect_select.ChangeContentToFilter += change_content_to_filter;
            mysql_connect_select.ChangeContentToIssue += change_content_to_issue;
            OpenAdminPanel += mysql_connect_select.open_admin_panel;
            OpenEditTicket += mysql_connect_select.open_edit_ticket;
            OpenEditUser += mysql_connect_select.open_edit_user;

            // Stworzenie nowego edytora bazy danych i podpięcie jego wydarzeń
            mysql_connect_edit = new DBConnectEdit(cur_user);
            EditIssue += mysql_connect_edit.update_ticket_db;
            mysql_connect_edit.UpdateView += update_view;
            CreateIssue += mysql_connect_edit.insert_ticket_db;
            EditIssue += mysql_connect_edit.update_ticket_db;
            CreateUser += mysql_connect_edit.insert_user_db;
            EditUser += mysql_connect_edit.update_user_db;
            CreateComm += mysql_connect_edit.insert_comm_db;

            // Podpięcie wydarzeń do obsługi filtru ticketów
            view_tic.OpenNewIssue += mysql_connect_select.open_new_issue;
            view_tic.ShowSearchResult += mysql_connect_select.show_search_result;

            // Podpięcie wydarzeń do obsługi wyświetlenia ticketa
            show_ticket.OpenEditTicket += WindowOpenCreateTicket;
            show_ticket.AddComm += insert_comm;
            show_ticket.UpdateState += mysql_connect_edit.move_state;

            // Podpięcie wydarzeń do górnego panelu
            top_panel.OpenAdmin += WindowOpenAdmin;
            top_panel.OpenCreateTicket += WindowOpenCreateTicket;
            top_panel.OpenFilter += mysql_connect_select.WindowOpenFilter;
            top_panel.OpenIssue += mysql_connect_select.WindowOpenIssue;
            top_panel.CloseIssuePage += mysql_connect_select.WindowOpenFilter;
            
            // Wywołanie wydarzenia do odświeżenia górnego panelu
            UpdateTopPanel?.Invoke(this,new EventArgs());

            // Ustwienie obecnego kontentu na prawidłowy
            view_top.Content = top_panel;
            view_content.Content = view_tic;
        }
        private void WindowOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateSize?.Invoke(this, new List<double>() { this.ActualHeight, this.ActualWidth});
        }
        // Odświeżenie widoku 
        private void update_view(object sender, EventArgs args)
        {
            if (view_content.Content == view_tic)
                mysql_connect_select.WindowOpenFilter(this, args);
            else
                mysql_connect_select.WindowOpenIssue(this, show_ticket.id);
        }

        private void WindowOpenAdmin(object sender, EventArgs args)
        {
            AdminPanel new_pan = new AdminPanel();
            new_pan.OpenEditPanel += WindowOpenEditUser;
            OpenAdminPanel?.Invoke(this, new_pan);
        }
        private void WindowOpenEditUser(object sender, string id)
        {
            EditUser edit_user = new EditUser();
            edit_user.id = id;
            edit_user.CreateUser += insert_user;
            edit_user.EditThatUser += update_user;
            OpenEditUser?.Invoke(this,edit_user);
        }
        private void WindowOpenCreateTicket(object sender, string id)
        {
            Edit_ticket edit_tocket = new Edit_ticket();
            edit_tocket.id = id;
            edit_tocket.CreateTicket += insert_ticket;
            edit_tocket.EditTicket += update_ticket;
            OpenEditTicket?.Invoke(this, edit_tocket);
        }
        private void change_content_to_filter(object sender, EventArgs args)
        {
            view_content.Content = view_tic;
        }
        private void change_content_to_issue(object sender, EventArgs args)
        {
            view_content.Content = show_ticket;
        }
        private void update_ticket(object sender, Dictionary<string, string> new_data)
        {
            EditIssue?.Invoke(this, new_data);

        }
        private void insert_ticket(object sender, Dictionary<string, string> new_data)
        {
            CreateIssue?.Invoke(this, new_data);
        }
        private void update_user(object sender, Dictionary<string, string> new_data)
        {
            EditUser?.Invoke(this, new_data);

        }
        private void insert_user(object sender, Dictionary<string, string> new_data)
        {
            CreateUser?.Invoke(this, new_data);
        }
        private void insert_comm(object sender, Dictionary<string, string> new_data)
        {
            CreateComm?.Invoke(this, new_data);
        }



    }
}
