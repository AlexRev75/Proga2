using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;


namespace programer
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            
        }


        [DllImport("user32.dll")]
        public static extern int SendMessage(
           int hWnd,      // handle to destination window
           uint Msg,       // message
           int wParam,  // first message parameter
           int lParam   // second message parameter
           );


        PictureBox pic = new PictureBox();
        private int begin_x;
        private int begin_y;
        bool resize = false;
        private int scroller_vert = -1;
        private int scroller_hor = -1;



        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Form1_Load(object sender, EventArgs e)
        {
            pic.Parent = pictureBox1;
            pic.BackColor = Color.Transparent;
            pic.SizeMode = PictureBoxSizeMode.AutoSize;
            pic.BorderStyle = BorderStyle.FixedSingle;
            pic.Visible = false;
            this.ActiveControl = btnCod;
            btnCod.Focus();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void btnGraphic_Click(object sender, EventArgs e)
        {
            richTextBox1.Hide();
            pictureBox1.Show();
            btnScreen.Enabled = true;
            

        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void btnCod_Click(object sender, EventArgs e)
        {
           
            richTextBox1.Show();
            pictureBox1.Hide();
            btnScreen.Enabled = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btnScreen_Click(object sender, EventArgs e)
        {
            Form Form1 = new Form1();
            this.Hide();//Прячем Form1
            Thread.Sleep(1000);//Пауза перед скрином что бы успела свернуться форма



            ////////////////////////////////////////////////////////////////////////////////////////////////////////

            Bitmap bm = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);//Код создания скриншота
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(0, 0, 0, 0, bm.Size);
            pictureBox1.Image = bm;
            this.Show();//Показываем форму после того как сделали скрин
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        static public Image Copy(Image srcBitmap, Rectangle section)
        {
            // Вырезаем выбранный кусок картинки
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {

                g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);
            }
            ////Возвращаем кусок картинки.
            return bmp;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {

            pic.Width = 0;
            pic.Height = 0;
            pic.Visible = false;
            timer1.Stop();
            if (resize == true)
            {
                if ((e.X > begin_x +5) && (e.Y > begin_y + 5)) //Чтобы совсем уж мелочь не вырезал - и по случайным нажатиям не срабатывал! (можно убрать +10)
                {
                    Rectangle rec = new Rectangle(begin_x, begin_y, e.X - begin_x, e.Y - begin_y);
                    pictureBox1.Image = Copy(pictureBox1.Image, rec);
                }
            }
            resize = false;

            


        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                begin_x = e.X;
                begin_y = e.Y;
                pic.Left = e.X;
                pic.Top = e.Y;
                pic.Width = 0;
                pic.Height = 0;
                pic.Visible = true;
                timer1.Start();
                resize = true;
            }

            //Участок кода на нажатие Сохранить как... для pictureBox1 правой кнопкой мыши
            if (e.Button == MouseButtons.Right)//Нажатие правой кнопки мыши
            {
                if (pictureBox1.Image != null)//Если в pictureBox есть изображение
                {
                     SaveFileDialog sfd = new SaveFileDialog();//Создание диалогового окна "Сохранить как..", для сохранения изображения
                    sfd.Title = "Сохранить картинку как...";
                     sfd.OverwritePrompt = true;//Отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                     sfd.CheckPathExists = true;//Отображать ли предупреждение, если пользователь указывает несуществующий путь
                    sfd.Filter = "Файл(*.bmp)|*.bmp|Файл(*.jpg)|*.jpg|Файл(*.gif)|*.gif|Файл(*.png)|*.png|Все файлы(*.*)|*.*";//Список форматов файла, отображаемый в поле "Тип файла"
                    sfd.ShowHelp = true;//отображается ли кнопка "Справка" в диалоговом окне
                    Thread.Sleep(500);//Пауза перед скрином что бы успела свернуться форма
                    if (sfd.ShowDialog() == DialogResult.OK)//Отлавливаем нажатие "ОК" если в диалоговом окне нажата кнопка "ОК"
                    {
                         try
                         {
                             pictureBox1.Image.Save(sfd.FileName);

                         }

                         catch
                         {
                             MessageBox.Show("Не возможно сохранить картинку", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                         }


                     }

                    

                    
                }
            }
           


        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pic.Width = e.X - begin_x;
                pic.Height = e.Y - begin_y;

                //Скроллинг...
                scroller_hor = -1;
                scroller_vert = -1;

                if (e.X > panel2.Width - 5)
                { scroller_hor = 0; }

                if (e.Y > panel2.Height - 5)
                { scroller_vert = 0; }


            }
            
        }




        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (scroller_vert > -1)
            {
                SendMessage(panel2.Handle.ToInt32(), 277, 1, scroller_vert);
            }
            if (scroller_hor > -1)
            {
                SendMessage(panel2.Handle.ToInt32(), 276, 1, scroller_hor);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void newToolStripMenuItem_Click(object sender, EventArgs e)// Обновить текстовый файл richTextBox1
        {
            this.richTextBox1.Clear();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
           {
                try
                {
                    richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.RichText);// Пытаемся загрузить текст как RTF
                }
                catch (System.ArgumentException)
                {
                    richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);// Пытаемся загрузить текст как обычный текст
                }

                this.Text = openFileDialog1.FileName;// Устанавливаем в заголовок окна имя открытого файла


            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)//Сохранить
        {

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                richTextBox1.SaveFile(saveFileDialog1.FileName);
                this.Text = saveFileDialog1.FileName;// Отображаем имя сохраненного файла в заголовке окна нашего приложения
            }

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void saveasToolStripMenuItem1_Click(object sender, EventArgs e)//Сохранить как...
        {
            SaveFileDialog saveFile1 = new SaveFileDialog();
            saveFile1.DefaultExt = "*.text";
            saveFile1.Filter = "Файл|*.text|Все файлы(*.*)|*.*";
            // Определяет, выбрал ли пользователь имя файла из файла saveFileDialog.
            if (saveFile1.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
               saveFile1.FileName.Length > 0)
            {
                // Сохраняет содержимое RichTextBox в файле.
                richTextBox1.SaveFile(saveFile1.FileName, RichTextBoxStreamType.PlainText);
                this.Text = saveFileDialog1.FileName;// Отображаем имя сохраненного файла в заголовке окна нашего приложения
            }

        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)// Закрыть
        {
            this.Close();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)// Отменить
        {
            richTextBox1.Undo();
           
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)//Повторить
        {
            richTextBox1.Redo();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)//Копировать
        {
            if (richTextBox1.SelectedText != null && richTextBox1.SelectedText != "")
            {
                Clipboard.SetText(richTextBox1.SelectedText);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)//Вставить
        {
            if (Clipboard.ContainsText())
            {
                int selstart = richTextBox1.SelectionStart;
                if (richTextBox1.SelectedText != null && richTextBox1.SelectedText != "")
                {
                    richTextBox1.Text = richTextBox1.Text.Remove(selstart, richTextBox1.SelectionLength);
                }

                string clip = Clipboard.GetText(TextDataFormat.Text).ToString();
                richTextBox1.Text = richTextBox1.Text.Insert(selstart, clip);
                richTextBox1.SelectionStart = selstart + clip.Length;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)//Вырезать
        {
            richTextBox1.Cut();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)//Вызываем contextMenuStrip правой кнопкой мыши
        {
            if(e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition,ToolStripDropDownDirection.Right);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)//Копирует правой кнопкой мыши ContextMenuStrip добавленного на форму
        {
            if (richTextBox1.Text != null)
            {
                try
                {
                    Clipboard.SetText(richTextBox1.SelectedText.ToString());

                }

                catch
                {
                    MessageBox.Show("Не возможно сохранить картинку", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)//Вставляет правой кнопкой мыши ContextMenuStrip добавленного на форму
        {
            
            if (Clipboard.ContainsText())
            {
                int selstart = richTextBox1.SelectionStart;
                if (richTextBox1.SelectedText != null && richTextBox1.SelectedText != "")
                {
                    richTextBox1.Text = richTextBox1.Text.Remove(selstart, richTextBox1.SelectionLength);
                }

                string clip = Clipboard.GetText(TextDataFormat.Text).ToString();
                richTextBox1.Text = richTextBox1.Text.Insert(selstart, clip);
                richTextBox1.SelectionStart = selstart + clip.Length;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)//Вырезает правой кнопкой мыши ContextMenuStrip добавленного на форму
        {
            if(richTextBox1.SelectionLength > 0)
            {
                richTextBox1.Cut();
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)//Выделить все правой кнопкой мыши ContextMenuStrip добавленного на форму
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.SelectAll();
            }
        }
    }
}   
// ты понял ?
//не совсем
