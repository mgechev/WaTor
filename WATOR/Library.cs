namespace WATOR
{
    public class Library
    {

        public struct Pair
        {
            private int _x;
            private int _y;

            /// <summary>
            /// Initializes a new instance of the <see cref="Pair"/> struct.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <remarks></remarks>
            public Pair(int x, int y)
            {
                _x = x;
                _y = y;
            }

            /// <summary>
            /// Gets or sets the X.
            /// </summary>
            /// <value>The X.</value>
            /// <remarks></remarks>
            public int X
            {
                get 
                { 
                    return _x;
                }
                set
                {
                    _x = value; 
                }
            }

            /// <summary>
            /// Gets or sets the Y.
            /// </summary>
            /// <value>The Y.</value>
            /// <remarks></remarks>
            public int Y
            {
                get
                { 
                    return _y; 
                }
                set
                { 
                    _y = value;
                }
            }

        }

    }
}
