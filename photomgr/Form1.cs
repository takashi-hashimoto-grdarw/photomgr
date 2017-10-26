using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace photomgr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.label1.Text = "";
            this.label2.Text = "";
            this.label3.Text = "";
            this.listBox1.Items.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult ret;

            this.folderBrowserDialog1.Description = "フォルダーを選択してください。";

            this.folderBrowserDialog1.ShowNewFolderButton = false;

            ret = this.folderBrowserDialog1.ShowDialog();

            if (ret == DialogResult.OK)
            {
                this.label1.Text = this.folderBrowserDialog1.SelectedPath;
            }
            else
            {
                this.label1.Text = "";
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult ret;

            this.folderBrowserDialog1.Description = "フォルダーを選択してください。";

            this.folderBrowserDialog1.ShowNewFolderButton = true;

            ret = this.folderBrowserDialog1.ShowDialog();

            if (ret == DialogResult.OK)
            {
                this.label2.Text = this.folderBrowserDialog1.SelectedPath;
            }
            else
            {
                this.label2.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();

            string dName1 = this.label1.Text;

            if(System.IO.Directory.Exists(dName1) == false)
            {
                MessageBox.Show("画像整理元フォルダー" + Environment.NewLine +
                    dName1 + "が見つかりません", "通知");
                return;
            }

            string dName2 = this.label2.Text;

            if(System.IO.Directory.Exists(dName2)  == false)
            {
                MessageBox.Show("画像整理先フォルダー" + Environment.NewLine + 
                    dName2 + "が見つかりません", "通知");
                return;
            }

            int i = 0;
            string strDatatime;

            foreach (string fName in System.IO.Directory.GetFiles(dName1))
            {

                strDatatime = "";
                
                if (System.IO.Path.GetExtension(fName) == ".JPG" || System.IO.Path.GetExtension(fName) == ".jpg")
                {
                    bool getFlg = false;
                    using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fName))
                    {
                        foreach (System.Drawing.Imaging.PropertyItem item in bmp.PropertyItems)
                        {
                            //Exif情報から撮影時間を取得する
                            if (item.Id == 0x9003 && item.Type == 2)
                            {
                                //文字列に変換する
                                string val = System.Text.Encoding.ASCII.GetString(item.Value);
                                val = val.Trim(new char[] { '\0' });

                                //DateTimeに変換
                                DateTime dt = DateTime.ParseExact(val, "yyyy:MM:dd HH:mm:ss", null);

                                strDatatime = dt.ToString("yyyy.MM.dd");

                                getFlg = true;

                                Console.WriteLine(fName + " 作成日：" + val + "; " + strDatatime) ;
                        
                            }
                        }
                    }
                    if (getFlg == false)
                    {
                        strDatatime = System.IO.File.GetLastWriteTime(fName).ToString("yyyy.MM.dd");
                    }
                }
                else
                {
                    strDatatime = System.IO.File.GetLastWriteTime(fName).ToString("yyyy.MM.dd");
                }

                this.listBox1.Items.Add(fName + " 作成日：" + strDatatime);

                string dName3 = this.label2.Text + @"\" + strDatatime;

                if (System.IO.Directory.Exists(dName3) == false)
                {
                    System.IO.Directory.CreateDirectory(dName3);
                }

                string sourceFName = System.IO.Path.GetFileName(fName);

                System.IO.File.Copy(fName, dName3 + @"\" + sourceFName);

                i++;

                this.Refresh();
            }

            if (i > 0)
            {
                this.label3.Text = i.ToString();
            }

        }

    }
}
