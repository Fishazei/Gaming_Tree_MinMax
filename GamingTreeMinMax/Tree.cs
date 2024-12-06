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
                node.Value = new Random().Next(-15, 15);
                Leaves.Add(node);
            }
            return node;
        }
        // Создание дерева из Варианта
        public void LoadTree()
        {
            List<int> leavesVal = new List<int>{ 0, 0, 2, 1, -2, 1, -3, 5, 4, 6, 5, 5, 4, 3, 7, 5, 4, 2, 1, 2, 1, 0, 1, 2, 0, -2, 1, 0, 2, -1, 0, 1, 2, 3, 2};
            Leaves = new ObservableCollection<TreeElement>();

            TreeElement root = new TreeElement(true);
            // Корень-1
            root.SetChild(3);
            // Глубина 1-2
            root.Children[0].SetChild(2);
            root.Children[1].SetChild(3);
            root.Children[2].SetChild(2);
            // Глубина 2-3
            root.Children[0].Children[0].SetChild(3);
            root.Children[0].Children[1].SetChild(3);
            root.Children[1].Children[0].SetChild(2);
            root.Children[1].Children[1].SetChild(2);
            root.Children[1].Children[2].SetChild(2);
            root.Children[2].Children[0].SetChild(2);
            root.Children[2].Children[1].SetChild(1);
            // Глубина 3-листья
            root.Children[0].Children[0].Children[0].SetChild(2, Leaves);
            root.Children[0].Children[0].Children[1].SetChild(2, Leaves);
            root.Children[0].Children[0].Children[2].SetChild(3, Leaves);
            root.Children[0].Children[1].Children[0].SetChild(2, Leaves);
            root.Children[0].Children[1].Children[1].SetChild(3, Leaves);
            root.Children[0].Children[1].Children[2].SetChild(3, Leaves);
            root.Children[1].Children[0].Children[0].SetChild(2, Leaves);
            root.Children[1].Children[0].Children[1].SetChild(2, Leaves);
            root.Children[1].Children[1].Children[0].SetChild(2, Leaves);
            root.Children[1].Children[1].Children[1].SetChild(3, Leaves);
            root.Children[1].Children[2].Children[0].SetChild(2, Leaves);
            root.Children[1].Children[2].Children[1].SetChild(2, Leaves);
            root.Children[2].Children[0].Children[0].SetChild(2, Leaves);
            root.Children[2].Children[0].Children[1].SetChild(2, Leaves);
            root.Children[2].Children[1].Children[0].SetChild(3, Leaves);
            
            for (int i = 0; i < leavesVal.Count(); i++)
                Leaves[i].Value = leavesVal[i];
            Root.Children.Clear();
            Root = root;
        }
        // Изменение порядка мин/макс
        internal void ToggleRootPlayer()
        {
            if (Root == null) return;
            UpdatePlayerRoles(Root, !Root.IsMaxNode);
            Debug.Write("Changing roles start -\n");
            Debug.Write($"    Is root max now? - {Root.IsMaxNode}\n") ;
        }
        // Изменение ролей рекурсивное
        private void UpdatePlayerRoles(TreeElement node, bool isMaxNode)
        {
            node.IsMaxNode = isMaxNode;
            foreach (var child in node.Children)
                UpdatePlayerRoles(child, !isMaxNode);
        }
        // Обновление данных в листе
        public void UpdateLeafValue(TreeElement leaf, int newValue)
        {
            if (Leaves.Contains(leaf))
                leaf.Value = newValue;
        }
        // Копирование дерева
        public Tree DeepCopy()
        {
            var copiedRoot = Root.DeepCopy();
            var copiedTree = new Tree(Depth, BranchingMin, BranchingMax, Root.IsMaxNode)
            {
                Root = copiedRoot,
                Leaves = new ObservableCollection<TreeElement>()
            };

            // Копирование листьев
            foreach (var leaf in Leaves)
            {
                // Соответствующий узел в скопированном дереве
                var copiedLeaf = FindCopiedLeaf(copiedRoot, leaf);
                if (copiedLeaf != null)
                    copiedTree.Leaves.Add(copiedLeaf);
            }

            return copiedTree;
        }
        // Вспомогательный метод для поиска копии узла в новом дереве
        private TreeElement? FindCopiedLeaf(TreeElement copiedNode, TreeElement originalLeaf)
        {
            if (copiedNode == null)
                return null;
            if (originalLeaf == copiedNode)
                return copiedNode;

            foreach (var child in copiedNode.Children)
            {
                var result = FindCopiedLeaf(child, originalLeaf);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
