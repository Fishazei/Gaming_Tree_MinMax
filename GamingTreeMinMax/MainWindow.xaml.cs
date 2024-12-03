using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace GamingTreeMinMax
{
    public partial class MainWindow : Window
    {
        private Tree _tree;
        private Tree _solvedTree;
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
            if (_tree == null) _tree = new Tree(0, 1, 1, true);
            _tree.LoadTree();
            _renderer.RenderTree(_tree);
            PopulateLeafSelector();
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
                PopulateLeafSelector();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите лист и введите корректное значение.", "Ошибка");
            }
        }

        // Переключение отсечений
        private bool AB = false; 
        private void AlphaBetaToggle_Checked(object sender, RoutedEventArgs e) => AB = true;
        private void AlphaBetaToggle_Unchecked(object sender, RoutedEventArgs e) => AB = false;
        private void AlphaBetaToggle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AlphaBetaToggle.IsChecked == true)
            {
                AlphaBetaToggle.IsChecked = false;
                e.Handled = true;
            }
        }

        // Переключение порядка обхода
        private bool LR = false;
        private void LeftRightToggle_Checked(object sender, RoutedEventArgs e) => LR = true;
        private void LeftRightToggle_Unchecked(object sender, RoutedEventArgs e) => LR = false;
        private void LeftRightToggle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (LeftRightToggle.IsChecked == true)
            {
                LeftRightToggle.IsChecked = false;
                e.Handled = true;
            }
        }

        private void RunMinimax_Click(object sender, RoutedEventArgs e)
        {
            if (_tree == null) return; 
            _solvedTree = _tree.DeepCopy();

            MinimaxSolver solver = new MinimaxSolver(_solvedTree.Root)
            {
                UseAlphaBeta = AB,
                AnalyzeRightToLeft = LR
            };

            // Запускаем решение
            int result = solver.Solve();
            solver.MarkOptimalPath(_solvedTree.Root);
            // Выводим результат
            Debug.WriteLine($"Результат минимаксного алгоритма: {result}");

            // Выводим причину отсечения для узлов
            foreach (var pruned in solver.PrunedNodes)
            {
                Debug.WriteLine($"Узел отсечён: {pruned.Node}, причина: {pruned.Reason}");
            }
            _renderer.RenderTree( _solvedTree);
        }

    }
}