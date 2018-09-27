using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Web;
using System.Reflection;


namespace Task
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)  //Button for selecting URL of directory for creating XML
        {
            try
            {
                FolderBrowserDialog f_url = new FolderBrowserDialog();
                f_url.RootFolder = Environment.SpecialFolder.Desktop;
                f_url.Description = "Select folder URL:";
                f_url.ShowNewFolderButton = false;

                if (f_url.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = f_url.SelectedPath + "\\";
                    button2.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: \n" + ex);
            }
        }
        private static XElement CreateXML(DirectoryInfo dir)    //Method for creating XML file
        {
            Int64 sumDirSize = 0;
            int fileNumber = 0;
            Int64 size = 0;

            //initial directory
            var xmlInfo = new XElement("Folder", new XAttribute("name", dir.Name));

            //sumarizing size of files in folder
            DirectoryInfo dirX = new DirectoryInfo(dir.ToString());
            FileSystemInfo[] filelist = dirX.GetFileSystemInfos();
            FileInfo[] fileInfo;
            fileInfo = dir.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfo.Length; i++)
            {
                try
                {
                    size += fileInfo[i].Length;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: \n" + ex);
                }
            }
            try
            {
                //get all files from initial directory
                foreach (var file in dir.GetFiles())
                {
                    //file size
                    Int64 fileSizeInBytes = new FileInfo(dir + "\\" + file.ToString()).Length;

                    //creation/modification/access time
                    DateTime creation = File.GetCreationTime(dir + "\\" + file.ToString());
                    DateTime modification = File.GetLastWriteTime(dir + "\\" + file.ToString());
                    DateTime access = file.LastAccessTime;

                    //new file
                    xmlInfo.Add(new XElement("File", new XAttribute("name", file.Name), new XAttribute("size", fileSizeInBytes + "b"), new XAttribute("creation_time", creation.ToString()), new XAttribute("last_access_time", access.ToString()), new XAttribute("modified_time", modification.ToString())));

                    sumDirSize += fileSizeInBytes;
                    fileNumber++;
                }
                //get subdirectories
                var subdirectories = dir.GetDirectories().ToList().OrderBy(d => d.Name);
                foreach (var subDir in subdirectories)
                {
                    string subDirPATH = dir + subDir.ToString();
                    xmlInfo.Add(CreateSubdirectoryXML(subDir, subDirPATH));
                    fileNumber++;

                }
                XAttribute attribute1 = new XAttribute("number_of_files", fileNumber);
                xmlInfo.Add(attribute1);

                XAttribute attribute2 = new XAttribute("size", size);
                xmlInfo.Add(attribute2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: \n" + ex);
            }
            return xmlInfo;
        }
        private static XElement CreateSubdirectoryXML(DirectoryInfo dir, string FULL_DIR_PATH)    //Method for getting all subdirectories
        {
            Int64 sumDirSize = 0;
            int fileNumber = 0;
            Int64 size = 0;

            //get directories
            var xmlInfo = new XElement("Folder", new XAttribute("name", dir.Name));

            //sumarizing size of files in subfolder
            DirectoryInfo dirX = new DirectoryInfo(FULL_DIR_PATH);
            FileSystemInfo[] filelist = dirX.GetFileSystemInfos();
            FileInfo[] fileInfo;
            fileInfo = dir.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfo.Length; i++)
            {
                try
                {
                    size += fileInfo[i].Length;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: \n" + ex);
                }
            }
            try
            {
                //get all the files first
                foreach (var file in dir.GetFiles())
                {
                    //file size
                    Int64 fileSizeInBytes = new FileInfo(FULL_DIR_PATH + "\\" + file.ToString()).Length;

                    //creation and modification time
                    DateTime creation = File.GetCreationTime(FULL_DIR_PATH + "\\" + file.ToString());
                    DateTime modification = File.GetLastWriteTime(FULL_DIR_PATH + "\\" + file.ToString());
                    DateTime access = file.LastAccessTime;

                    //new file
                    xmlInfo.Add(new XElement("File", new XAttribute("name", file.Name), new XAttribute("size", fileSizeInBytes + "b"), new XAttribute("creation_time", creation.ToString()), new XAttribute("last_access_time", access.ToString()), new XAttribute("modified_time", modification.ToString())));

                    sumDirSize += fileSizeInBytes;
                    fileNumber++;
                }
                //get subdirectories
                var subdirectories = dir.GetDirectories().ToList().OrderBy(d => d.Name);
                foreach (var subDir in subdirectories)
                {
                    string subDirPATH_x = FULL_DIR_PATH + "\\" + subDir.ToString();
                    xmlInfo.Add(CreateSubdirectoryXML(subDir, subDirPATH_x));
                    fileNumber++;
                }

                XAttribute attribute1 = new XAttribute("number_of_files", fileNumber);
                xmlInfo.Add(attribute1);

                XAttribute attribute2 = new XAttribute("size", size);
                xmlInfo.Add(attribute2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: \n" + ex);
            }
            return xmlInfo;
        }
        private void button2_Click(object sender, EventArgs e)  //Button for creating XML file (after URL of directory is selected)
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo("" + textBox1.Text.ToString() + "");
                var XMLs = new XDocument(CreateXML(d));
                XMLs.Save("THE_XML");
                button3.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: \n" + ex);
            }
        }
        private static TreeNode CreateSubDirectoryNode(DirectoryInfo directoryinfo, string FULL_DIR_PATH)
        {
            Int64 sumDirSize = 0;
            int fileNumber = 0;
            
            TreeNode directoryNode = new TreeNode(directoryinfo.Name);
            
            Int64 size = 0;
            
            //sumarizing size of files in subfolder
            FileSystemInfo[] filelist = directoryinfo.GetFileSystemInfos();
            FileInfo[] fileInfo;
            fileInfo = directoryinfo.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfo.Length; i++)
            {
                try
                {
                    size += fileInfo[i].Length;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: \n" + ex);
                }
            }
            foreach (var file in directoryinfo.GetFiles())
                {
                    DateTime creation = File.GetCreationTime(FULL_DIR_PATH + "\\" + file.ToString());
                    DateTime modification = File.GetLastWriteTime(FULL_DIR_PATH + "\\" + file.ToString());
                    DateTime access = file.LastAccessTime;

                    TreeNode x = new TreeNode(file.Name);
                    x.Tag = "File name=" + file.Name + " size=" + new FileInfo(FULL_DIR_PATH + "\\" + file.ToString()).Length + " creation_time=" + creation + " last_access_time=" + access + " modified_time=" + modification + "";

                    directoryNode.Nodes.Add(x);

                    fileNumber++;
                    sumDirSize += new FileInfo(FULL_DIR_PATH + "\\" + file.ToString()).Length;

                }
            var subdirectories = directoryinfo.GetDirectories().ToList().OrderBy(d => d.Name);
            foreach (var subDir in subdirectories)
            {
                directoryNode.Nodes.Add(CreateSubDirectoryNode(subDir, subDir.FullName.ToString()));
                fileNumber++;
            }
            
            directoryNode.Tag = "Folder name=" + directoryinfo.Name + " number_of_files=" + fileNumber + " size=" + size + " .";

            return directoryNode;
        }
        //showing hierarchy without info
        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryinfo)
        {
            Int64 sumDirSize = 0;
            int fileNumber = 0;
            Int64 size = 0;
            
            //sumarizing size of files in subfolder
            FileSystemInfo[] filelist = directoryinfo.GetFileSystemInfos();
            FileInfo[] fileInfo;
            fileInfo = directoryinfo.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfo.Length; i++)
            {
                try
                {
                    size += fileInfo[i].Length;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: \n" + ex);
                }
            }
            var directoryNode = new TreeNode(directoryinfo.Name);
            foreach (var file in directoryinfo.GetFiles())
            {
                DateTime creation = File.GetCreationTime(directoryinfo+ "\\" + file.ToString());
                DateTime modification = File.GetLastWriteTime(directoryinfo + "\\"  + file.ToString());
                DateTime access = file.LastAccessTime;
                
                TreeNode x = new TreeNode(file.Name);
                x.Tag = "File name=" + file.Name + " size=" + new FileInfo(directoryinfo.FullName + "\\" + file.ToString()).Length + " creation_time=" + creation + " last_access_time=" + access+ " modified_time=" + modification+"";
                
                directoryNode.Nodes.Add(x);
                
                fileNumber++;
                sumDirSize += new FileInfo(directoryinfo.FullName +"\\" + file.ToString()).Length;
            }
            //get subdirectories
            var subdirectories = directoryinfo.GetDirectories().ToList().OrderBy(d => d.Name);
            foreach (var subDir in subdirectories)
            {
                directoryNode.Nodes.Add(CreateSubDirectoryNode(subDir, subDir.FullName.ToString()));
                fileNumber++;
            }
            directoryNode.Tag = "Folder name=" + directoryinfo.Name + " number_of_files=" +fileNumber+   " size=" + size + " .";
            
            return directoryNode;
        }
        private void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }
        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)    //adding xml items to treeview one by one
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;
            string x = "";
            int s = 0;
            // Loop through the XML nodes until the end of branch is reached
            // Add the nodes to the TreeView during the looping process
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i < nodeList.Count; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                    x = inXmlNode.OuterXml;
                    s = x.IndexOf(">");
                    inTreeNode.Text = inXmlNode.OuterXml.Remove(s+1);
                }
            }
            else
            {
                //Pulling the data from the XmlNode based on the type of node
                inTreeNode.Text = inXmlNode.OuterXml;
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) //Select item to show it's info in separate TreeView
        {
            try
            {
                string str = "";

                str = treeView1.SelectedNode.Tag.ToString();

                string[] x = str.Split(' ');

                if (x[0] == "Folder")  //folder attributes
                {
                    //name
                    int pFrom1 = str.IndexOf("name=") + "name=".Length;
                    int pTo1 = str.IndexOf("number_of_files");
                    string result1 = str.Substring(pFrom1, pTo1 - pFrom1);
                    label5.Text = "" + x[0].Substring(0, x[0].Length ) + " " + "name: " + result1;

                    //number of files
                    int pFrom2 = str.IndexOf("number_of_files=") + "number_of_files=".Length;
                    int pTo2 = str.IndexOf("size");
                    string result2 = str.Substring(pFrom2, pTo2 - pFrom2);
                    label6.Text = "" + x[0].Substring(0, x[0].Length) + " " + "number of files: " + result2;

                    //size in bytes
                    int pFrom3 = str.IndexOf("size=") + "size=".Length;
                    int pTo3 = str.Length-1;
                    
                    string result3 = str.Substring(pFrom3, pTo3 - pFrom3);

                    label7.Text = "" + x[0].Substring(0, x[0].Length) + " " + "size: " + result3 + " bytes";

                    label8.Text = "";
                    label9.Text = "";
                }
                if (x[0] == "File")    //file attributes
                {
                    //name
                    int pFrom1 = str.IndexOf("name=") + "name=".Length;
                    int pTo1 = str.IndexOf("size");
                    string result1 = str.Substring(pFrom1, pTo1 - pFrom1);
                    label5.Text = "" + x[0].Substring(0, x[0].Length) + " " + "name: " + result1;

                    //size in bytes
                    int pFrom2 = str.IndexOf("size=") + "size=".Length;
                    int pTo2 = str.IndexOf("creation_time");
                    string result2 = str.Substring(pFrom2, pTo2 - pFrom2);
                    label6.Text = "" + x[0].Substring(0, x[0].Length) + " " + "size: " + result2 + " bytes";

                    //file creation time
                    int pFrom3 = str.IndexOf("creation_time=") + "creation_time=".Length;
                    int pTo3 = str.IndexOf("last_access_time");
                    string result3 = str.Substring(pFrom3, pTo3 - pFrom3);
                    label7.Text = "" + x[0].Substring(0, x[0].Length) + " " + "creation time: " + result3;

                    //file last access time
                    int pFrom4 = str.IndexOf("last_access_time=") + "last_access_time=".Length;
                    int pTo4 = str.IndexOf("modified_time");
                    string result4 = str.Substring(pFrom4, pTo4 - pFrom4);
                    label8.Text = "" + x[0].Substring(0, x[0].Length) + " " + "last access time: " + result4;

                    //file last modified time
                    int pFrom5 = str.IndexOf("modified_time=") + "modified_time=".Length;
                    int pTo5 = str.Length;
                    string result5 = str.Substring(pFrom5, pTo5 - pFrom5);
                    label9.Text = "" + x[0].Substring(0, x[0].Length) + " " + "last modified time: " + result5;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: \n" + ex);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ListDirectory(treeView1, textBox1.Text);
            treeView1.ExpandAll();
        }
    }
}
