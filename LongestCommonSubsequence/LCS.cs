using System;
using System.Collections.Generic;
using System.Threading;

namespace LongestCommonSubsequence
{
    class LCS<T>
    {
        //-----------------------------------------//
        //-------Attributes declaration------------//
        
        //sequences that will be compared
        private List<T> firstSequence;
        private List<T> secondSequence;

        //internal variables
        private Int16[,] LCSarray;
        private volatile int verticalDone = 0;
        private volatile int horizontalDone = 0;
        //-----------------------------------------//

        /* Class constructor
         * first argument : a list of objects
         * second argument : a list of objects
         * third argument : a function that compares the elements of the two lists
         * this function should return TRUE if two elements are equal and FALSE if not!
         */
        public LCS(List<T> FirstSequence, List<T> SecondSequence)
        {
            if(FirstSequence == null || SecondSequence == null)
            {
                Console.Error.WriteLine("LCS: constructor got bad arguments.");
                return;
            }

            this.firstSequence = FirstSequence;
            this.secondSequence = SecondSequence;
            int length1 = firstSequence.Count;
            int length2 = secondSequence.Count;
            try
            {
                int i = 0, j = 0;
                LCSarray = new Int16[length1 + 1, length2 + 1];
                for (j = 0; j < length2 + 1; j++)
                {
                    LCSarray[i, j] = 0;
                }

                j = 0;
                for (i = 0; i < length1 + 1; i++)
                {
                    LCSarray[i, j] = 0;
                }

                for (i = 1; i < length1 + 1; i++)
                    for (j = 1; j < length2 + 1; j++)
                        LCSarray[i, j] = -1;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine("LCS: could not properply initialize");
                Console.Error.WriteLine(e.Message);
                return;
            }
        }

        public void Run()
        {
            //start horizontal thread
            Thread horizontalThread = new Thread(new ThreadStart(this.HorizontalLCS));
            horizontalThread.Start();
            //start vertical thread
            Thread verticalThread = new Thread(new ThreadStart(this.VerticalLCS));
            verticalThread.Start();

            //wait for them to finish
            try
            {
                verticalThread.Join();
                horizontalThread.Join();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public string Length()
        {
            if (LCSarray != null)
                if (LCSarray[length1, length2] != -1)
                    return LCSarray[length1, length2].ToString();

            return "null";
        }
    }
}
