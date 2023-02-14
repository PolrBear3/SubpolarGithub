using System;

class Two_Sum
{
    static void Main(string[] args)
    {
        Console.WriteLine("insert first number");
        inputNums[0] = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("insert second number");
        inputNums[1] = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("insert third number");
        inputNums[2] = Convert.ToInt32(Console.ReadLine());
        
        Console.WriteLine("insert target");
        target = Convert.ToInt32(Console.ReadLine());
        
        Get_Output();
        Console.ReadKey();
    }
    
    static int[] inputNums = new int[3];
    static int target;

    static void Get_Output()
    {
        int checkNumIndex;
        int compareNumIndex;
        
        for (int i = 0; i < inputNums.Length; i++)
        {
            for (int j = 0; j < inputNums.Length; j++)
            {
                if (i == j) continue;

                if (inputNums[i] + inputNums[j] == target)
                {
                    checkNumIndex = i;
                    compareNumIndex = j;

                    Console.WriteLine(checkNumIndex);
                    Console.WriteLine(compareNumIndex);

                    return;
                }
            }
        }
    }
}