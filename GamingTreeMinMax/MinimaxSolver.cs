using System;
using System.Collections.Generic;
using System.Diagnostics;

// Находит всё равно слева направо
//
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
        #region Solving
        // Запуск алгоритма решения минимаксного дерева
        public int Solve()
        {
            PrunedNodes.Clear(); // Очищаем список отсечённых узлов перед началом
            return UseAlphaBeta ? MinimaxAlphaBeta(Root, int.MinValue, int.MaxValue, Root.IsMaxNode, 0) : MinimaxBasic(Root, Root.IsMaxNode, 0);
        }

        // Базовый минимакс без альфа-бета отсечений.
        private int MinimaxBasic(TreeElement node, bool isMaximizing, int depth)
        {
            // Если лист
            if (node.Children.Count == 0 || depth >= MaxDepth)
            {
                return node.Value ?? 0;
            }

            int bestValue = isMaximizing ? int.MinValue : int.MaxValue; // Задаются опорные значения для сравнения
            var children = GetOrderedChildren(node); //Разворот детей по необходимости

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
                node.Value = node.Value ?? 0;
                node.PruneReason = $"α={alpha}, β={beta}";
                return node.Value.Value;
            }

            int bestValue = isMaximizing ? int.MinValue : int.MaxValue;
            var children = GetOrderedChildren(node);

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

                // Проверяем условие отсечения
                if (beta <= alpha)
                {
                    // Помечаем только узлы, которые следуют за текущим узлом
                    MarkPrunedSubtreeAndSiblings(node, child);

                    // Добавляем текст причины отсечения к первому ребёнку
                    if (string.IsNullOrEmpty(child.PruneReason))
                    {
                        string condition = isMaximizing
                            ? $"z <= α({alpha})"
                            : $"z >= β({beta})";
                        child.PruneReason = $"{condition}";
                        PrunedNodes.Add((child, child.PruneReason));
                    }

                    break; // Останавливаем обработку остальных узлов
                }
            }

            node.Value = bestValue;
            return bestValue;
        }

        #endregion
        #region Marking
        // Пометка оптимального пути
        public void MarkOptimalPath(TreeElement node)
        {
            if (node == null)
                return;

            // Если это лист, помечаем узел, если у него есть значение
            if (node.Children.Count == 0 && node.Value.HasValue)
            {
                node.IsOptimalPath = true;
                return;
            }

            // Ищем оптимального ребёнка для текущего узла
            TreeElement? optimalChild = null;
            var childrens = GetOrderedChildren(node);
            foreach (var child in childrens)
            {
                if (child.Value.HasValue)
                {
                    if (optimalChild == null ||
                        (node.IsMaxNode && child.Value > optimalChild.Value) ||
                        (!node.IsMaxNode && child.Value < optimalChild.Value))
                    {
                        optimalChild = child;
                    }
                }
            }

            // Если найден оптимальный ребёнок, помечаем его и продолжаем путь
            if (optimalChild != null)
            {
                optimalChild.IsOptimalPath = true;
                MarkOptimalPath(optimalChild); // Рекурсивно продолжаем по пути
            }
        }

        // Пометка отсечённых поддеревьев
        private void MarkPrunedSubtreeAndSiblings(TreeElement parent, TreeElement prunedChild)
        {
            // Получаем детей в порядке обхода
            var orderedChildren = GetOrderedChildren(parent);
            int prunedIndex = orderedChildren.IndexOf(prunedChild);

            // Определяем братьев, которые должны быть помечены как отсечённые
            IEnumerable<TreeElement> siblings;
            if (AnalyzeRightToLeft)
                siblings = orderedChildren.Take(prunedIndex); // Все перед prunedChild
            else
                siblings = orderedChildren.Skip(prunedIndex + 1); // Все после prunedChild

            // Помечаем только братьев и их поддеревья
            foreach (var sibling in siblings)
                MarkPrunedSubtree(sibling);
        }

        // Рекурсивная пометка узлов и поддерева
        private void MarkPrunedSubtree(TreeElement node)
        {
            node.IsPruned = true;

            foreach (var child in node.Children)
                MarkPrunedSubtree(child); // Помечаем всех потомков
        }
        #endregion
        // Упорядочивает детей узла в зависимости от заданного порядка анализа
        private List<TreeElement> GetOrderedChildren(TreeElement node)
        {
            var children = new List<TreeElement>(node.Children);
            if (AnalyzeRightToLeft)
            {
                children.Reverse();
                Debug.WriteLine("Reversing childrens");
            }
            return children;
        }
    }
}
