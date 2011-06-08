namespace WATOR
{
    public abstract class Creature
    {
        //shark's life time in milliseconds
        private static int _lifeTime;
        private int _age;
        private Library.Pair _position;
        private bool _visible;

        /// <summary>
        /// Initializes a new instance of the <see cref="Creature"/> class.
        /// </summary>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="position">The position.</param>
        /// <remarks></remarks>
        public Creature(int lifeTime, Library.Pair position)
        {
            LifeTime = lifeTime;
            Position = position;
            Age = lifeTime;
        }

        /// <summary>
        /// Gets or sets the visibility.
        /// </summary>
        /// <value>The visibility.</value>
        /// <remarks></remarks>
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>The age.</value>
        /// <remarks></remarks>
        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                _age = value;
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        /// <remarks></remarks>
        public Library.Pair Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }


        /// <summary>
        /// Gets or sets the life time.
        /// </summary>
        /// <value>The life time.</value>
        /// <remarks></remarks>
        public int LifeTime
        {
            get
            {
                return _lifeTime;
            }
            set
            {
                if (value > 0)
                {
                    _lifeTime = value;
                }
            }
        }

    }
}
