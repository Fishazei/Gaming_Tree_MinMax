﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace GamingTreeMinMax
{
    public class TreeRenderer
    {
        private Canvas _canvas;

        public TreeRenderer(Canvas canvas)
        {
            _canvas = canvas;
        }

        private Dictionary<int, List<TreeElement>> CalculateNodesByLevel(TreeElement root)
        {
            var levels = new Dictionary<int, List<TreeElement>>();
            TraverseTree(root, 0, levels);
            return levels;
        }

        private void TraverseTree(TreeElement node, int level, Dictionary<int, List<TreeElement>> levels)
        {
            if (!levels.ContainsKey(level))
            {
                levels[level] = new List<TreeElement>();
            }
            levels[level].Add(node);

            foreach (var child in node.Children)
            {
                TraverseTree(child, level + 1, levels);
            }
        }

        public void RenderTree(Tree tree)
        {
            _canvas.Children.Clear();
            Debug.Write("Render start -");
            var levels = CalculateNodesByLevel(tree.Root);
            var nodePositions = new Dictionary<TreeElement, (double X, double Y)>();

            double verticalSpacing = 75;
            double canvasWidth = _canvas.ActualWidth;
            foreach (var level in levels)
            {
                int numNodes = level.Value.Count;
                double horizontalSpacing = canvasWidth / (numNodes + 1); // Расстояние между узлами на уровне
                for (int i = 0; i < numNodes; i++)
                {
                    var node = level.Value[i];
                    double x = (i + 1) * horizontalSpacing; // Расположение узла на уровне
                    double y = level.Key * verticalSpacing + 30;
                    nodePositions[node] = (x, y);
                    DrawNode(node, x, y, horizontalSpacing / 2);
                }
            }

            foreach (var level in levels)
            {
                foreach (var node in level.Value)
                {
                    // Получаем позицию текущего узла
                    if (nodePositions.TryGetValue(node, out var parentPos))
                    {
                        foreach (var child in node.Children)
                        {
                            if (nodePositions.TryGetValue(child, out var childPos))
                            {
                                // Добавляем линию между узлом и его дочерним элементом
                                DrawLine(parentPos.X, parentPos.Y, childPos.X, childPos.Y);
                            }
                        }
                    }
                }
            }
            Debug.Write("- Render end\n");
        }

        private void DrawNode(TreeElement node, double x, double y, double xOffset){
            // Рисуем круг для узла
            var ellipse = new Ellipse{
                Width = 30, Height = 30,
                Fill = node.IsMaxNode ? Brushes.LightBlue : Brushes.LightCoral,
                Stroke = node.IsPruned ? Brushes.Gray : Brushes.Black,
                StrokeThickness = node.IsPruned ? 1 : 2
            };
            Canvas.SetLeft(ellipse, x - ellipse.Width / 2);
            Canvas.SetTop(ellipse, y - ellipse.Height / 2);
            _canvas.Children.Add(ellipse);
            // Текстовая метка с оценкой узла
            var text = new TextBlock
            {
                Text = node.Value.HasValue ? node.Value.Value.ToString() : "?",
                Foreground = Brushes.Black, FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Canvas.SetLeft(text, x - 10);
            Canvas.SetTop(text, y - 10);
            _canvas.Children.Add(text);
        }

        private void DrawLine(double x1, double y1, double x2, double y2){
            var line = new Line{
                X1 = x1, Y1 = y1 + 15, 
                X2 = x2, Y2 = y2 - 15,
                Stroke = Brushes.Black
            };
            _canvas.Children.Add(line);
        }
    }
}