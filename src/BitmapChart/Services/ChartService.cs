using BitmapChart.Properties;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;

namespace BitmapChart.Services
{
    public class ChartService
    {
        public int DivisionAxisX { get; set; } = 1;
        public int DivisionAxisY { get; set; } = 1;
        public Font ChartFont { get; set; } = Resources.GetFont(Resources.FontResources.NinaB);
        public Pen AxisPen { get; set; } = new Pen(Color.Black, 1);
        public Pen ChartPen { get; set; } = new Pen(Color.Green, 5);
        public int RadiusPoint { get; set; } = 10;
        public Brush EllipseColor { get; set; } = new SolidBrush(Color.Black);
        public Brush DivisionColor { get; set; } = new SolidBrush(Color.Black);
        public Brush TextColor { get; set; } = new SolidBrush(Color.Black);
        public Brush BackgroundColor { get; set; } = new SolidBrush(Color.White);

        private int _paddingLeft = 50;
        public string ChartTitle { get; set; } = "Chart1";
        public ArrayList Items { get; set; } //array of items

        private readonly int _width;
        private readonly int _height;
        private const int Margin = 50;
        private Point _start;
        private Point _end;

        public ChartService(int width, int height)
        {
            _width = width;
            _height = height;

            _start = new Point(Margin + _paddingLeft, height - Margin);
            _end = new Point(width - Margin, Margin + 100);
        }

        public Bitmap GetChart(ChartMode mode)
        {
            return mode switch
            {
                ChartMode.LineMode => GetLineChart(),
                ChartMode.RectangleMode => GetRectangleChart(),
                _ => null
            };
        }

        double GetMax(ArrayList data)
        {
            double max = 0;
            foreach(DataItem item in data)
            {
                if (item.Value > max) max = item.Value;
            }
            return max;
        }
        
        double GetMin(ArrayList data)
        {
            double min = double.MaxValue;
            foreach(DataItem item in data)
            {
                if (item.Value < min) min = item.Value;
            }
            return min;
        }

        private Bitmap GetLineChart()
        {
            var bitmap = new Bitmap(_width, _height);

            using var graph = Graphics.FromImage(bitmap);
            //graph.SmoothingMode = SmoothingMode.AntiAlias;
            //graph.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            //graph.InterpolationMode = InterpolationMode.High;

            var imageSize = new Rectangle(0, 0, _width, _height);
            graph.FillRectangle(BackgroundColor, imageSize.X,imageSize.Y,imageSize.Width,imageSize.Height);

            //title
            graph.DrawString(ChartTitle,ChartFont, TextColor,
                _width / 2 - (ChartTitle.Length / 2 * 18), 30);

            graph.DrawLine(AxisPen, Margin, Margin + 100, Margin, _height - Margin);
            graph.DrawLine(AxisPen, Margin, _height - Margin, _width - Margin, _height - Margin);

            var maxValue = Math.Ceiling(GetMax(Items));
            var minValue = (int) GetMin(Items);
            var countValue = this.Items.Count;

            var chartWidth = Math.Abs(_end.X - _start.X - 50);
            var chartHeight = Math.Abs(_end.Y - (_start.Y - 50));

            var divisionHeight = chartHeight / (maxValue - minValue);
            var divisionWidth = chartWidth / countValue;

            #region Draw divisions

            var startDivX = _start.X + divisionWidth;
            foreach (DataItem xx in Items)
            {
                var item = xx.Name;
                graph.FillEllipse(DivisionColor, startDivX - RadiusPoint / 2, _start.Y - RadiusPoint / 2,
                    RadiusPoint,
                    RadiusPoint);
                graph.DrawString(item.ToString(), ChartFont, TextColor,
                    startDivX - 7, _start.Y + Margin / 2 - 7);
                startDivX += divisionWidth * DivisionAxisX;
            }

            var startDivY = _start.Y - 25;
            for (var i = minValue; i <= maxValue; i += DivisionAxisY)
            {
                graph.FillEllipse(DivisionColor, _start.X - _paddingLeft - RadiusPoint / 2, startDivY - RadiusPoint / 2,
                    RadiusPoint,
                    RadiusPoint);
                graph.DrawString(i.ToString(), ChartFont, TextColor,
                    _start.X - _paddingLeft + Margin / 2, startDivY - 10);
                startDivY -= (int) (divisionHeight * DivisionAxisY);
            }

            #endregion

            #region Draw points

            var prevPoint = new Point();

            var ellipsePoints = new ArrayList(); //PointModel
            for (var i = 0; i < this.Items.Count; i++)
            {
                var item = (DataItem)this.Items[i];
                var pixelYValue = divisionHeight * item.Value -
                    divisionHeight * minValue + 25;
                var pixelXValue = divisionWidth * (i + 1);

                if (i > 0)
                {
                    var currentPoint = new Point(_start.X + pixelXValue, (int) (_start.Y - pixelYValue));
                    graph.DrawLine(ChartPen, prevPoint.X, prevPoint.Y, currentPoint.X , currentPoint.Y);
                }

                ellipsePoints.Add(new PointModel()
                {
                    Point = new Point(_start.X + pixelXValue, (int) (_start.Y - pixelYValue)),
                    Value = item.Value
                });

                prevPoint = new Point(_start.X + pixelXValue, (int) (_start.Y - pixelYValue));
            }

            foreach (PointModel pointModel in ellipsePoints)
            {
                graph.FillEllipse(EllipseColor, pointModel.Point.X - RadiusPoint / 2,
                    pointModel.Point.Y - RadiusPoint / 2, RadiusPoint, RadiusPoint);
                graph.DrawString($"({pointModel.Value})", ChartFont, TextColor,
                    pointModel.Point.X -
                    ChartFont.Height * pointModel.Value.ToString().Length,
                    pointModel.Point.Y - ChartFont.Height - 15);
            }

            #endregion

            return bitmap;
        }

        private Bitmap GetRectangleChart()
        {
            var bitmap = new Bitmap(_width, _height);

            using var graph = Graphics.FromImage(bitmap);
            //graph.SmoothingMode = SmoothingMode.AntiAlias;
            //graph.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            //graph.InterpolationMode = InterpolationMode.High;

            var imageSize = new Rectangle(0, 0, _width, _height);
            graph.FillRectangle(BackgroundColor, imageSize.X, imageSize.Y, imageSize.Width, imageSize.Height);

            //title
            graph.DrawString(ChartTitle, ChartFont, TextColor,
                _width / 2 - (ChartTitle.Length / 2 * 18), 30);

            graph.DrawLine(AxisPen, Margin, Margin + 100, Margin, _height - Margin);
            graph.DrawLine(AxisPen, Margin, _height - Margin, _width - Margin, _height - Margin);

            var maxValue = Math.Ceiling(GetMax(Items));
            var minValue = (int) GetMin(Items);
            var countValue = this.Items.Count;

            var chartWidth = Math.Abs(_end.X - _start.X - 50);
            var chartHeight = Math.Abs(_end.Y - (_start.Y - 50));

            var divisionHeight = chartHeight / (maxValue - minValue);
            var divisionWidth = chartWidth / countValue;

            #region Draw divisions

            var startDivX = _start.X + divisionWidth;
            foreach (DataItem xx in Items)
            {
                var item = xx.Name;
                graph.FillEllipse(DivisionColor, startDivX - RadiusPoint / 2, _start.Y - RadiusPoint / 2,
                    RadiusPoint,
                    RadiusPoint);
                graph.DrawString(item.ToString(), ChartFont, TextColor,
                    startDivX - 7, _start.Y + Margin / 2 - 7);
                startDivX += divisionWidth * DivisionAxisX;
            }

            var startDivY = _start.Y - 25;
            for (var i = minValue; i <= maxValue; i += DivisionAxisY)
            {
                graph.FillEllipse(DivisionColor, _start.X - _paddingLeft - RadiusPoint / 2, startDivY - RadiusPoint / 2,
                    RadiusPoint,
                    RadiusPoint);
                graph.DrawString(i.ToString(), ChartFont, TextColor,
                    _start.X - _paddingLeft + Margin / 2, startDivY - 10);
                startDivY -= (int) (divisionHeight * DivisionAxisY);
            }

            #endregion

            #region Draw rectangles

            for (var i = 0; i < this.Items.Count; i++)
            {
                var item = (DataItem)this.Items[i];
                var itemValue = item.Value;
                var pixelYValue = divisionHeight * itemValue -
                    divisionHeight * minValue + 25;
                var pixelXValue = divisionWidth * (i + 1);
                const int borderWidth = 2;

                graph.FillRectangle(EllipseColor, _start.X + pixelXValue - divisionWidth / 2,
                    _start.Y - (int) pixelYValue,
                    divisionWidth, (int) pixelYValue - borderWidth);

                var commonX = _start.X + pixelXValue - divisionWidth / 2;
                graph.DrawLine(new Pen(BackgroundColor, borderWidth), commonX,
                    _start.Y - borderWidth, commonX, _start.Y - (int) pixelYValue);
                graph.DrawLine(new Pen(BackgroundColor, borderWidth), commonX,
                    _start.Y - (int) pixelYValue, commonX + divisionWidth,
                    _start.Y - (int) pixelYValue);
                graph.DrawLine(new Pen(BackgroundColor, borderWidth),
                    commonX + divisionWidth,
                    _start.Y - (int) pixelYValue, commonX + divisionWidth,
                    _start.Y - borderWidth);

                graph.DrawString(itemValue.ToString(), ChartFont, TextColor, commonX,
                    _start.Y - (int) pixelYValue - ChartFont.Height - borderWidth);
            }
            #endregion

            return bitmap;
        }
    }
}