using Kiwi.Markdown.ContentProviders;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
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
    /// Interaction logic for Note.xaml
    /// </summary>
    public partial class Note : UserControl
    {      
        private SQLiteConnection conn;
        private SQLiteCommand cmd;
        private SQLiteDataAdapter DBAdapter;        
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private MainWindow ParentWindow;
        private Book ParentBook ;
        public DateTime lastTIme;        
        MarkdownDeep.Markdown md = new MarkdownDeep.Markdown();
        Kiwi.Markdown.MarkdownService ser = new Kiwi.Markdown.MarkdownService(new FileContentProvider(""));
        public Brush heighlight  =(Brush)new BrushConverter().ConvertFromString("#44000000");
        string head = "<html><head>		<meta charset=\"utf-8\">		<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, minimal-ui\">		<title>GitHub Markdown CSS demo</title>		<style TYPE=\"text/css\">" + new StreamReader(AppDomain.CurrentDomain.BaseDirectory+@"\github-markdown.css").ReadToEnd() + "</style></head>	<body>		<article class=\"markdown-body\">";
        string tail = " </article></body></html>";
        public bool isNew = true;
        
        public Note(Book book,bool New,MainWindow window)
        {
            InitializeComponent();
            conn = new SQLiteConnection("Data Source=Metrobook.db;Version=3;New=True;Compress=True;");
            ParentWindow = (MainWindow)MahAppMetro.MainWindow.GetWindow(this);
            ParentBook = book;
            isNew = New;
            ParentWindow = window;

        }
        public Note(string newTitle, string Content, Book book, bool New, MainWindow window)
        {
            InitializeComponent();
            ParentWindow = window;
            ParentBook = book;
            isNew = New;
            conn = new SQLiteConnection("Data Source=Metrobook.db;Version=3;New=True;Compress=True;");
            ChapterTitle.Text = newTitle;
            ChapterDescription.Text = Content;
        }

        public void DBAddNewNote() {
            conn.Open();
            cmd = conn.CreateCommand();
            string CommandText = "Insert into Notes values(@name,@content,@bookName,@time)";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SQLiteParameter("@name",ChapterTitle.Text ));
            cmd.Parameters.Add(new SQLiteParameter("@content",ChapterDescription.Text));
            cmd.Parameters.Add(new SQLiteParameter("@bookName", ParentBook.BookTitle.Text ));
            cmd.Parameters.Add(new SQLiteParameter("@time",lastTIme.ToLocalTime().ToFileTime()));
            cmd.CommandText = CommandText;
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void GenerateHTML() {

            if (ParentWindow.Renderer!=null)
            ParentWindow.Renderer.LoadHTML(head + ser.ToHtml(ChapterDescription.Text) + tail);
            
        }
        

        public void SelfDestruct() {
            conn.Open();
            cmd = conn.CreateCommand();
            string CommandText = "Delete from Notes where NoteTime=@time";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SQLiteParameter("@time", lastTIme.ToFileTime()));
            cmd.CommandText = CommandText;
            cmd.ExecuteNonQuery();
            conn.Close();
            this.Visibility = Visibility.Collapsed;
        }

        public void SelfUpdate(string tile, long time, string desc)
        {
            conn.Open();
            cmd = conn.CreateCommand();
            string CommandText = "Update Notes set name=@name,content=@cont where notetime=@time";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SQLiteParameter("@name", tile));
            cmd.Parameters.Add(new SQLiteParameter("@time", time));
            cmd.Parameters.Add(new SQLiteParameter("@cont", desc));
            cmd.CommandText = CommandText;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void SelfUpdate()
        {
            conn.Open();
            cmd = conn.CreateCommand();
            string CommandText = "Update Notes set name=@name,content=@cont where notetime=@time";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SQLiteParameter("@name", ChapterTitle.Text));            
            cmd.Parameters.Add(new SQLiteParameter("@time", lastTIme.ToFileTime()));
            cmd.Parameters.Add(new SQLiteParameter("@cont", ChapterDescription.Text));
            cmd.CommandText = CommandText;
            cmd.ExecuteNonQuery();
            conn.Close();
         
                
            
        }
       
       
        private void TileNote_Click(object sender, RoutedEventArgs e)
        {

            if (ParentWindow.SelectedNote!=null && ParentWindow.SelectedNote.isNew && ParentWindow.SelectedNote.ChapterTitle.Text != "")
            {
                ParentWindow.SelectedNote.DBAddNewNote();
                ParentWindow.SelectedNote.isNew = false;
            }
            
            ParentWindow.SelectedNote = this;
        }
    }
}
