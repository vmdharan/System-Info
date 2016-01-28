using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace System_Info
{
    /// <summary>
    /// Interaction logic for ProgressArc.xaml
    /// </summary>
    public partial class ProgressArc : UserControl
    {
        public ProgressArc()
        {
            InitializeComponent();

            Angle = (Percentage / 100.0) * 360.0;
            calculateProgress();
        }

        // Calculate progress based on parameters.
        public void calculateProgress()
        {
            Point p1, p2;
            double rAngle;

            // Get angle in radians.
            rAngle = (Math.PI / 180.0) * (Angle - 90);
            
            // Compute start and end points.
            p1 = new Point(Radius, 0);
            p2 = new Point(Radius * Math.Cos(rAngle), Radius * Math.Sin(rAngle));

            p2.X += Radius;
            p2.Y += Radius;

            pathRoot.Width = Radius * 2 + StrokeThickness;
            pathRoot.Height = Radius * 2 + StrokeThickness;
            pathRoot.Margin = new Thickness(StrokeThickness, StrokeThickness, 0, 0);

            bool largeArc = Angle > 180.0;
            Size outerArcSize = new Size(Radius, Radius);

            pathFigure.StartPoint = p1;

            if((p1.X == Math.Round(p2.X)) && (p1.Y == Math.Round(p2.Y)))
            {
                p2.X -= 0.01;
            }

            arcSegment.Point = p2;
            arcSegment.Size = outerArcSize;
            arcSegment.IsLargeArc = largeArc;
        }

        // Update progress
        private static void OnPercentageChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            ProgressArc arc = sender as ProgressArc;
            arc.Angle = (arc.Percentage / 100.0) * 360.0;
        }

        // Update property
        private static void OnPropertyChanged(DependencyObject sender, 
            DependencyPropertyChangedEventArgs eventArgs)
        {
            ProgressArc arc = sender as ProgressArc;
            arc.calculateProgress();
        }

        // Angle
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(ProgressArc), 
                new PropertyMetadata(90d, new PropertyChangedCallback(OnPropertyChanged)));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        // Radius
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(ProgressArc),
                new PropertyMetadata(20.0, new PropertyChangedCallback(OnPropertyChanged)));

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        // Progress
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(ProgressArc),
                new PropertyMetadata(50.0, new PropertyChangedCallback(OnPercentageChanged)));

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        // Color
        public static readonly DependencyProperty SegmentColorProperty =
            DependencyProperty.Register("SegmentColor", typeof(Brush), typeof(ProgressArc),
                new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush SegmentColor
        {
            get { return (Brush)GetValue(SegmentColorProperty); }
            set { SetValue(SegmentColorProperty, value); }
        }

        // Thickness
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(int), typeof(ProgressArc),
                new PropertyMetadata(10));

        public int StrokeThickness
        {
            get { return (int)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
    }
}
