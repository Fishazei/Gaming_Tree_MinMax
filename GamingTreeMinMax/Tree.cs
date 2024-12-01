using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GamingTreeMinMax
{
    public class Tree
    {
        public TreeElement Root { get; set; }
        public ObservableCollection<TreeElement> Leaves { get; set; } = new ObservableCollection<TreeElement>(); // Список листьев, чтобы иметь удобный способ изменять значения для настройки
        // Параметры случайной генерации дерева
        public int Depth { get; private set; }
        public int BranchingMax { get; private set; }
        public int BranchingMin { get; private set; }

        public Tree(int depth, int branMin, int branMax, bool isMaxRoot)
        {
            Depth = depth;
            BranchingMax = branMax;
            BranchingMin = branMin;
            Root = GenerateTree(depth, isMaxRoot);
        }

        // Метод для генерации дерева случайной структуры с указанной глубиной и ветвлением
        private TreeElement GenerateTree(int depth, bool isMaxNode)
        {
            var node = new TreeElement(isMaxNode);

            if (depth > 0)
            {
                var rand = new Random();
                int numChildren = depth == Depth? BranchingMax : rand.Next(BranchingMin, BranchingMax+1);
                for (int i = 0; i < numChildren; i++)
                {
                    node.AddChild(GenerateTree(depth - 1, !isMaxNode));
                }
            }
            else
            {
                // Случайная оценка листа
                node.Value = new Random().Next(-100, 100);
                Leaves.Add(node);
            }
            return node;
        }
        
        // Создание дерева из Варианта
        private TreeElement LoadTree()
        {

            return null;
        }

        // Изменение порядка мин/макс
        internal void ToggleRootPlayer()
        {
            if (Root == null) return;
            //Root.IsMaxNode = !Root.IsMaxNode;
            UpdatePlayerRoles(Root, !Root.IsMaxNode);
            Debug.Write("Changing roles start -\n");
            Debug.Write($"    Is root max now? - {Root.IsMaxNode}\n") ;
        }
        private void UpdatePlayerRoles(TreeElement node, bool isMaxNode)
        {
            node.IsMaxNode = isMaxNode;
            foreach (var child in node.Children)
            {
                UpdatePlayerRoles(child, !isMaxNode);
            }
        }

        // Обновление данных в листе
        public void UpdateLeafValue(TreeElement leaf, int newValue)
        {
            if (Leaves.Contains(leaf))
            {
                leaf.Value = newValue;
            }
        }
    }
}
