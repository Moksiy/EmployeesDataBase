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
        public string SalaryDate { get; set; }
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

            //Выставляем значение по умолчанию для ComboBox'а
            SelectedFilter.SelectedValue = AVGFilter;
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
                connection.ConnectionString = ConnectionSrting;

                //Открываем подключение
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand();

                //Запрос
                command.CommandText = "SELECT dbo.Employees.EmployeeName, " + $"{(SelectedFilter.SelectedValue == MAXFilter ? "MAX" : "AVG")}" +
                                      "(ISNULL(Salary,0)), dbo.Employees.Active, ISNULL(SalaryDate,'01-01-9999') " +
                                      "FROM dbo.Employees LEFT OUTER JOIN dbo.Salary ON dbo.Employees.EmployeeId = dbo.Salary.EmployeeId " +
                                      $"{(ShowDismissed.IsChecked != true ? "WHERE Active = 1 " : " ")}" +
                                      "GROUP BY EmployeeName, Active, SalaryDate " +
                                      "ORDER BY ISNULL(SalaryDate,'01-01-9999'), " + $"{(SelectedFilter.SelectedValue == MAXFilter ? "MAX" : "AVG")}" +
                                      "(ISNULL(Salary,0)) DESC";

                command.Connection = connection;

                SqlDataReader dataReader = command.ExecuteReader();

                //Очищаем список
                EmployeeList.Clear();

                //Чистим строку поиска
                Search.Text = "";

                while (dataReader.Read())
                {
                    DateTime date = Convert.ToDateTime(dataReader[3]);
                    string salaryDate = Convert.ToDouble(dataReader[1]) == 0 ? "-" : date.ToString("dd.MM.yyyy");
                    EmployeeList.Add(new Data
                    {
                        EmployeeName = dataReader[0].ToString(),
                        EmployeeSalary = Convert.ToDouble(dataReader[1]),
                        Active = Convert.ToBoolean(dataReader[2]),                        
                        SalaryDate = salaryDate
                    });
                }
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

            //Очищаем listview
            EmployeeListView.Items.Clear();

            //Выводим в listview элементы списка
            foreach (var item in EmployeeList)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = item;

                //Подсвечиваем, если ЗП работника меньше 20000
                if (item.EmployeeSalary < 20000)
                    listItem.Background = Brushes.LightYellow;

                //Уволенных работников выводим красным шрифтом
                if (item.Active == false)
                    listItem.Foreground = Brushes.Red;

                EmployeeListView.Items.Add(listItem);
            }
        }

        /// <summary>
        /// Поиск
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartSearch(object sender, RoutedEventArgs e)
        {
            if (EmployeeList.Count > 0)
            {
                //Очищаем ListView
                EmployeeListView.Items.Clear();

                //Поиск по списку
                foreach (var item in EmployeeList)
                {
                    if (item.EmployeeName.Contains(Search.Text))
                    {
                        ListViewItem listItem = new ListViewItem();

                        listItem.Content = item;

                        //Подсвечиваем, если ЗП работника меньше 20000
                        if (item.EmployeeSalary < 20000)
                            listItem.Background = Brushes.LightYellow;

                        //Уволенных работников выводим красным шрифтом
                        if (item.Active == false)
                            listItem.Foreground = Brushes.Red;

                        EmployeeListView.Items.Add(listItem);
                    }
                }
            }
        }
    }
}
