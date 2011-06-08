using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;


namespace WATOR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _groundSize;
        private int _sharksCount;
        private int _sepiasCount;
        private int _sharksHungerRate;
        private int _sharksLife;
        private int _sepiasLife;
        private int _sepiasChildren;
        private int _sharksChildren;
        private Thread simulationThread;
        private Rectangle[,] fieldMap;
        private Simulation sim;


        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <remarks></remarks>
        public MainWindow()
        {
            InitializeComponent();
            sliderFieldSize.ValueChanged += new RoutedPropertyChangedEventHandler<double>(ChangeFieldSize);
        }


        /*
         * Initializing all variables and starting the simulation
         * */
        private void StartSimulation(object sender, RoutedEventArgs e)
        {
            if (simulationThread != null)
            {
                simulationThread.Abort();
            }
            _sepiasCount = (int)sliderSepiasCount.Value;
            _sharksCount = (int)sliderSharksCount.Value;
            _sharksHungerRate = (int)sliderSharksHunger.Value;
            _sharksLife = (int)sliderSharkLife.Value;
            _sepiasLife = (int)sliderSepiaLife.Value;
            _groundSize = (int)sliderFieldSize.Value;
            _sharksChildren = (int)sliderSepiasChildren.Value;
            _sepiasChildren = (int)sliderSharksChildren.Value;
            InitializeField();
            StartSimulation();
        }

        /*
         * Stopping the simulation
         * */
        private void StopSimulation(object sender, RoutedEventArgs e)
        {
            if (simulationThread != null)
            {
                simulationThread.Abort();
            }
        }

        /*
         * Running the thread which is containing the simulation.
         * First creating thread with infinite loop
         * where in Dispatcher.Invoke I'm displaying SimulationField array
         * which is representing the simulation field
         * */
        private void StartSimulation()
        {
            try
            {
                simulationThread = new Thread(new ThreadStart(
                    () =>
                    {
                        sim = new Simulation(new Library.Pair(_groundSize, _groundSize), _sharksLife,
                            _sharksHungerRate, _sepiasLife, _sharksCount, _sepiasCount, _sepiasChildren, _sharksChildren);
                        RefreshField();
                    }));
                simulationThread.IsBackground = true;
                simulationThread.Start();
            }
            catch (Exception) { }
        }

        /*
         * Refresing the field
         * */
        private void RefreshField()
        {
            while (true)
            {
                for (int i = 0; i < _groundSize; i++)
                {
                    for (int j = 0; j < _groundSize; j++)
                    {
                        Dispatcher.Invoke(new Action(
                            () =>
                            {
                                SetColor(i, j);
                            }));
                    }
                }
                Thread.Sleep(200);
            }
        }

        /*
         * Coloring the Rectangle with coordinates (x, y) in Canvas
         * */
        private void SetColor(int x, int y)
        {
            if (sim.SimulationField[x, y] is Shark)
            {
                fieldMap[x, y].Fill = Brushes.Red;
            }
            else if (sim.SimulationField[x, y] is Sepia)
            {
                fieldMap[x, y].Fill = Brushes.Green;
            }
            else
            {
                fieldMap[x, y].Fill = Brushes.CadetBlue;
            }
        }


        /*
         * Initializing the field
         * */
        private void InitializeField()
        {
            fieldMap = new Rectangle[_groundSize, _groundSize];
            double rectsSize = simulation.Width / _groundSize;
            simulation.Children.RemoveRange(0, simulation.Children.Count);
            for (int i = 0; i < _groundSize; i++)
            {
                for (int j = 0; j < _groundSize; j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = Brushes.DarkGray;
                    rect.Width = rectsSize - rectsSize / 30;
                    rect.Height = rectsSize - rectsSize / 30;
                    simulation.Children.Add(rect);
                    Canvas.SetLeft(rect, i * rectsSize);
                    Canvas.SetTop(rect, j * rectsSize);
                    fieldMap[j, i] = rect;
                }
            }
        }


        /*
         * Controlling shark's and sepia's count depending on fieldsize
         * */
        private void ChangeFieldSize(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderSepiasCount.Maximum = sliderFieldSize.Value * sliderFieldSize.Value;
            sliderSharksCount.Maximum = sliderFieldSize.Value * sliderFieldSize.Value;
        }


        /*
         * Controlling sepia's count
         * */
        private void ChangeSharksCount(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderSepiasCount.Maximum = sliderFieldSize.Value * sliderFieldSize.Value - sliderSharksCount.Value;
        }


        /*
         * Controlling shark's count
         * */
        private void ChangeSepiasCount(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderSharksCount.Maximum = sliderFieldSize.Value * sliderFieldSize.Value - sliderSepiasCount.Value;
        }


    }
}
