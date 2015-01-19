using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MahAppMetro
{
    /// <summary>
    /// Interaction logic for CategoryTile.xaml
    /// </summary>
    public partial class CategoryTile : UserControl
    {
        private MainWindow ParentWindow;
        private Book BookObject;
        public bool DefaultCategory{
            get 
            { 
                return isDefualt.Visibility.Equals(Visibility.Visible); 
            }
            set 
            {
                if (value == true)
                    isDefualt.Visibility = Visibility.Visible;
                else
                    isDefualt.Visibility = Visibility.Collapsed;
            }
        } 

        public CategoryTile(MainWindow window)
        {
            InitializeComponent();
            ParentWindow = window;
        }
        public CategoryTile(string title, MainWindow window,Book book)
        {
            InitializeComponent();
            TileHeader.Title = title;
            ParentWindow = window;
            BookObject = book;
        }

        private void RemoveCat_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ParentWindow.DeletBook(this, BookObject); ;
        }
    }
}
