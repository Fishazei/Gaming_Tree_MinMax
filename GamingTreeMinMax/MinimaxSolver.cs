using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GamingTreeMinMax
{
    public class MinimaxSolver
    {
        public TreeElement Root { get; }
        public bool UseAlphaBeta { get; set; } // Флаг использования альфа-бета отсечений
        public int MaxDepth { get; set; } // Максимальная глубина поиска

        private int _currentDepth; // Текущая глубина анализа
        private TreeElement _currentNode; // Текущий узел
        private Stack<(TreeElement Node, int Alpha, int Beta)> _stack = new(); // Стек для пошагового выполнения

        public MinimaxSolver(TreeElement root, bool useAlphaBeta = false, int maxDepth = int.MaxValue)
        {
            Root = root;
            UseAlphaBeta = useAlphaBeta;
            MaxDepth = maxDepth;
        }

        public void Start()
        {
            _currentDepth = 0;
            _currentNode = Root;
            _stack.Clear();
            _stack.Push((Root, int.MinValue, int.MaxValue));
        }

        public bool Step(out string debugInfo)
        {
            if (_stack.Count == 0)
            {
                debugInfo = "Анализ завершён.";
                return false; // Алгоритм завершён
            }

            var (node, alpha, beta) = _stack.Pop();
            debugInfo = "";

            if (node.Children.Count == 0 || _currentDepth >= MaxDepth) // Лист или лимит глубины
            {
                node.Value = node.Value ?? 0; // Если значения нет, считаем 0
                debugInfo = $"Лист {node.Value}, глубина {_currentDepth}.";
                return true;
            }

            int bestValue = node.IsMaxNode ? int.MinValue : int.MaxValue;

            foreach (var child in node.Children)
            {
                _stack.Push((child, alpha, beta)); // Добавляем в стек для дальнейшего анализа

                int childValue = child.Value ?? 0;
                if (node.IsMaxNode)
                {
                    bestValue = Math.Max(bestValue, childValue);
                    alpha = Math.Max(alpha, bestValue);
                }
                else
                {
                    bestValue = Math.Min(bestValue, childValue);
                    beta = Math.Min(beta, bestValue);
                }

                debugInfo += $"Узел {node.Value}, обновление: {bestValue}, Alpha={alpha}, Beta={beta}\n";

                if (UseAlphaBeta && alpha >= beta) // Условие отсечения
                {
                    node.IsPruned = true;
                    debugInfo += $"Отсечение в узле {node.Value}.\n";
                    break;
                }
            }
            Debug.WriteLine(debugInfo);
            node.Value = bestValue;
            return true;
        }
    }

}
