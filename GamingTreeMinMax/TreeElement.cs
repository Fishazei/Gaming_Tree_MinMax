using System.Collections.ObjectModel;

namespace GamingTreeMinMax
{
    public class TreeElement
    {
        public TreeElement? Parent { get; set; } // Cсылка на отцовский элемент, для упрощения работы алгоритма
        public List<TreeElement> Children { get; set; } = new List<TreeElement>();
        public int? Value { get; set; } // Nullable, так как у промежуточных узлов значение может отсутствовать
        public bool IsMaxNode { get; set; }
        public bool IsPruned { get; set; }
        public string? PruneReason { get; set; } // Причина отсечения
        public bool IsOptimalPath { get; set; } // Оптимальный путь

        public TreeElement(bool isMaxNode)
        {
            IsMaxNode = isMaxNode;
        }

        // Метод добавления дочернего элемента
        public void AddChild(TreeElement child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void SetChild(int count, ObservableCollection<TreeElement> forLeaves = null)
        {
            Children.Clear();
            for (int i = 0; i < count; i++)
            {
                Children.Add(new TreeElement(!IsMaxNode));
                Children[i].Parent = this;
                if (forLeaves != null)  // Добавление листьев в список листьев дерева
                {
                    forLeaves.Add(Children[i]);
                }
            }
        }
    }
}
