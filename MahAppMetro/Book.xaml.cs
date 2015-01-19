using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
    /// Interaction logic for Book.xaml
    /// </summary>
    public partial class Book : UserControl
    {
        public Book()
        {
            InitializeComponent();
        
        }
        private SQLiteConnection conn;
        private SQLiteCommand cmd;
        private SQLiteDataAdapter DBAdapter;        
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private MainWindow ParentWindow;
        public Book(string title,MainWindow window)
        {
            InitializeComponent();
            conn = new SQLiteConnection("Data Source=Metrobook.db;Version=3;New=True;Compress=True;");
            BookTitle.Text = title;
            ParentWindow = window;
            AddAllNotes();   
        }        
        private void AddNewEmpty()
        {
            Note newNote = new Note(this,true,ParentWindow);
            ((MainWindow)MahAppMetro.MainWindow.GetWindow(this)).SelectedNote = newNote;
            newNote.lastTIme = DateTime.Now;          
            ChapterStack.Children.Add(newNote);

        }       
        public void DeleteBook(string NoteName){
            conn.Open();
            cmd = conn.CreateCommand();
            string CommandText = "Delete from notes where Name=@name And BookName=@bookName";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SQLiteParameter("@name",NoteName ));            
            //cmd.Parameters.Add(new SQLiteParameter("@bookName",BookTitle.Text);
            cmd.CommandText = CommandText;
           // cmd.ExecuteNonQuery();
            conn.Close();                
        }
        public void AddAllNotes()
        {
            conn.Open();
            cmd = conn.CreateCommand();
            string CommandText = "Select * from Notes Where BookName='"+BookTitle.Text+"'";
            DBAdapter = new SQLiteDataAdapter(CommandText, conn);
            DS.Reset();
            DBAdapter.Fill(DS);
            conn.Close();
            foreach (DataRow row in DS.Tables[0].Rows)
            {
                Note note = new Note(row.Field<string>(0), row.Field<string>(1),this,false,ParentWindow);
                note.lastTIme = DateTime.FromFileTime(row.Field<long>(3));
                ChapterStack.Children.Add(note);
            }
   
        }
        public void SelfDestruct()
        {
            conn.Open();
            cmd = conn.CreateCommand();
            string CommandText = "Delete from NoteBooks where Name=@name";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SQLiteParameter("@name", BookTitle.Text));
            cmd.CommandText = CommandText;
            cmd.ExecuteNonQuery();
            conn.Close();
            this.Visibility = Visibility.Collapsed;
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            AddNewEmpty();
        }
    
    }
}
