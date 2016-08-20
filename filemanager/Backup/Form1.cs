using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DirFileProject
{
    public partial class Form1 : Form
    {
        string FPath;
        string backPath;
        List<string> ls = new List<string>();
        public Form1()
        {
            InitializeComponent();

        }


        private void CopyDir(DirectoryInfo soursDir, DirectoryInfo destDir)
        {
            while (true)
            {
                CreateDir(soursDir, destDir);

                //теперь проверяем наличие в ней папок
                DirectoryInfo[] dirs = soursDir.GetDirectories();
                if (dirs.Length > 0)
                {
                    foreach (DirectoryInfo di in dirs)
                    {
                        DirectoryInfo dir = new DirectoryInfo(destDir.FullName.ToString() + "\\" + di.Name.ToString());
                        CopyDir(di, dir);
                    }
                    break;
                }
                else break;
            }
        }

        //создаем папку
        private void CreateDir(DirectoryInfo soursDir, DirectoryInfo destDir)
        {
            if (!destDir.Exists) destDir.Create();

            //проверяем наличие файлов
            FileInfo[] fls = soursDir.GetFiles();
            if (fls.Length > 0) //копируем если есть
                foreach (FileInfo fi in fls)
                    fi.CopyTo(destDir.FullName.ToString() + "\\" + fi.Name.ToString(), true);
        }

        void GetLocDir()
        {
            listView1.Items.Clear();
            this.Text = "Мой компьютер";
            String[] LogicalDrives = Environment.GetLogicalDrives();
            foreach (string s in LogicalDrives)
            {

                listView1.Items.Add(s, 1);
                ls.Add(s);
            }
        }

        void GetFiles()
        {
            listView1.BeginUpdate();

            try
            {

                string[] dirs = Directory.GetDirectories(FPath);
                ls.Clear();

                try
                {
                    DirectoryInfo dInfo = new DirectoryInfo(FPath);
                    backPath = dInfo.Parent.FullName;
                }
                catch { }

                listView1.Items.Clear();

                foreach (string s in dirs)
                {
                    //   if ((File.GetAttributes(s) & FileAttributes.Hidden) == FileAttributes.Hidden)
                    //       continue;

                    string dirname = System.IO.Path.GetFileName(s);
                    listView1.Items.Add(dirname, 1);
                    ls.Add(s);
                }

                string[] files = Directory.GetFiles(FPath);
                foreach (string s in files)
                {
                    string filename = System.IO.Path.GetFileName(s);
                    listView1.Items.Add(filename, 0);
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            listView1.EndUpdate();
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            ListViewItem item = listView1.SelectedItems[0];
            if (item.ImageIndex == 1)
            {
                string it = item.Text;
                string title = "";
                foreach (string s in ls)
                {
                    try
                    {
                        if (s.Substring(s.Length - it.Length, it.Length) == it)
                        {
                            FPath = s;
                            title = s;
                        }
                    }
                    catch { }
                }
                try
                {
                    string[] dirs = Directory.GetDirectories(FPath);
                    this.Text = title;
                    GetFiles();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            else if (item.ImageIndex == 0)
            {
                string start = this.Text + "\\" + item.Text;

                System.Diagnostics.Process.Start(start);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            bool b = false;
            String[] LogicalDrives = Environment.GetLogicalDrives();
            foreach (string s in LogicalDrives)
            {
                if (backPath == s && FPath == s) b = true;
            }
            if (backPath != null && !b)
            {
                this.Text = backPath;
                FPath = backPath;
                GetFiles();
            }
            else
            {
                GetLocDir();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetLocDir();
        }

        public void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Microsoft.VisualBasic.Interaction.InputBox("Введите в поле путь для копирования",
                "Укажите путь", "D:\\", 10, 10);
            ListViewItem item = listView1.SelectedItems[0];
            string copy = this.Text + "\\" + item.Text;
            try
            {
                if (item.ImageIndex == 1)
                {
                    DirectoryInfo soursDir = new DirectoryInfo(copy); //папка из которой копировать
                    DirectoryInfo destDir = new DirectoryInfo(path + "new" + item.Text); //куда копируешь
                    System.Threading.Thread MyThread1 =
                    new System.Threading.Thread(delegate() { CopyDir(soursDir, destDir); });
                    MyThread1.Start();


                }
                else
                {
                    System.Threading.Thread MyThread1 =
                       new System.Threading.Thread(delegate() { File.Copy(copy, path + "new" + item.Text); });
                    MyThread1.Start();


                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();
        }


        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = listView1.SelectedItems[0];
            string message = "Вы действительно хотите удалить " + item.Text + "?";
            string caption = "Удаление файла";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            try
            {

                result = MessageBox.Show(message, caption, buttons);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {

                    string delete = this.Text + "\\" + item.Text;
                    if (item.ImageIndex == 1)
                    {
                        Directory.Delete(delete, true);

                    }
                    else File.Delete(delete);


                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetFiles();
        }

        private void переместитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Microsoft.VisualBasic.Interaction.InputBox("Введите в поле путь для перемещения",
                "Укажите путь", "D:\\", 10, 10);
            ListViewItem item = listView1.SelectedItems[0];
            string move = this.Text + "\\" + item.Text;
            try
            {
                if (item.ImageIndex == 1)
                {
                    DirectoryInfo soursDir = new DirectoryInfo(move); //папка из которой копировать
                    DirectoryInfo destDir = new DirectoryInfo(path + item.Text); //куда копируешь                    
                    CopyDir(soursDir, destDir);

                    Directory.Delete(move, true);

                }
                else
                {
                    File.Move(move, path + item.Text);

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string name = Microsoft.VisualBasic.Interaction.InputBox("Введите в поле путь и название файла",
                   "Введите данные", "D:\\", 10, 10);
                File.Create(name);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();
        }

        private void папкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string name = Microsoft.VisualBasic.Interaction.InputBox("Введите в поле путь и название файла",
                  "Введите данные", "D:\\", 10, 10);
                Directory.CreateDirectory(name);

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            GetFiles();
        }
    }
}
