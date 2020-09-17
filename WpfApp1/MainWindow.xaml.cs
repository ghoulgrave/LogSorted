using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        
        }


        private void selectFile_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";
            dialog.Filter = "日志文件(*.log,*.out)|*.log;*.out";

            if (dialog.ShowDialog() == true)
            {
                filePath.Text = dialog.FileName;
            }
            Color color = Colors.Blue;
            string s1 = filePath.Text;
            new System.Threading.Thread(() =>
            {
                try
                {
                        // 创建一个 StreamReader 的实例来读取文件 
                        // using 语句也能关闭 StreamReader
                        using (StreamReader sr = new StreamReader(s1, Encoding.Default))
                    {
                        UpdateText("开始读取数据:", color, true);
                        string line;
                        int outNum = 0;
                            // 从文件读取并显示行，直到文件的末尾 
                            while ((line = sr.ReadLine()) != null)
                        {
                            outNum = checkToRichText(line, outNum);
                                //System.Threading.Thread.Sleep(5);
                            }
                        UpdateText("", color, true);
                        UpdateText("结束读取数据", color, true);
                    }
                }
                catch (Exception ex)
                {
                        // 向用户显示出错消息
                        Console.WriteLine("The file could not be read:");
                    Console.WriteLine(ex.Message);
                }
            }).Start();
        }

        public void UpdateText(string msg, Color color, bool nextline)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                SolidColorBrush _brushColor = new SolidColorBrush(color);

                string _msg = nextline ? msg + "\r\n" : msg;
                var r = new Run(_msg);
                Paragraph p = new Paragraph() { Foreground = _brushColor };
                p.Inlines.Add(r);
                richText.Document.Blocks.Add(p);
                richText.Focus();
                richText.ScrollToEnd();
            }));
        }

        /**
         * 校验是否显示在前台页面中
         * 显示什么样的文字
         * outNum 继续输出多少行数据
         */
        private int checkToRichText(string line,int outNum ) {
            Color color = Colors.Gray;
            Color colorRed = Colors.Red;
            if (outNum > 0) {
                UpdateText(line , color, false);
            }
            else {
                if (line.Contains("java.lang.ClassNotFoundException"))
                {
                    UpdateText("缺少文件", colorRed, false);
                    UpdateText(line.Replace("java.lang.ClassNotFoundException:", "").Replace("Caused by: ", ""), color, false);
                    return 0;
                }
                if (line.Contains("当前实体类不包含名为"))
                {
                    UpdateText("实体缺少字段", colorRed, false);
                    UpdateText(line.Replace("cn.gtmap.bdcdj.core.exception.BdcdjException:", ""), color, false);
                    return 6;
                }
                if (line.Contains("org.springframework.web.client.ResourceAccessException: I/O error")) {
                    UpdateText("调用接口错误:", colorRed, false);
                    UpdateText(""+line, color, false);
                    return 6;
                }
                if (line.Contains("org.springframework.jdbc.BadSqlGrammarException:"))
                {
                    UpdateText("数据库错误:", colorRed, false);
                    UpdateText("" + line, color, false);
                    return 8;
                }

                if (line.Contains("java.lang.NoSuchMethodError:"))
                {
                    UpdateText("类文件需要更新: ", colorRed, false);
                    UpdateText("" + line.Substring(line.IndexOf("java.lang.NoSuchMethodError:")), color, false);
                    return 2;
                }
                if (line.Contains("java.lang.NoSuchFieldError:"))
                {
                    UpdateText("类文件需要更新: ", colorRed, false);
                    UpdateText("" + line.Substring(line.IndexOf("java.lang.NoSuchFieldError:")), color, false);
                    return 2;
                }
                if (line.Contains("org.apache.ibatis.exceptions.PersistenceException"))
                {
                    UpdateText("mybatis 无法找到对应方法: ", colorRed, false);
                    UpdateText( line , color, false);
                    return 4;
                }
                if (line.Contains("ClassFormatException"))
                {
                    UpdateText("存在不兼容问题: ", colorRed, false);
                    UpdateText(line , color, false);
                    return 4;
                }
                if (line.Contains("java.lang.NullPointerException"))
                {
                    UpdateText("空指针问题: ", colorRed, false);
                    UpdateText(line , color, false);
                    return 5;
                }
            }
            return outNum >1 ? --outNum:0;
        }





    }
}
