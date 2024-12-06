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

                if (beta <= alpha)
                {
                    // Помечаем только узлы, которые следуют за текущим узлом
                    MarkPrunedSubtreeAndSiblings(node, child, alpha, beta);
                    break; // Останавливаем дальнейший анализ
                }
            }

            node.Value = bestValue;
            Debug.WriteLine($"Children order: {string.Join(", ", children.Select(c => c.Value))}");
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
        private void MarkPrunedSubtreeAndSiblings(TreeElement parent, TreeElement prunedChild, int alpha, int beta)
        {
            if (string.IsNullOrEmpty(prunedChild.PruneReason))
            {
                string condition = $"α({alpha})\nβ({beta})";
                parent.PruneReason = $"{condition}";
            }
            // Упорядоченные дети
            var orderedChildren = GetOrderedChildren(parent);
            int prunedIndex = orderedChildren.IndexOf(prunedChild);

            // Определяем братьев, которые должны быть помечены как отсечённые
            IEnumerable<TreeElement> siblings;
            siblings = orderedChildren.Skip(prunedIndex + 1); // Узлы после отсечённого

            // Рекурсивно помечаем поддеревья как отсечённые
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
                Debug.WriteLine($"Reversing children of node with Value={node.Value}");
            }
            return children;
        }
    }
}
