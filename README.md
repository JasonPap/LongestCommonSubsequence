# Longest Common Subsequence
A C# generic class implementation for finding the longest common subsequence of two lists. 

This implementation works with any type of data as long as the user provides the apropriated IsEqual() function.

Optimal performace is achieved by using two threads that run (almost) lock-free, each computing half of the array needed. 
Almost because there are two volatile attributes in the class.

# Subsequence != Substring
The longest common subsequence (LCS) problem is the problem of finding the longest subsequence common to all sequences in a set of sequences (often just two sequences). It differs from problems of finding common substrings: unlike substrings, subsequences are not required to occupy consecutive positions within the original sequences.
