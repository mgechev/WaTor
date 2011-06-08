namespace WATOR
{
    public class Sepia : Creature
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Sepia"/> class.
        /// </summary>
        /// <param name="lifeTime">The life time.</param>
        /// <remarks></remarks>
        public Sepia(int lifeTime, Library.Pair position) : base(lifeTime, position)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Sepia() : this(0, new Library.Pair()) { }

    }
}
