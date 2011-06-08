using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace WATOR
{
    public class Simulation
    {
        #region Private variables

        private const int MAX_SECTOR_SIZE = 25;
        private int _sharksCount;
        private int _sepiasCount;
        private int _sepiasChildren;
        private int _sharksChildren;
        private int _sepiasLifeTime;
        private int _sharksLifeTime;
        private int _sharksHunger;
        private List<Library.Pair> _positionsCatalog;
        private Barrier _barrier;
        private Creature[,] _simulationField;

        #endregion

        #region Constructor and field initializing
        /// <summary>
        /// Initializes a new instance of the <see cref="Simulation"/> class.
        /// </summary>
        /// <param name="playGroundSize">Size of the play ground.</param>
        /// <param name="sharksLifeTime">The sharks life time.</param>
        /// <param name="sharksHunger">The sharks hunger.</param>
        /// <param name="sepiasLifeTime">The sepias life time.</param>
        /// <param name="sharksCount">The sharks count.</param>
        /// <param name="sepiasCount">The sepias count.</param>
        /// <remarks></remarks>
        public Simulation(Library.Pair playGroundSize, int sharksLifeTime, int sharksHunger,
            int sepiasLifeTime, int sharksCount, int sepiasCount, int sepiasChildren, int sharksChildren)
        {
            _sharksHunger = sharksHunger;
            _sharksLifeTime = sharksLifeTime;
            _sepiasLifeTime = sepiasLifeTime;
            if (sharksCount + sepiasCount > (playGroundSize.Y * playGroundSize.X))
            {
                throw new Exception("Too much creatures!");
            }
            if (playGroundSize.X < 5 || playGroundSize.Y < 5)
            {
                throw new Exception("Too small field");
            }

            _positionsCatalog = new List<Library.Pair>();
            _positionsCatalog.Add(new Library.Pair(-1, -1));
            _positionsCatalog.Add(new Library.Pair(0, -1));
            _positionsCatalog.Add(new Library.Pair(1, -1));
            _positionsCatalog.Add(new Library.Pair(-1, 0));
            _positionsCatalog.Add(new Library.Pair(1, 0));
            _positionsCatalog.Add(new Library.Pair(-1, 1));
            _positionsCatalog.Add(new Library.Pair(0, 1));
            _positionsCatalog.Add(new Library.Pair(1, 1));

            _sepiasChildren = sepiasChildren;
            _sharksChildren = sharksChildren;
            SepiasCount = sepiasCount;
            SharksCount = sharksCount;
            InitializeField(playGroundSize);
            Thread run = new Thread(new ThreadStart(RunSimulation));
            run.IsBackground = true;
            run.Start();
        }

        /*
         * Initializing the field
         * */
        private void InitializeField(Library.Pair size)
        {
            _simulationField = new Creature[size.X, size.Y];


            for (int i = 0; i < _sharksCount; i++)
            {
                Library.Pair position = FindPlaceForCreature();
                Shark shark = new Shark(_sharksLifeTime, 0, _sharksHunger, position);
                _simulationField[position.X, position.Y] = shark;
            }
            for (int i = 0; i < _sepiasCount; i++)
            {
                Library.Pair position = FindPlaceForCreature();
                Sepia sepia = new Sepia(_sepiasLifeTime, position);
                _simulationField[position.X, position.Y] = sepia;
            }


        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sepias count.
        /// </summary>
        /// <value>The sepias count.</value>
        /// <remarks></remarks>
        public int SepiasCount
        {
            get
            {
                return _sepiasCount;
            }
            set
            {
                if (value > 0)
                {
                    _sepiasCount = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the sharks count.
        /// </summary>
        /// <value>The sharks count.</value>
        /// <remarks></remarks>
        public int SharksCount
        {
            get
            {
                return _sharksCount;
            }
            set
            {
                if (value > 0)
                {
                    _sharksCount = value;
                }
            }
        }

        /// <summary>
        /// Gets the simulation field.
        /// </summary>
        public Creature[,] SimulationField
        {
            get
            {
                return _simulationField;
            }
        }
        #endregion

        #region Controlling threads
        /*
         * Running the simulation
         * */
        private void RunSimulation()
        {
            int sideSize = (int)Math.Ceiling((double)_simulationField.GetLength(0) / (double)MAX_SECTOR_SIZE);
            int threadsCount = sideSize * sideSize;
            
            _barrier = new Barrier(0);
            while (true)
            {

                if (_barrier.ParticipantCount > 0)
                {
                    _barrier.SignalAndWait();
                }

                for (int sector = 0; sector <= threadsCount; sector++)
                {
                    _barrier.AddParticipant();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(new Action<object>(DoSector)),
                       sector);
                }

                Thread.Sleep(200);
            }
        }


        /*
         * Calculating the coordinates of any sector
         * */
        private Library.Pair GetSectorPosition(int sector)
        {
            int sectorsPerRow = (int)Math.Ceiling((double)_simulationField.GetLength(0) / (double)MAX_SECTOR_SIZE);
            int onRow = (sector / sectorsPerRow);
            int onCol = (sector % sectorsPerRow);
            return new Library.Pair(onRow * MAX_SECTOR_SIZE, onCol * MAX_SECTOR_SIZE);
        }


        /*
         * Moving all creatures for the sector obj
         * */
        private void DoSector(object obj)
        {
            int sector = (int)obj;
            Library.Pair sectorPosition = GetSectorPosition(sector);
            for (int i = sectorPosition.X; i < sectorPosition.X + MAX_SECTOR_SIZE && i < _simulationField.GetLength(0); i++)
            {
                for (int j = sectorPosition.Y; j < sectorPosition.Y + MAX_SECTOR_SIZE && j < _simulationField.GetLength(1); j++)
                {
                    if (_simulationField[i, j] != null)
                    {
                        MoveCreature(_simulationField[i, j], i, j);
                    }
                }
            }
            _barrier.RemoveParticipant();
        }
        #endregion

        #region Spawn methods
        /*
         * Checking if shark is old enough to have children
         * */
        private bool CheckForSpawnShark(Shark creature)
        {
            if (creature.Age == 2 * creature.LifeTime / 3 && creature.Hunger != 0)
            {
                SpawnShark(creature);
                return true;
            }
            return false;
        }

        /*
         * Shark spawn
         * */
        private void SpawnShark(Shark creature)
        {
            for (int i = 0; i < _sepiasChildren; i++)
            {
                Creature child;
                child = new Shark(creature.LifeTime, 0, ((Shark)creature).MaxHunger, new Library.Pair());
                if (i == 0)
                {
                    KillCreature(creature.Position.X, creature.Position.Y);
                    child.Position = creature.Position;
                }
                else
                {
                    child.Position = FindPlaceForShark(creature.Position.X, creature.Position.Y);
                }
                _simulationField[child.Position.X, child.Position.Y] = child;
            }
        }

        /*
         * Checking if sepia is old enough to have children
         * */
        private bool CheckForSpawnSepia(Sepia creature)
        {
            if (creature.Age == 2 * creature.LifeTime / 3)
            {
                SpawnSepia(creature);
                return true;
            }
            return false;
        }

        /*
         * Sepia spawn
         * */
        private void SpawnSepia(Sepia creature)
        {
            for (int i = 0; i < _sepiasChildren; i++)
            {
                Creature child;
                child = new Sepia(creature.LifeTime, new Library.Pair());
                if (i == 0)
                {
                    KillCreature(creature.Position.X, creature.Position.Y);
                    child.Position = creature.Position;
                }
                else
                {
                    child.Position = FindPlaceForSepia(child, creature.Position.X, creature.Position.Y);
                }
                _simulationField[child.Position.X, child.Position.Y] = child;
            }
        }

        /*
         * Increacing the population if any creature is older than 1/3 of it's life
         * */
        private bool CheckForSpawn(Creature creature)
        {
            if (creature is Shark)
            {
                return CheckForSpawnShark((Shark)creature);
            }
            else
            {
                return CheckForSpawnSepia((Sepia)creature);
            }
        }

        #endregion

        #region Move methods

        /*
         * Method used for moving any creature, it's checking
         * if the creature is old enough for spawn and is it
         * old enough to die
         * */
        private void MoveCreature(Creature creature, int x, int y)
        {
            if (creature != null)
            {
                if (!IsAlive(creature, x, y))
                {
                    return;
                }
                if (CheckForSpawn(creature))
                {
                    return;
                }
                if (creature is Shark)
                {
                    MoveShark((Shark)creature, x, y);
                }
                else
                {
                    MoveSepia((Sepia)creature, x, y);
                }
            }
        }

        /*
         * Moving Shark
         * */
        private void MoveShark(Shark creature, int x, int y)
        {
            Library.Pair position = FindPlaceForShark(x, y);
            if (_simulationField[position.X, position.Y] is Sepia)
            {
                KillCreature(position.X, position.Y);
                creature.Hunger = 0;
            }
            _simulationField[x, y] = null;
            _simulationField[position.X, position.Y] = creature;
        }

        /*
         * Moveing Sepia
         * */
        private void MoveSepia(Sepia creature, int x, int y)
        {
            Library.Pair position = FindPlaceForSepia(creature, x, y);
            if (_simulationField[position.X, position.Y] is Shark)
            {
                KillCreature(x, y);
                try
                {
                    ((Shark)_simulationField[position.X, position.Y]).Hunger = 0;
                }
                catch (Exception) { }
            }
            else
            {
                _simulationField[x, y] = null;
                _simulationField[position.X, position.Y] = creature;
            }
        }

        #endregion

        #region Positioning creatures
        /*
         * Finding next position for any Shark. First the creature is looking
         * for Sepia in the near squares if it did not find it's moving to another free random square.
         * This random position is freePosition variable.
         * */
        private Library.Pair FindPlaceForShark(int x, int y)
        {
            MixPositions();
            Library.Pair freePosition = new Library.Pair();
            bool setFreePosition = false;
            for (int i = 0; i < _positionsCatalog.Count; i++)
            {
                if (IsLegal(_simulationField[x, y], x + _positionsCatalog[i].X, y + _positionsCatalog[i].Y))
                {
                    if (_simulationField[x + _positionsCatalog[i].X, y + _positionsCatalog[i].Y] is Sepia)
                    {
                        return new Library.Pair(x + _positionsCatalog[i].X, y + _positionsCatalog[i].Y);
                    }                    
                    else if (!setFreePosition)
                    {
                        freePosition = new Library.Pair(x + _positionsCatalog[i].X, y + _positionsCatalog[i].Y);
                        setFreePosition = true;
                    }
                }
            }
            return freePosition;
        }

        /*
         * Generating random index which isn't going to surpass positionCatalog's limits
         * */
        private int GetRandomPositionIndex()
        {
            DateTime date = DateTime.Now;
            float minutes = date.Minute;
            float seconds = date.Second;
            float hours = date.Hour;
            Random random = new Random();
            return random.Next(0, (int)((minutes * seconds) + hours) % _positionsCatalog.Count);
        }

        /*
         * This method is mixing all possible future positions for random movement.
         * */
        private void MixPositions()
        {
            Random position = new Random();
            for (int i = 0; i < _positionsCatalog.Count; i++)
            {
                int firstIndex = GetRandomPositionIndex();
                int secondIndex = GetRandomPositionIndex();
                Library.Pair temp = _positionsCatalog[firstIndex];
                _positionsCatalog[firstIndex] = _positionsCatalog[secondIndex];
                _positionsCatalog[secondIndex] = temp;
            }
        }

        /*
         * Finding the next position for any creature with current coordinates (x, y)
         * */
        private Library.Pair FindPlaceForSepia(Creature creature, int x, int y)
        {
            bool positionFound = false;
            Random rand = new Random();
            Library.Pair newPosition = new Library.Pair(0, 0);
            MixPositions();
            for (int i = 0; i < _positionsCatalog.Count; i++)
            {
                if (IsLegal(creature, _positionsCatalog[i].X + x, _positionsCatalog[i].Y + y))
                {
                    newPosition.X = _positionsCatalog[i].X + x;
                    newPosition.Y = _positionsCatalog[i].Y + y;
                    positionFound = true;
                }
            }
            if (!positionFound)
            {
                newPosition.X = x;
                newPosition.Y = y;
            }
            return newPosition;
        }

        /*
         * Checking of the creature - creature can
         * change it's location to position (x, y)
         * */
        private bool IsLegal(Creature creature, int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return false;
            }
            if (x >= _simulationField.GetLength(0) || y >= _simulationField.GetLength(1))
            {
                return false;
            }
            if (creature is Sepia && _simulationField[x, y] is Sepia)
            {
                return false;
            }
            if (creature is Shark && _simulationField[x, y] is Shark)
            {
                return false;
            }
            return true;
        }

        /*
         * Finding random place for creature
         * */
        private Library.Pair FindPlaceForCreature()
        {
            Random rand = new Random();
            Library.Pair position = new Library.Pair(0, 0);
            do
            {
                position.X = rand.Next(0, _simulationField.GetLength(0));
                position.Y = rand.Next(0, _simulationField.GetLength(1));

            } while (_simulationField[position.X, position.Y] != null);

            return position;
        }

        #endregion

        #region Controlling creatures

        /*
         * This method is checking if any creature is old enough to be removed
         * from the simulation ground. If the creature is shark
         * this method is also checking if it's hunger.
         * */
        private bool IsAlive(Creature creature, int x, int y)
        {
            if (creature.Age <= 0)
            {
                KillCreature(x, y);
                return false;
            }
            else
            {
                creature.Age--;
            }
            if (creature is Shark)
            {
                if (((Shark)creature).Hunger >= ((Shark)creature).MaxHunger)
                {
                    KillCreature(x, y);
                    return false;
                }
                else
                {
                    ((Shark)creature).Hunger++;
                }
            }
            return true;
        }

        /*
         * This method is killing any creature
         * */
        private void KillCreature(int x, int y)
        {
            _simulationField[x, y] = null;
        }

        #endregion

    }
}
