using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        // Объявление переменных для хранения информации о вопросах, ответах и количестве верных/неверных ответов
        int quection_count;
        int correct_answers;
        int wrong_answers;
        string[] array; // Массив для хранения неверных ответов

        int correct_answers_number; // Номер правильного ответа для текущего вопроса
        int selected_response; // Номер выбранного пользователем ответа

        System.IO.StreamReader Read; // Используется для чтения из файла

        public Form1()
        {
            InitializeComponent();
        }

        // Метод инициализации тестирования
        void start()
        {
            // Установка кодировки для чтения текстового файла
            var Encoding = System.Text.Encoding.GetEncoding(65001);
            try
            {
                // Чтение файла с вопросами и ответами
                Read = new System.IO.StreamReader(
                System.IO.Directory.GetCurrentDirectory() + @"\test.txt", Encoding);
                this.Text = Read.ReadLine(); // Установка заголовка окна программы

                quection_count = 0; // Обнуление счетчиков
                correct_answers = 0;
                wrong_answers = 0;

                array = new String[10]; // Инициализация массива для хранения неверных ответов
            }
            catch (Exception)
            {
                MessageBox.Show("ошибка 1"); // Вывод сообщения об ошибке, если чтение файла не удалось
            }
            question(); // Переход к первому вопросу
        }

        // Метод отображения вопроса и ответов
        void question()
        {
            label1.Text = Read.ReadLine();

            radioButton1.Text = Read.ReadLine();
            radioButton2.Text = Read.ReadLine();
            radioButton3.Text = Read.ReadLine();
            radioButton4.Text = Read.ReadLine();

            correct_answers_number = int.Parse(Read.ReadLine()); // Загрузка номера правильного ответа

            // Сброс выбранных пользователем ответов
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;

            button1.Enabled = false; // Отключение кнопки "Ответить"
            quection_count = quection_count + 1; // Увеличение счетчика вопросов

            if (Read.EndOfStream == true) button1.Text = "Завершить"; // Изменение текста кнопки, если вопросы закончились
        }

        // Обработчик выбора ответа пользователем
        void OnOff(object sender, EventArgs e)
        {
            button1.Enabled = true; // Включение кнопки "Ответить"
            button1.Focus(); // Фокусировка на кнопке
            RadioButton Переключатель = (RadioButton)sender; // Определение выбранного пользователем ответа
            var tmp = Переключатель.Name;
            selected_response = int.Parse(tmp.Substring(11)); // Получение номера выбранного ответа
        }

        // Обработчик события нажатия на текст вопроса (можно оставить пустым)
        private void label1_Click(object sender, EventArgs e)
        {
            // Ничего не делаем
        }

        // Обработчики события изменения состояния радиокнопок (можно оставить пустыми)
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { }
        private void radioButton4_CheckedChanged(object sender, EventArgs e) { }

        // Логика при нажатии на кнопку "Ответить"
        private void button1_Click(object sender, EventArgs e)
        {
            // Логика обработки правильных и неправильных ответов
            if (selected_response == correct_answers_number) correct_answers = correct_answers + 1;
            if (selected_response != correct_answers_number)
            {
                wrong_answers = wrong_answers + 1;
                array[wrong_answers] = label1.Text; // Сохранение текста вопроса с неверным ответом
            }

            // Логика завершения или продолжения теста в зависимости от состояния кнопки
            if (button1.Text == "Начать тестирование сначала")
            {
                button1.Text = "Следующий вопрос";
                radioButton1.Visible = true;
                radioButton2.Visible = true;
                radioButton3.Visible = true;
                radioButton4.Visible = true;
                start(); // Начать тестирование заново
                return;
            }

            if (button1.Text == "Завершить")
            {
                // Логика окончания тестирования и вывода результатов
                Read.Close();
                radioButton1.Visible = false;
                radioButton2.Visible = false;
                radioButton3.Visible = false;
                radioButton4.Visible = false;

                float percentage = (correct_answers * 100.0f) / quection_count;

                label1.Text = String.Format("Количество вопросов: {0}\n" +
                    "Правильных ответов: {1}\n" +
                    "Процент правильных ответов: {2:F2}%", quection_count,
                    correct_answers, percentage);

                button1.Text = "Начать тестирование сначала";

                var Str = "Список ошибок " +
                          ":\n\n";
                for (int i = 1; i <= wrong_answers; i++)
                    Str = Str + array[i] + "\n";

                if (wrong_answers != 0)
                    MessageBox.Show(Str, "Тестирование завершено");
            }

            if (button1.Text == "Следующий вопрос") question(); // Переход к следующему вопросу
        }

        // Обработчик нажатия на кнопку "Прекратить тестирование"
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрытие окна
        }

        // Метод инициализации формы
        private void Form1_Load(object sender, EventArgs e)
        {
            // Настройка текста кнопок
            button1.Text = "Следующий вопрос";
            button2.Text = "Прекратить тестирование";

            // Привязка обработчиков событий изменения радиокнопок к методу OnOff
            radioButton1.CheckedChanged += new EventHandler(OnOff);
            radioButton2.CheckedChanged += new EventHandler(OnOff);
            radioButton3.CheckedChanged += new EventHandler(OnOff);
            radioButton4.CheckedChanged += new EventHandler(OnOff);
            start(); // Инициализация начального состояния тестирования
        }
    }
}
