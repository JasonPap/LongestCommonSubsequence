
using System;
using System.Collections.Generic;
namespace LongestCommonSubsequence
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> sequenceOne = new List<int>();    //multiples of 2
            List<int> sequenceTwo = new List<int>();    //multiples of 4

            for (int i = 0; i < 40; i+=2)
			{
                sequenceOne.Add(i);
                if (i % 4 == 0)
                    sequenceTwo.Add(i);
			}

            LCS<int> lcs = new LCS<int>(sequenceOne, sequenceTwo, IsEqual);
            lcs.Run();
            Console.WriteLine("The size of the longest common subsequence is: {0}", lcs.Length());
            Console.WriteLine("And the seqeunce is: ");
            PrintIntList(lcs.GetLCS());
            Console.WriteLine("Press Enter to exit the example");
            Console.ReadLine();
        }

        public static bool IsEqual(int A, int B)
        {
            if (A == B)
                return true;
            else
                return false;
        }

        private static void PrintIntList(List<int> l)
        {
            foreach ( int x in l)
                Console.WriteLine(x.ToString());
        }
    }
}
