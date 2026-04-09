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

namespace Familiya_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>


    public partial class ServicePage : Page
    {
        int TotalRecordsInDatabase; // Все записи в базе (без фильтрации)

        public ServicePage()
        {
            InitializeComponent();

            var currentServices = Balahnin_autoservisEntities.GetContext().Service.ToList();
            ServiceListViev.ItemsSource = currentServices;

            ComboType.SelectedIndex = 0;
            CurrentPage = 0;
            UpdateServices();
            ChangePage(0, 0);
            this.Loaded += (s, e) =>
            {
                // Обновляем при каждом возврате на страницу
                UpdateServices();
                ChangePage(0, 0);
            };

        }
        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PageListBox.SelectedIndex >= 0)
            {
                ChangePage(0, PageListBox.SelectedIndex);
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }
        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        int CountRecords;//записи
        int CountPage;//страницы
        int CurrentPage = 0;
        int RecordsPage = 10;//все записи
        List<Service> CurrentPageList = new List<Service>();
        List<Service> TableList;
        private void ChangePage(int direction, int? selectedPage)
        {
            // Проверка на пустой список
            if (TableList == null || TableList.Count == 0)
            {
                ServiceListViev.ItemsSource = null;
                PageListBox.Items.Clear();
                TBCount.Text = "0";
                TBAllRecords.Text = " из 0";
                return;
            }

            // Расчет количества страниц
            CountRecords = TableList.Count;
            CountPage = (int)Math.Ceiling((double)CountRecords / RecordsPage);

            // Навигация
            if (selectedPage.HasValue && selectedPage >= 0 && selectedPage < CountPage)
            {
                CurrentPage = selectedPage.Value;
            }
            else
            {
                if (direction == 1 && CurrentPage > 0) // Влево
                {
                    CurrentPage--;
                }
                else if (direction == 2 && CurrentPage < CountPage - 1) // Вправо
                {
                    CurrentPage++;
                }
                else
                {
                    // Если листать некуда, просто обновляем отображение
                    UpdatePageDisplay();
                    return;
                }
            }

            // Обновляем отображение
            UpdatePageDisplay();
        }

        private void UpdatePageDisplay()
        {
            // Очищаем текущий список
            CurrentPageList.Clear();

            // Заполняем данными текущей страницы
            int startIndex = CurrentPage * RecordsPage;
            int endIndex = Math.Min(startIndex + RecordsPage, CountRecords);

            for (int i = startIndex; i < endIndex; i++)
            {
                CurrentPageList.Add(TableList[i]);
            }

            // Обновляем список страниц
            PageListBox.Items.Clear();
            for (int i = 1; i <= CountPage; i++)
            {
                PageListBox.Items.Add(i);
            }

            // Выбираем текущую страницу
            if (CountPage > 0 && CurrentPage >= 0 && CurrentPage < CountPage)
            {
                PageListBox.SelectedIndex = CurrentPage;
            }

            // Обновляем счетчик
            TBCount.Text = endIndex.ToString();
            TBAllRecords.Text = " из " + CountRecords.ToString();

            // Обновляем ListView
            ServiceListViev.ItemsSource = CurrentPageList;
            ServiceListViev.Items.Refresh();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Service));
        }
        private void UpdateServices()
        {
            var currentServices = Balahnin_autoservisEntities.GetContext().Service.ToList();
            if (ComboType.SelectedIndex==0)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 0 && Convert.ToInt32(p.Discount) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 0 && Convert.ToInt32(p.Discount) < 5)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 5 && Convert.ToInt32(p.Discount) < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 15 && Convert.ToInt32(p.Discount) < 30)).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 30 && Convert.ToInt32(p.Discount) < 75)).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.Discount) >= 75 && Convert.ToInt32(p.Discount) < 100)).ToList();
            }
            currentServices = currentServices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            ServiceListViev.ItemsSource = currentServices.ToList();
            
            if(RButtonDown.IsChecked.Value)
            {
                currentServices = currentServices.OrderByDescending(p => p.Cost).ToList();
            }
            if(RButtonUp.IsChecked.Value)
            {
                currentServices = currentServices.OrderBy(p => p.Cost).ToList();
            }
            ServiceListViev.ItemsSource = currentServices;
            TableList = currentServices;
            TotalRecordsInDatabase = currentServices.Count; // Например: 450

        }


        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Visibility == Visibility.Visible)
            {
                Balahnin_autoservisEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListViev.ItemsSource = Balahnin_autoservisEntities.GetContext().Service.ToList();
            }
        }
        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new SignUpPage((sender as Button).DataContext as Service));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
            ChangePage(0, 0);
            CurrentPage = 0;

        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
            ChangePage(0, 0);
            CurrentPage = 0;

        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
            ChangePage(0, 0);
            CurrentPage = 0;

        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
            ChangePage(0, 0);
            CurrentPage = 0;

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentService = (sender as Button).DataContext as Service;
            var currentClientServices = Balahnin_autoservisEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ServiceID == currentService.ID).ToList();
            if (currentClientServices.Count != 0)
            {
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу");
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Balahnin_autoservisEntities.GetContext().Service.Remove(currentService);
                        Balahnin_autoservisEntities.GetContext().SaveChanges();
                        ServiceListViev.ItemsSource = Balahnin_autoservisEntities.GetContext().Service.ToList();
                        UpdateServices();
                        ChangePage(0, 0);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }
    }
}
