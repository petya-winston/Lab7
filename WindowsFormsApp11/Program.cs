using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp11
{
    public class Snowflake
    {
        public string Shape { get; private set; }
        public string Color { get; private set; }
        public int Size { get; private set; }

        public Snowflake(string shape, string color, int size)
        {
            Shape = shape;
            Color = color;
            Size = size;
        }

        public void Draw(Graphics graphics, int x, int y)
        {
            using (Brush brush = new SolidBrush(ColorTranslator.FromHtml(Color)))
            {
                graphics.FillEllipse(brush, x, y, Size, Size);
            }
        }
    }


    public class SnowflakeFactory
    {
        private readonly Dictionary<string, Snowflake> _snowflakes = new Dictionary<string, Snowflake>();

        public Snowflake GetSnowflake(string shape, string color, int size)
        {
            string key = shape + "-" + color + "-" + size;
            if (!_snowflakes.ContainsKey(key))
            {
                _snowflakes[key] = new Snowflake(shape, color, size);
            }

            return _snowflakes[key];
        }
    }


    public class MainForm : Form
    {
        private readonly Random _random = new Random();
        private readonly SnowflakeFactory _factory = new SnowflakeFactory();
        private Timer _timer;
        private List<Tuple<Snowflake, int, int>> _snowflakes = new List<Tuple<Snowflake, int, int>>();

        public MainForm()
        {
            this.Text = "Snow Simulation with Flyweight";
            this.Width = 800;
            this.Height = 600;

            Button startButton = new Button { Text = "Start Snow", Left = 50, Top = 20, Width = 100 };
            startButton.Click += StartSnow;
            this.Controls.Add(startButton);

            Button stopButton = new Button { Text = "Stop Snow", Left = 170, Top = 20, Width = 100 };
            stopButton.Click += StopSnow;
            this.Controls.Add(stopButton);

            this.Paint += OnPaint;
            this.DoubleBuffered = true;

            _timer = new Timer();
            _timer.Interval = 100;
            _timer.Tick += TimerTick;
        }

        private void StartSnow(object sender, EventArgs e)
        {
            _timer.Start();
        }

        private void StopSnow(object sender, EventArgs e)
        {
            _timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                string shape = "circle";
                string color = GetRandomColor();
                int size = _random.Next(5, 15);
                int x = _random.Next(0, this.Width);
                int y = 0;

                Snowflake snowflake = _factory.GetSnowflake(shape, color, size);
                _snowflakes.Add(new Tuple<Snowflake, int, int>(snowflake, x, y));
            }

            for (int i = 0; i < _snowflakes.Count; i++)
            {
                var tuple = _snowflakes[i];
                _snowflakes[i] = new Tuple<Snowflake, int, int>(tuple.Item1, tuple.Item2, tuple.Item3 + _random.Next(2, 5));
            }

            _snowflakes.RemoveAll(s => s.Item3 > this.Height);

            this.Invalidate();
        }

        private string GetRandomColor()
        {
            string[] colors = { "#FFFFFF", "#AEEEEE", "#87CEFA", "#F0FFFF" };
            return colors[_random.Next(colors.Length)];
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            foreach (var tuple in _snowflakes)
            {
                tuple.Item1.Draw(e.Graphics, tuple.Item2, tuple.Item3);
            }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
