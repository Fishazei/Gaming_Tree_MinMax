using System.Diagnostics;
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

namespace GamingTreeMinMax
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Tree _tree;
        private TreeRenderer _renderer;

        public MainWindow()
        {
            InitializeComponent();
            _renderer = new TreeRenderer(TreeCanvas); // инициализация рендерера для отрисовки
        }

        private void GenerateTreeButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем глубину и степень ветвления из полей ввода
            if (int.TryParse(DepthInput.Text, out int depth) && 
                int.TryParse(BranchingInputMin.Text, out int branchingMin) && 
                int.TryParse(BranchingInputMax.Text, out int branchingMax))
            {
                if (branchingMax >= branchingMin)
                // Создаем дерево с заданными параметрами (начинаем с корня MAX)
                _tree = new Tree(depth, branchingMin, branchingMax, isMaxRoot: true);
                // Отрисовываем дерево
                _renderer.RenderTree(_tree);
                PopulateLeafSelector();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые значения для глубины и ветвления.");
            }
        }
        private void LoadTreeButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ToggleRootPlayer_Click(object sender, RoutedEventArgs e)
        {
            _tree.ToggleRootPlayer();
            Debug.Write("- Changing roles end\n");
            _renderer.RenderTree(_tree); // Перерисовать дерево
        }

        //Изменение данных в листьях
        private void PopulateLeafSelector()
        {
            LeafSelector.ItemsSource = _tree.Leaves;
            LeafSelector.SelectedIndex = 0; // Выбираем первый лист
        }
        private void UpdateLeafValue_Click(object sender, RoutedEventArgs e)
        {
            if (LeafSelector.SelectedItem is TreeElement selectedLeaf && int.TryParse(LeafValueInput.Text, out int newValue))
            {
                _tree.UpdateLeafValue(selectedLeaf, newValue);
                _renderer.RenderTree(_tree); // Перерисовать дерево с обновленным значением
                //PopulateLeafSelector();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите лист и введите корректное значение.", "Ошибка");
            }
        }

        private void RunMinimax_Click(object sender, RoutedEventArgs e)
        {
            if (_tree == null) return;

            var solver = new MinimaxSolver(_tree.Root, true, 3);
            solver.Start();

            string info;
            while (solver.Step(out info))
            {
                Debug.WriteLine(info);
                _renderer.RenderTree(_tree);

                //Thread.Sleep(100);
            }
        }
    }
}