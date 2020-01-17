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
using System.Data.SqlClient;

namespace RSS
{
    //Класс для списка
    public class Data
    {
        public string EmployeeName { get; set; }
        public double EmployeeSalary { get; set; }
        public bool Active { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //===========================   Строка подключения БД   ================================
        public string ConnectionSrting { get; } = @"Data Source=DESKTOP-SJE2N6P\SQLEXPRESS;Initial Catalog=SOBES_RSS;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //======================================================================================

        //Список сотрудников
        List<Data> EmployeeList = new List<Data>();

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Показать
        /// Загрузка из БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ShowEmployees(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection();

            try
            {
                //Открываем подключение
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand();

                //Получаем 
                command.CommandText = "";//-----------------------------

                command.Connection = connection;

                SqlDataReader dataReader = command.ExecuteReader();

                //while (dataReader.Read())
                //{ Runners.Items.Add($"{dataReader[0]} {dataReader[1]}"); }


            }
            catch (SqlException ex)
            {
                //Выводим сообщение об ошибке
                MessageBox.Show(Convert.ToString(ex));
            }
            finally
            {
                //В любом случае закрываем подключение
                connection.Close();
            }
        }

        /// <summary>
        /// Поиск
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartSearch(object sender, RoutedEventArgs e)
        {

        }
    }
}
