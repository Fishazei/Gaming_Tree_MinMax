namespace GamingTreeMinMax
{
    public class TreeElement
    {
        public TreeElement? Parent { get; set; } // Cсылка на отцовский элемент, для упрощения работы алгоритма
        public List<TreeElement> Children { get; set; } = new List<TreeElement>();
        public int? Value { get; set; } // Nullable, так как у промежуточных узлов значение может отсутствовать
        public bool IsMaxNode { get; set; }
        public bool IsPruned { get; set; }

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

        // Метод для вычисления оценки узла на основе минимаксного алгоритма
        /*
        public int CalculateMinimax(int alpha, int beta)
        {
            // Если это лист, возвращаем его значение
            if (Children.Count == 0 && Value.HasValue)
            {
                return Value.Value;
            }

            int bestValue;
            if (IsMaxNode)
            {
                bestValue = int.MinValue;
                foreach (var child in Children)
                {
                    int childValue = child.CalculateMinimax(alpha, beta);
                    bestValue = System.Math.Max(bestValue, childValue);
                    alpha = System.Math.Max(alpha, bestValue);
                    if (beta <= alpha)
                    {
                        child.IsPruned = true; // Отмечаем узел как отсеченный
                        break;
                    }
                }
            }
            else
            {
                bestValue = int.MaxValue;
                foreach (var child in Children)
                {
                    int childValue = child.CalculateMinimax(alpha, beta);
                    bestValue = System.Math.Min(bestValue, childValue);
                    beta = System.Math.Min(beta, bestValue);
                    if (beta <= alpha)
                    {
                        child.IsPruned = true;
                        break;
                    }
                }
            }
            return bestValue;
        }
        */
    }
}
