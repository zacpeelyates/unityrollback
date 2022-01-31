namespace Burst.Compiler.IL.Tests.Shared
{
    internal class TestStackalloc
    {
        [TestCompiler(1)]
        public static unsafe int StackallocInBranch(int takeBranch)
        {
            int* array = null;

            if (takeBranch != 0)
            {
                int* elem = stackalloc int[1];
                array = elem;
            }

            if (takeBranch != 0)
            {
                int* elem = stackalloc int[1];

                if (array == elem)
                {
                    return -1;
                }
            }

            return 0;
        }

        [TestCompiler(4)]
        public static unsafe int StackallocInLoop(int iterations)
        {
            int** array = stackalloc int*[iterations];

            for (int i = 0; i < iterations; i++)
            {
                int* elem = stackalloc int[1];
                array[i] = elem;
            }

            for (int i = 0; i < iterations; i++)
            {
                for (int k = i + 1; k < iterations; k++)
                {
                    // Make sure all the stack allocations within the loop are unique addresses.
                    if (array[i] == array[k])
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }
    }
}
