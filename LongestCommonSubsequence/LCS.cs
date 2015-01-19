using System;
using System.Collections.Generic;
using System.Threading;

namespace LongestCommonSubsequence
{
    class LCS<T>
    {
        #region ATTRIBUTES
        //sequences that will be compared
        private List<T> firstSequence;
        private List<T> secondSequence;

        //internal variables
        private Int16[,] LCSarray;
        private volatile int verticalDone = 0;
        private volatile int horizontalDone = 0;

        private Func<T, T, bool> isEqual;
        #endregion

        #region PUBLIC METHODS
        /* Class constructor
         * first argument : a list of objects
         * second argument : a list of objects
         * third argument : a function that compares the elements of the two lists
         * this function should return TRUE if two elements are equal and FALSE if not!
         * bool IsEqual(T a,T b)
         */
        public LCS(List<T> FirstSequence, List<T> SecondSequence, Func<T,T,bool> IsEqual)
        {
            if(FirstSequence == null || SecondSequence == null)
            {
                Console.Error.WriteLine("LCS: constructor got bad arguments.");
                return;
            }

            this.firstSequence = FirstSequence;
            this.secondSequence = SecondSequence;
            this.isEqual = IsEqual;

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

        /* This is the main method of the class that is used to find the 
         * longest common subsequence. When this method returns you can 
         * use either the method Length() to get the length of the longest
         * common subsequence or the GetLCS() to get the sequence. 
         */
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
                Console.Error.WriteLine(e.Message);
            }
        }

        /* This method will return the length of the common subsequence if
         * the LCS object has been properly initialized and the Run() method
         * is done.
         * In any other case it will throw a InvalidOperationException. 
         */
        public Int16 Length()
        {
            if (LCSarray != null)
            {
                if (LCSarray[firstSequence.Count, secondSequence.Count] != -1)
                {
                    return LCSarray[firstSequence.Count, secondSequence.Count];
                }
                else
                {
                    throw new InvalidOperationException("LCS initialized but LCS.Run() not yet completed");
                }
            }
            else
            {
                throw new InvalidOperationException("LCS object is not properly initialized");
            }
        }

        /* This method will return a List<T> with the common subsequence if the
         * LCS object has been properly initialized and the Run() method is done.
         * In any other case it will throw a InvalidOperationException. 
         */
        public List<T> GetLCS()
        {
            if (LCSarray != null)
            {
                if (LCSarray[firstSequence.Count, secondSequence.Count] != -1)
                {
                    return BacktrackPath(firstSequence.Count, secondSequence.Count);
                }
                else
                {
                    throw new InvalidOperationException("LCS initialized but LCS.Run() not yet completed");
                }
            }
            else
            {
                throw new InvalidOperationException("LCS object is not properly initialized");
            }
        }
        #endregion

        #region PRIVATE METHODS
        private List<T> BacktrackPath(int i, int j)
        {
            List<T> result = new List<T>();
            while (i > 1 && j > 1)
            {
                bool commonElement = false;
                try
                {
                    commonElement = isEqual(firstSequence[i - 1], secondSequence[j - 1]);
                }
                catch(Exception e)
                {
                    throw new Exception("IsEqual function passed to LCS object threw an exception", e);
                }
                if (commonElement)
                {
                    result.Add(firstSequence[i - 1]);
                    i -= 1;
                    j -= 1;
                }
                else
                {
                    if (LCSarray[i - 1, j] > LCSarray[i, j - 1])
                    {
                        i -= 1;
                    }
                    else
                    {
                        j -= 1;
                    }
                }

            }

            result.Reverse();
            return result;
        }

        private void HorizontalLCS()
        {
            int i;
            int j;

            for (i = 1; i < firstSequence.Count + 1; i++)
            {
                for (j = verticalDone + 1; j < secondSequence.Count + 1; j++)
                {
                    //LocksArray[i].EnterReadLock();
                    try
                    {
                        if (LCSarray[i, j] > -1)    //this case has been filled
                            continue;               //continue to the next one
                    }
                    finally
                    {
                        //LocksArray[i].ExitReadLock();
                    }

                    //if the case hasn't been filled then calculate the value
                    if (isEqual(firstSequence[i - 1], secondSequence[j - 1]))
                    {
                        //LocksArray[i].EnterWriteLock();
                        //Console.WriteLine("H");
                        try
                        {
                            Int16 val = LCSarray[i - 1, j - 1];
                            val++;
                            LCSarray[i, j] = val;
                        }
                        finally
                        {
                            //LocksArray[i].ExitWriteLock();
                        }
                    }
                    else
                    {
                        Int16 val = Max(LCSarray[i - 1, j], LCSarray[i, j - 1]);
                        //LocksArray[i].EnterWriteLock();
                        try
                        {
                            LCSarray[i, j] = val;
                        }
                        finally
                        {
                            // LocksArray[i].ExitWriteLock();
                        }

                    }
                }
                horizontalDone++;
            }
        }

        private void VerticalLCS()
        {
            int i;
            int j;

            for (j = 1; j < secondSequence.Count + 1; j++)
            {
                for (i = horizontalDone + 1; i < firstSequence.Count + 1; i++)
                {
                    //LocksArray[i].EnterReadLock();
                    try
                    {
                        if (LCSarray[i, j] > -1)    //this case has been filled
                            continue;               //continue to the next one
                    }
                    finally
                    {
                        //LocksArray[i].ExitReadLock();
                    }

                    //if the case hasn't been filled then calculate the value
                    if (isEqual(firstSequence[i - 1], secondSequence[j - 1]))
                    {
                        //LocksArray[i].EnterWriteLock();
                        // Console.WriteLine("V");
                        try
                        {
                            Int16 val = LCSarray[i - 1, j - 1];
                            val++;
                            LCSarray[i, j] = val;
                        }
                        finally
                        {
                            //LocksArray[i].ExitWriteLock();
                        }
                    }
                    else
                    {
                        Int16 val = Max(LCSarray[i - 1, j], LCSarray[i, j - 1]);
                        //LocksArray[i].EnterWriteLock();
                        try
                        {
                            LCSarray[i, j] = val;
                        }
                        finally
                        {
                            // LocksArray[i].ExitWriteLock();
                        }
                    }
                }
                verticalDone++;
            }
        }

        private Int16 Max(short p1, short p2)
        {
            if (p1 > p2)
                return p1;
            else
                return p2;
        }
        #endregion 
    }
}
