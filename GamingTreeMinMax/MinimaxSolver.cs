using System;
using System.Collections.Generic;

namespace GamingTreeMinMax
{
    public class MinimaxSolver
    {
        public TreeElement Root { get; }
        public bool UseAlphaBeta { get; set; } = false; // Использование альфа-бета отсечений
        public bool AnalyzeRightToLeft { get; set; } = false; // Изменение порядка анализа узлов
        public int MaxDepth { get; set; } = int.MaxValue; // Лимит глубины (по умолчанию бесконечность)
        public List<(TreeElement Node, string Reason)> PrunedNodes { get; private set; } = new(); // Список отсечённых узлов с причинами

        public MinimaxSolver(TreeElement root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
        }

        // Запуск алгоритма решения минимаксного дерева
        public int Solve()
        {
            PrunedNodes.Clear(); // Очищаем список отсечённых узлов перед началом
            return UseAlphaBeta ? MinimaxAlphaBeta(Root, int.MinValue, int.MaxValue, true, 0) : MinimaxBasic(Root, true, 0);
        }

        // Базовый минимакс без альфа-бета отсечений.
        private int MinimaxBasic(TreeElement node, bool isMaximizing, int depth)
        {
            if (node.Children.Count == 0 || depth >= MaxDepth)
            {
                return node.Value ?? 0;
            }

            int bestValue = isMaximizing ? int.MinValue : int.MaxValue;
            var children = GetOrderedChildren(node);

            foreach (var child in children)
            {
                int childValue = MinimaxBasic(child, !isMaximizing, depth + 1);
                bestValue = isMaximizing ? Math.Max(bestValue, childValue) : Math.Min(bestValue, childValue);
            }

            node.Value = bestValue;
            return bestValue;
        }
        // Минимакс с альфа-бета отсечениями
        private int MinimaxAlphaBeta(TreeElement node, int alpha, int beta, bool isMaximizing, int depth)
        {
            if (node.Children.Count == 0 || depth >= MaxDepth)
            {
                return node.Value ?? 0; // Лист или достигнута максимальная глубина
            }

            int bestValue = isMaximizing ? int.MinValue : int.MaxValue;
            var children = GetOrderedChildren(node); // Упорядоченные дочерние элементы

            foreach (var child in children)
            {
                int childValue = MinimaxAlphaBeta(child, alpha, beta, !isMaximizing, depth + 1);

                if (isMaximizing)
                {
                    bestValue = Math.Max(bestValue, childValue);
                    alpha = Math.Max(alpha, bestValue);
                }
                else
                {
                    bestValue = Math.Min(bestValue, childValue);
                    beta = Math.Min(beta, bestValue);
                }

                // Условие отсечения
                if (alpha >= beta)
                {
                    child.IsPruned = true;
                    child.PruneReason = $"α={alpha}, β={beta}";
                    if (string.IsNullOrEmpty(child.PruneReason) || true)
                    {
                        // Добавляем текст только для первого отсечённого узла
                        string condition = isMaximizing
                            ? $"z <= α({alpha})"
                            : $"z >= β({beta})";
                        child.PruneReason = $"{condition}";
                        PrunedNodes.Add((child, child.PruneReason));
                    }

                    // Помечаем всех братьев и их потомков
                    MarkPrunedSubtreeAndSiblings(node, child);
                    break;
                }
            }

            node.Value = bestValue;
            return bestValue;
        }
   
        // Пометка оптимального пути
        public void MarkOptimalPath(TreeElement node)
        {
            if (node == null)
                return;

            // Если это лист, то помечаем узел, если его значение не null
            if (node.Children.Count == 0 && node.Value.HasValue)
            {
                node.IsOptimalPath = true;
                return;
            }

            // Для промежуточных узлов с дочерними элементами выбираем оптимальный путь
            TreeElement? optimalChild = null;

            foreach (var child in node.Children)
            {
                if (child.Value.HasValue)
                {
                    if (optimalChild == null || (node.IsMaxNode && child.Value > optimalChild.Value) || (!node.IsMaxNode && child.Value < optimalChild.Value))
                    {
                        optimalChild = child;
                    }
                }
            }

            // Если найден оптимальный дочерний узел, помечаем его и продолжаем идти по дереву
            if (optimalChild != null)
            {
                optimalChild.IsOptimalPath = true;
                MarkOptimalPath(optimalChild); // Рекурсивно продолжаем по оптимальному пути
            }
        }
        // Пометка отсечённых поддеревьев
        private void MarkPrunedSubtreeAndSiblings(TreeElement parent, TreeElement prunedChild)
        {
            // Получаем братьев, следующих за отсечённым узлом
            var siblings = parent.Children.Skip(parent.Children.IndexOf(prunedChild) + 1);

            foreach (var sibling in siblings)
            {
                MarkPrunedSubtree(sibling); // Рекурсивно помечаем поддерево как отсечённое
            }
        }
        // Рекурсивная пометка узлов и поддерева
        private void MarkPrunedSubtree(TreeElement node)
        {
            node.IsPruned = true;

            foreach (var child in node.Children)
            {
                MarkPrunedSubtree(child); // Помечаем всех потомков
            }
        }

        // Упорядочивает детей узла в зависимости от заданного порядка анализа
        private List<TreeElement> GetOrderedChildren(TreeElement node)
        {
            var children = new List<TreeElement>(node.Children);
            if (AnalyzeRightToLeft)
            {
                children.Reverse();
            }
            return children;
        }
    }

}
