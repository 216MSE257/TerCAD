using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiniCadProject {
    public partial class MainWindow : Window {
        private List<UIElement> _elements = new List<UIElement>();
        private UIElement? _selected = null;
        private Point _lastPt = new Point(0, 0);
        private bool _ortho = false;

        public MainWindow() {
            InitializeComponent();
            CmdInput.Focus();
            this.KeyDown += (s, e) => {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z) Undo();
                if (e.Key == Key.F8) { _ortho = !_ortho; StatusTxt.Text = _ortho ? "ORTHO: AÇIK" : "ORTHO: KAPALI (F8)"; }
                if (e.Key == Key.Delete) DeleteSelected();
            };
        }

        private double Eval(string ex) { try { return Convert.ToDouble(new DataTable().Compute(ex.Replace(",", "."), "")); } catch { return 0; } }
        private Point Parse(string i) {
            bool r = i.StartsWith("@"); string c = r ? i.Substring(1) : i; string[] s = c.Split(',');
            double x = Eval(s[0]), y = Eval(s[1]);
            return r ? new Point(_lastPt.X + x, _lastPt.Y + y) : new Point(x, y);
        }

        private void CmdInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) { Execute(CmdInput.Text.Trim().ToUpper()); CmdInput.Text = ""; }
        }

        private void Execute(string cmd) {
            Log("> " + cmd); string[] p = cmd.Split(' ');
            try {
                switch (p[0]) {
                    case "L": DrawLine(Parse(p[1]), Parse(p[2])); break;
                    case "REC": DrawRect(Parse(p[1]), Parse(p[2])); break;
                    case "C": DrawCircle(Parse(p[1]), Eval(p[2])); break;
                    case "POL": DrawPoly(Parse(p[1]), (int)Eval(p[2]), Eval(p[3])); break;
                    case "U": Undo(); break;
                    case "EXP": Export(); break;
                    case "CLEAR": DrawingCanvas.Children.Clear(); _elements.Clear(); break;
                    default: Log("KOMUT HATALI"); break;
                }
            } catch { Log("PARAMETRE HATASI"); }
        }

        private void Add(UIElement el) { DrawingCanvas.Children.Add(el); _elements.Add(el); el.MouseDown += (s, e) => { Select(el); e.Handled = true; }; }

        private void DrawLine(Point s, Point e) {
            var l = new Line { X1=s.X, Y1=s.Y, X2=e.X, Y2=e.Y, Stroke=GetCol(), StrokeThickness=ThickSlider.Value };
            Add(l); _lastPt = e;
        }

        private void DrawRect(Point p1, Point p2) {
            DrawLine(p1, new Point(p2.X, p1.Y)); DrawLine(new Point(p2.X, p1.Y), p2);
            DrawLine(p2, new Point(p1.X, p2.Y)); DrawLine(new Point(p1.X, p2.Y), p1);
        }

        private void DrawPoly(Point center, int sides, double r) {
            Point prev = new Point();
            for (int i = 0; i <= sides; i++) {
                double a = 2 * Math.PI * i / sides;
                Point curr = new Point(center.X + r * Math.Cos(a), center.Y + r * Math.Sin(a));
                if (i > 0) DrawLine(prev, curr); prev = curr;
            }
        }

        private void DrawCircle(Point c, double r) {
            var el = new Ellipse { Width=r*2, Height=r*2, Stroke=GetCol(), StrokeThickness=ThickSlider.Value };
            Canvas.SetLeft(el, c.X-r); Canvas.SetTop(el, c.Y-r); Add(el);
        }

        private void Select(UIElement el) { if(_selected is Shape sOld) sOld.Stroke=Brushes.White; _selected=el; if(_selected is Shape sNew) sNew.Stroke=Brushes.Red; }
        private void Undo() { if(_elements.Count > 0) { var el=_elements.Last(); _elements.Remove(el); DrawingCanvas.Children.Remove(el); } }
        private void DeleteSelected() { if(_selected != null) { DrawingCanvas.Children.Remove(_selected); _elements.Remove(_selected); } }

        private void Export() {
            using (StreamWriter sw = new StreamWriter("Cizim.dxf")) {
                sw.WriteLine("0\nSECTION\n2\nENTITIES");
                foreach (var l in _elements.OfType<Line>()) sw.WriteLine($"0\nLINE\n8\n0\n10\n{l.X1}\n20\n{l.Y1}\n11\n{l.X2}\n21\n{l.Y2}");
                sw.WriteLine("0\nENDSEC\n0\nEOF");
            }
            Log("KAYDEDİLDİ: Cizim.dxf");
        }

        private Brush GetCol() => new BrushConverter().ConvertFromString(((ComboBoxItem)ColorPicker.SelectedItem).Tag.ToString()) as Brush;
        private void Log(string m) { CmdHistory.Items.Add(m); CmdHistory.ScrollIntoView(m); }
        
        private void OnMouseMove(object s, MouseEventArgs e) {
            Point p = e.GetPosition(DrawingCanvas);
            foreach (var l in _elements.OfType<Line>()) {
                if (Dist(p, new Point(l.X1, l.Y1)) < 15) { p = new Point(l.X1, l.Y1); SnapMarker.Visibility=Visibility.Visible; break; }
                if (Dist(p, new Point(l.X2, l.Y2)) < 15) { p = new Point(l.X2, l.Y2); SnapMarker.Visibility=Visibility.Visible; break; }
                SnapMarker.Visibility=Visibility.Collapsed;
            }
            if (SnapMarker.Visibility == Visibility.Visible) { Canvas.SetLeft(SnapMarker, p.X-5); Canvas.SetTop(SnapMarker, p.Y-5); }
            CoordTxt.Text = $"X: {(int)p.X} Y: {(int)p.Y}";
        }
        private double Dist(Point a, Point b) => Math.Sqrt(Math.Pow(a.X-b.X,2)+Math.Pow(a.Y-b.Y,2));
        private void ColorPicker_Changed(object s, SelectionChangedEventArgs e) { if(_selected is Shape sh) sh.Stroke=GetCol(); }
        private void Slider_Changed(object s, RoutedPropertyChangedEventArgs<double> e) { if(_selected is Shape sh) sh.StrokeThickness=e.NewValue; }
        private void OnMouseWheel(object s, MouseWheelEventArgs e) { var m=MainTransform.Matrix; double sc=e.Delta>0?1.1:1/1.1; m.ScaleAt(sc,sc,e.GetPosition(DrawingCanvas).X,e.GetPosition(DrawingCanvas).Y); MainTransform.Matrix=m; }
        private void OnMouseDown(object s, MouseButtonEventArgs e) { if(e.LeftButton==MouseButtonState.Pressed) CmdInput.Focus(); }
    }
}