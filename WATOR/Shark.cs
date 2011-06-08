namespace WATOR
{
    /// <summary>
    /// Class which is representing the Shark
    /// </summary>
    /// <remarks></remarks>
    public class Shark : Creature
    {

        //shark's hunger
        private int _hunger;
        //shark's maximum hunger, when _hunger is equals to _maxHunger the shark's die
        private static int _maxHunger;


        /// <summary>
        /// Initializes a new instance of the <see cref="Shark"/> class.
        /// </summary>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="hunger">The hunger.</param>
        /// <param name="maxHunger">The max hunger.</param>
        /// <remarks></remarks>
        public Shark(int lifeTime, int hunger, int maxHunger, Library.Pair position) : base(lifeTime, position)
        {
            Hunger = hunger;
            MaxHunger = maxHunger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Shark() : this(0, 0, 100, new Library.Pair()) { }


        /// <summary>
        /// Gets or sets the hunger.
        /// </summary>
        /// <value>The hunger.</value>
        /// <remarks></remarks>
        public int Hunger
        {
            get
            {
                return _hunger;
            }
            set
            {
                if (value >= 0)
                {
                    _hunger = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the max hunger.
        /// </summary>
        /// <value>The max hunger.</value>
        /// <remarks></remarks>
        public int MaxHunger
        {
            get
            {
                return _maxHunger;
            }
            set
            {
                if (value >= 0)
                {
                    _maxHunger = value;
                }
            }
        }


    }
}
