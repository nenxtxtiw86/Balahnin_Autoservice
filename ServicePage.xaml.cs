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
        public ServicePage()
        {
            InitializeComponent();

            var currentServices = Balahnin_avtoservisEntities.GetContext().Service.ToList();
            ServiceListViev.ItemsSource = currentServices;

            ComboType.SelectedIndex = 0;
            UpdateSerices();
        }

        private void UpdateSerices()
        {
            var currentServices = Balahnin_avtoservisEntities.GetContext().Service.ToList();

            if(ComboType.SelectedIndex==0)
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
                ServiceListViev.ItemsSource = currentServices.OrderByDescending(p => p.Cost).ToList();
            }
            if(RButtonUp.IsChecked.Value)
            {
                ServiceListViev.ItemsSource = currentServices.OrderBy(p => p.Cost).ToList();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSerices();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateSerices();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateSerices();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSerices();
        }

    }
}
