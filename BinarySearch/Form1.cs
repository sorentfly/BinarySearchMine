using System;
using System.Linq;
using System.Windows.Forms;

namespace BinarySearch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Метод для перестановки элементов местами.
        /// </summary>
        /// <param name="a">Первый элемент</param>
        /// <param name="b">Второй элемент</param>
        public static void Swap(ref int a, ref int b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Метод сортировки массива методом Шейкера.
        /// </summary>
        /// <param name="mass">Массив</param>
        public static void ShakeSort(int[] mass)
        {
            //temp должен быть обязательно равен чему-либо из-за того, что 
            //исходя из кода программы temp может использоваться, но не иметь значения.
            var temp = 0;

            //Левая граница массива
            var left = 0;
            
            //Правая граница массива
            var right = mass.Length - 1;

            while (left < right)
            {
                for (var i = left; i < right; i++)
                {
                    if (mass[i] < mass[i + 1]) continue;
                    Swap(ref mass[i], ref mass[i + 1]);
                    //Запоминаем позицию элемента
                    temp = i;
                }
                //Сокращаем границы массива
                right = temp;

                //Если левая часть станет больше или равна правой, тогда выходим из цикла.
                if (left >= right) break;

                //Почти идентичное повторение первого цикла.
                for (var i = right; i > left; i--)
                {
                    if (mass[i - 1] < mass[i]) continue;
                    Swap(ref mass[i], ref mass[i - 1]);
                    temp = i;
                }
                left = temp;
            }
        }

        /// <summary>
        /// Метод для заполнения массива случайными элементами в выбранном диапазоне.
        /// </summary>
        /// <param name="n">Размерность массива</param>
        /// <param name="mass">Сам массив</param>
        /// <param name="left">Левая граница массива</param>
        /// <param name="right">Правая граница массива</param>
        public static void CreateMass(int n, out int[] mass, int left, int right)
        {
            var rnd = new Random();
            var massLocal = new int[n];
            //Просто поэлементно заполняем массив.
            for (var i = 0; i < massLocal.Length; i++)
            {
                massLocal[i] = rnd.Next(left, right + 1);
            }
            mass = massLocal;
        }

        /// <summary>
        /// Метод бинарного поиска элемента в массиве.
        /// </summary>
        /// <param name="mass">Массив</param>
        /// <param name="descendingOrder">Переменная, позволяющая выбрать левую или правую часть массива для поиска</param>
        /// <param name="key">Искомое число</param>
        /// <param name="left">Левая граница массива</param>
        /// <param name="right">Правая граница массива</param>
        /// <returns>Порядковый номер элемента</returns>
        public static int BinarySearch_Rec(int[] mass, bool descendingOrder, int key, int left, int right)
        {
            //Запоминаем центр массива
            var mid = left + (right - left) / 2;

            //Если мы сократили границы до невозможного
            if (left >= right)
            {
                return -(1 + left);
            }

            //Если левый элемент - искомое число
            if (mass[left] == key)
                return left;

            //Если центральный элемент - искомое число, то мы начинаем рекурсировать.
            if (mass[mid] == key)
                return mid == left + 1 ? mid : BinarySearch_Rec(mass, descendingOrder, key, left, mid + 1);

            //Суть флага descinatingOrder в том, что он помогает выбрать нужную половину массива для поиска элемента.
            //Дело в том, что все сводится к XOR'у. Знания, приобретенные из дискретной математики помогают в этом вопросе.
            if ((mass[mid] > key) ^ descendingOrder)
                return BinarySearch_Rec(mass, descendingOrder, key, left, mid);

            return BinarySearch_Rec(mass, descendingOrder, key, mid + 1, right);
        }


        /// <summary>
        /// Обертка для рекурсивной функции. Просто так удобно.
        /// </summary>
        /// <param name="mass">Массив</param>
        /// <param name="key">Искомое число</param>
        /// <returns></returns>
        public static int BinarySearch_Rec_Wrapper(int[] mass, int key)
        {
            if (mass.Length == 0)
            {
                return -1;
            }

            var descendingOrder = mass[0] > mass[mass.Length - 1];
            return BinarySearch_Rec(mass, descendingOrder, key, 0, mass.Length);
        }

        /// <summary>
        /// Действия при нажатии на волшебную кнопку "Найти элемент".
        /// </summary>
        private void search_Click(object sender, EventArgs e)
        {
            //try/cath нужен для того, чтобы не делать кучу условий для ввода. 
            //Эта штучка позволяет не крашнуться программе, а просто вывести сообщение об ошибке и продолжить работу.
            try
            {
                //Кол-во элементов массива.
                var a = track.Value;

                //Число, которое нужно найти.
                var b = int.Parse(key.Text);

                //Границы для рандома.
                var left = int.Parse(interval_rnd_left.Text);
                var right = int.Parse(interval_rnd_right.Text);

                //Вывод ошибки при неправильных данных. В этом случае мы пойдем сразу в catch с тем текстом, который находится в скобочках
                if (left > right)
                    throw new Exception("Левая граница не может быть больше правой. Введите данные заного.");
                if ((b > right) || (b < left))
                    //Знак $ заменяет String.Format (ну почти, даже лучше). Короче говоря, мы можем внутри кавычек писать переменные в фигурных скобках
                    //и они будут красивенько так отображаться
                    throw new Exception($"Искомое число должно находиться в пределах [{left};{right}]. Попробуйте заного.");

                //Создаем массивчик пустой, а потом закидываем его в метод CreateMass
                int[] mass;
                CreateMass(a, out mass, left, right);

                //Сортируем массив
                ShakeSort(mass);

                /*
                MessageBox.Show(mass.Aggregate("", (current, t) => current + $"{t}  "), @"Отсортированный массив",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                */

                //Находим нужный элемент с помощью бинарного поиска, то есть путем деления
                //одномерного массива на 2 части до того момента, пока не будет найден элемент,
                //ну или пока мы не сократим массив до размера одного элемента, который не равен искомому.
                var result = BinarySearch_Rec_Wrapper(mass, b) + 1;
                MessageBox.Show(
                    result > 0 //bool
                        ? $"Порядковый номер найденного элемента в отсортированном массиве: {result}." //true
                        : @"Такого числа нет в созданном массиве.", //false
                    @"Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //Вывод ошибки при неправильных данных
                MessageBox.Show(ex.Message, @"AHTUNG!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Движется ползунок - меняется текст под ним.
        /// </summary>
        private void track_Scroll(object sender, EventArgs e)
        {
            label2.Text = track.Value.ToString();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var help = new AboutProgram();
            help.Show();
        }

        private void обАвтореToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new AboutMe();
            about.Show();
        }
    }
}
