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
using System.Data.SQLite;
using System.Data;
namespace Book
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
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
        public Book(string title)
        {
            InitializeComponent();
            conn = new SQLiteConnection("Data Source=Metrobook.db;Version=3;New=True;Compress=True;");
            BookTitle.Text = title;
            AddBooks();   
        }
       
        public void AddBooks(){
            conn.Open();
            cmd = conn.CreateCommand();
            string CommandText = "Select * from Notes Where BookName='"+BookTitle.Text+"'";
            DBAdapter = new SQLiteDataAdapter(CommandText, conn);
            DS.Reset();
            DBAdapter.Fill(DS);
            conn.Close();
            foreach (DataRow row in DS.Tables[0].Rows)
            {
                Note.NewNote note = new Note.NewNote(row.Field<string>(0), row.Field<string>(1));
                ChapterStack.Children.Add(note);
            }
   
        }
    }
}
