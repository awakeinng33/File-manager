using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace DirFileProject
{
    public partial class Form1 : Form
    {
        string FPath;
        string backPath;

        Dictionary<string,Object> lsObject = new Dictionary<string,Object>();
        
        public Form1()
        {
            InitializeComponent();
        }


        void GetLocDir()
        {
            listView1.Items.Clear();
            this.Text = "My Computer";
            String[] LogicalDrives = Environment.GetLogicalDrives();
            foreach (string s in LogicalDrives)
            {
                listView1.Items.Add(s, 1);
                lsObject.Add(s, new ObjectDir(s));
            }
        }



       public void Refresh(bool flag)
        {
            listView1.BeginUpdate();

            try
            {
                string[] dirs = Directory.GetDirectories(FPath);

                lsObject.Clear();

                try
                {
                    DirectoryInfo dInfo = new DirectoryInfo(FPath);
                    backPath = dInfo.Parent.FullName;
                }
                catch { }

                listView1.Items.Clear();

                foreach (string s in dirs)
                {
                    string dirname = System.IO.Path.GetFileName(s);
                    listView1.Items.Add(dirname, 1);
                    lsObject.Add(dirname, new ObjectDir(s));
                }


                if (flag)
                {
                    var sortFiles = Directory.GetFiles(this.Text).OrderBy(f => File.GetCreationTime(f));
                    foreach (string s in sortFiles)
                    {
                        string filename = System.IO.Path.GetFileName(s);
                        listView1.Items.Add(filename, 0);
                    }
                    listView1.EndUpdate();
                    return;

                }

                string[] files = Directory.GetFiles(FPath);
            
                foreach (string s in files)
                {
                    string filename = System.IO.Path.GetFileName(s);
                    listView1.Items.Add(filename, 0);
                    lsObject.Add(filename, new ObjectFile(s));
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            listView1.EndUpdate();
        }

        //открытие файла или папки
        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            ListViewItem item = listView1.SelectedItems[0];

            if (item.ImageIndex == 1)
            {
                string it = item.Text;
                string title = "";

                FPath = lsObject[item.Text].pathOfFile;
                title = lsObject[item.Text].pathOfFile;

                try
                {
                    string[] dirs = Directory.GetDirectories(FPath);
                    this.Text = title;
                    Refresh(false);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            else if (item.ImageIndex == 0)
            {
                string start = lsObject[item.Text].pathOfFile;

                System.Diagnostics.Process.Start(start);
            }
        }


        private void buttonBack_Click(object sender, EventArgs e)
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
                Refresh(false);
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


        private void createFileOrDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = sender as ToolStripMenuItem;
            try
            {
                string name = Microsoft.VisualBasic.Interaction.InputBox("Enter the filename or directory :",
                   "Enter the data :");
                if (t.Text == "File") File.Create(this.Text + "\\" + name);
                else Directory.CreateDirectory(this.Text + name);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            Refresh(false);
        }


        private void dateToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            Refresh(true); 
        }


        private void nameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Sorting == SortOrder.Ascending)
                listView1.Sorting = SortOrder.Descending;
            else
                listView1.Sorting = SortOrder.Ascending;
        }


        public void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            contextMenuStrip2.Show();
            try
            {
                ListViewItem item = this.listView1.SelectedItems[0];
                string copy = this.Text + "\\" + item.Text;
                string newName = Microsoft.VisualBasic.Interaction.InputBox("Enter the new name :",
                       "Enter the data :");
                lsObject[item.Text].rename(this.Text + "\\" + newName);
                Refresh(false);
            }
            catch (ArgumentOutOfRangeException) { MessageBox.Show("File or directory not selected!"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

       public void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            ListViewItem item = this.listView1.SelectedItems[0];
            try
            {
                lsObject[item.Text].getProperties();///
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void moveOrCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item = this.listView1.SelectedItems[0];
                ToolStripMenuItem t = sender as ToolStripMenuItem;
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Select the place for operation :";
                folderBrowserDialog.ShowNewFolderButton = true;
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    lsObject[item.Text].moveOrCopy(folderBrowserDialog.SelectedPath,t.Text,item.Text);
                }

            }
            catch (ArgumentOutOfRangeException) { MessageBox.Show("File or directory not selected!"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            Refresh(false);
        }

        public void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item = this.listView1.SelectedItems[0];
                string message = "Are you realy want to delete " + item.Text + "?";
                string caption = "Delete";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                try
                {
                    result = MessageBox.Show(message, caption, buttons);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        lsObject[item.Text].remove();
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                Refresh(false);
            }
            catch (ArgumentOutOfRangeException) { MessageBox.Show("File or directory not selected!"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void refreshToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Refresh(false);
        }
    }

    abstract class Object
    {
        public string pathOfFile;

        public Object(string path)
        {
            pathOfFile = path;
        }

        public abstract void getProperties();

        public abstract void remove();

        public abstract void rename(string newName);

        public abstract void moveOrCopy(string pathCopy, string typeAction, string newName);
        
    }

    class ObjectFile : Object
    {
        public ObjectFile(string path) : base(path) { }

        public override void remove()
        {
            File.Delete(pathOfFile);
        }

        public override void rename(string newName)
        {
            File.Move(pathOfFile, newName);
        }

        public override void getProperties()
        {
            FileInfo file = new FileInfo(pathOfFile);
            DirectoryInfo sDir = new DirectoryInfo(pathOfFile);
            FileSecurity fileSecurity = file.GetAccessControl();
            IdentityReference identityReference = fileSecurity.GetOwner(typeof(NTAccount));

            MessageBox.Show("File size: " + file.Length + "bytes\n"
                    + "File creation time: " + file.CreationTime + "\n" +
                    "Full path: " + file.FullName + "\n" +
                     "Exstension: " + file.Extension + "\n" +
                      "Name of owner: " + identityReference.Value);
            
        }

        public override void moveOrCopy(string pathCopy, string typeAction, string newName)
        {
            try
            {
       
                if (typeAction == "Move")
                {
                    File.Move(pathOfFile, pathCopy + "\\" + newName);
                }
                else
                {
                    System.Threading.Thread MyThread1 =
                    new System.Threading.Thread(delegate() { File.Copy(pathOfFile, pathCopy + "\\" + newName); });
                    MyThread1.Start();
                }
                
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }   
        }
    }

    class ObjectDir : Object
    {
        private int count_files = 0;
        private int count_dirs = 0;

        public ObjectDir(string path) : base(path) { }

        public override void remove()
        {
            Directory.Delete(pathOfFile, true);
        }

        public override void rename(string newName)
        {
            Directory.Move(pathOfFile, newName);
        }

        public override void getProperties()
        {
            FileInfo file = new FileInfo(pathOfFile);
            DirectoryInfo sDir = new DirectoryInfo(pathOfFile);
            FileSecurity fileSecurity = file.GetAccessControl();
            IdentityReference identityReference = fileSecurity.GetOwner(typeof(NTAccount));

            long size = getDirSize(sDir);
            MessageBox.Show("Directory size: " + size + " bytes\n" +
                    "Creation time: " + sDir.CreationTime + "\n" + "Full name: "
                    + sDir.FullName + "\n" + "Count of files:" + count_files + "\n" +
                    "Count of directories: " + count_dirs);
            count_dirs = 0;
            count_files = 0;
            
        }

        public override void moveOrCopy(string pathCopy, string typeAction, string newName)
        {
            DirectoryInfo soursDir = new DirectoryInfo(pathOfFile); //папка из которой копировать
            DirectoryInfo destDir = new DirectoryInfo(pathCopy + "\\" + newName); //куда копируешь                    
            if (typeAction == "Move")
            {
                this.copyDir(soursDir, destDir);
                Directory.Delete(pathOfFile, true);
            }
            else
            {
                System.Threading.Thread MyThread1 =
                new System.Threading.Thread(delegate() { this.copyDir(soursDir, destDir); });
                MyThread1.Start();
            }
        }

        private void createDir(DirectoryInfo sourceDir, DirectoryInfo destDir)
        {
            if (!destDir.Exists) destDir.Create();

            //проверяем наличие файлов
            FileInfo[] fls = sourceDir.GetFiles();
            if (fls.Length > 0) //копируем если есть
                foreach (FileInfo fi in fls)
                    fi.CopyTo(destDir.FullName.ToString() + "\\" + fi.Name.ToString(), true);
        }

        private void copyDir(DirectoryInfo sourceDir, DirectoryInfo destDir)
        {
            while (true)
            {
                createDir(sourceDir, destDir);

                //теперь проверяем наличие в ней папок
                DirectoryInfo[] dirs = sourceDir.GetDirectories();
                if (dirs.Length > 0)
                {
                    foreach (DirectoryInfo di in dirs)
                    {
                        DirectoryInfo dir = new DirectoryInfo(destDir.FullName.ToString() + "\\" + di.Name.ToString());
                        copyDir(di, dir);
                    }
                    break;
                }
                else break;
            }
        }

        private long getDirSize(DirectoryInfo sDir)
        {
            long size = 0;
            foreach (var one_file in sDir.GetFiles())
            {
               size += one_file.Length;
               count_files++;
            }
            DirectoryInfo[] dirs = sDir.GetDirectories();
            
            foreach (var dir in dirs)
            {
                size += getDirSize(dir);
                count_dirs++;

            }

            return (size);
        }
    }



}
