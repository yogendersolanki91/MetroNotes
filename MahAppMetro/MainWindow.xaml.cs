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
using MahApps.Metro.Controls;
using Kiwi.Markdown.ContentProviders;
using System.Data.SQLite;
using System.IO;
using System.IO.Packaging;
using System.Data;

namespace MahAppMetro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:MetroWindow
    {
        #region Global Data
        string final;
        bool isEditMode = false;        
        private SQLiteConnection conn;
        private SQLiteCommand cmd;
        private SQLiteDataAdapter DBAdapter;        
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        string lastTile="";       
        private Note myTempNote = null;
        public Book SelectedBook
        {
            get
            {
                return DefualtBook;
            }
            set 
            { 
                DefualtBook = value; 
            }

        }
        public Note SelectedNote 
        {
            get
            {
                    return myTempNote;
            }
            set 
            {
                if (SelectedNote != null)
                {
                    if (SelectedNote.ChapterTitle.Text == string.Empty)
                    {
                        SelectedNote.Visibility = Visibility.Collapsed;
                        SelectedNote.SelfDestruct();                        
                    }
                    SelectedNote.Background = Brushes.Transparent;
                    SelectedNote.SelfUpdate();
                }                                
                myTempNote = value;
                myTempNote.Background = myTempNote.heighlight;
                myTempNote.GenerateHTML();
                CurrentTitle.Text = value.ChapterTitle.Text;
                ChapterDay.Text = value.lastTIme.ToLocalTime().ToShortDateString();
            }

        }
        #endregion
        public MainWindow()
        {
           InitializeComponent();
           Renderer.WebSession = Awesomium.Core.WebCore.CreateWebSession(new Awesomium.Core.WebPreferences { SmoothScrolling = true, });
           Renderer.Zoom = 60;
            conn = new SQLiteConnection("Data Source=Metrobook.db;Version=3;New=True;Compress=True;");
           LoadAllCategory();
     
        }

        private void EditMode()
        {
            Renderer.Visibility = Visibility.Collapsed;
            EditContent.Visibility = Visibility.Visible;
            if (SelectedNote != null)
            {
                EditContent.Text = SelectedNote.ChapterDescription.Text;
            }
            isEditMode = true;
        }

        public void ReadMode()
        {
            
            LoadUpdatedMD();
            Renderer.Visibility = Visibility.Visible;
            EditContent.Visibility = Visibility.Collapsed;          
            isEditMode = false;
        }

        Book DefualtBook = null;
        private void LoadAllCategory() { 
            conn.Open();
            cmd = conn.CreateCommand();
            string CommandText = "Select * from NoteBooks";
            DBAdapter = new SQLiteDataAdapter(CommandText, conn);
            DS.Reset();
            DBAdapter.Fill(DS);
            conn.Close();            
            foreach(DataRow row in DS.Tables[0].Rows)
            {
                Book newbook= new Book(row.Field<string>(0), this);
                CategoryTile cat=new CategoryTile(row.Field<string>(0),this,newbook);
                cat.DefaultCategory = Convert.ToBoolean(row.Field<long>(1));
                if (row.Field<long>(1) == 1)
                {
                    DefualtBook = newbook;
                    BookStack.Children.Add(DefualtBook);
                }
                else
                    BookStack.Children.Add(newbook);
                
                CategoryManager.Children.Add(cat);
            }
        }

        
        public void LoadUpdatedMD() {
            //MessageBox.Show(SelectedNote.ChapterTitle.Text);
            SelectedNote.GenerateHTML();
            
        }
        Note newNote = null;

        private void AddNewEmpty() 
        {
                    
        }

        private void ContentMe_Loaded(object sender, RoutedEventArgs e)
        {

        } 
      
        private void ContentMe_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           // ContentMe.LoadHTML(final);
        }
 
        private void EditContent_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            
        }

        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                if (SelectedNote != null)
                {
                    SelectedNote.ChapterDescription.Text = EditContent.Text;
                    SelectedNote.SelfUpdate();
                }
                ReadMode();
                
            }
            else
                EditMode();
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNote == null)
            {
                AddNewEmpty();
                CurrentTitle.Text = "";
            }
            else if (SelectedNote.ChapterTitle.Text != "")
            {
                SelectedNote.SelfUpdate();              
                AddNewEmpty();
                CurrentTitle.Text = "";
            }
            else {
                MessageBox.Show("There is already a empty Note");
                
            }
        }

        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNote != null)
            {
                SelectedNote.SelfDestruct();
                BookStack.Children.Remove(SelectedNote);

            }
        }

        private void ShowFlyOut_Click(object sender, RoutedEventArgs e)
        {
            Extra.IsOpen = true;
        }

        private void CurrentTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            lastTile = CurrentTitle.Text;
            SelectedNote.ChapterTitle.Text = lastTile;
          
            
        }
        public void DeletBook(CategoryTile CatTile,Book TargetBook) {
            CategoryManager.Children.Remove(CatTile);
            TargetBook.SelfDestruct();
            BookStack.Children.Remove(TargetBook);
        
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (NewBookText.Text != "")
            {
                try
                {
           
                    conn.Open();
                    cmd = conn.CreateCommand();
                    string CommandText = "Insert into NoteBooks values(@name,@isdef)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SQLiteParameter("@name", NewBookText.Text));
                    cmd.Parameters.Add(new SQLiteParameter("@isdef", 1));
                    cmd.CommandText = CommandText;
                    cmd.ExecuteNonQuery();
                    Book newbook = new Book(NewBookText.Text, this);
                    BookStack.Children.Add(new Book(NewBookText.Text, this));
                    CategoryTile cat = new CategoryTile(NewBookText.Text,this,newbook);
                    cat.isDefualt.Visibility = Visibility.Hidden;
                    CategoryManager.Children.Add(cat);
                }
                catch (Exception ex) { 
                
                }
                finally
                {
                    conn.Close();
                }

            }
            else
                MessageBox.Show("Notebook name is required");
        }
    }
}
