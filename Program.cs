//*****************************************************************************
//** 2463. Minimum Total Distance Traveled    leetcode hard                  **
//** The first commented out section shows where I started vs. where I ended **
//** up.  It was a long road with multiple errors.  But in the end it worked **
//*****************************************************************************


/*

long long minimumTotalDistance(int* robot, int robotSize, int** factory, int factorySize, int* factoryColSize) {
    long long totalDistance = 0;
    long long currentDistance = INT_MAX;
    long long bestDistance = 0;
    for(int i = 0; i < robotSize; i++)
    {
        bestDistance = 0;
        long long currentRobot = robot[i];
        int factoryNumber = 0;
        for(int j = 0; j < factorySize; j++)
        {
            if(factory[j][1] > 0)
            {
                if(currentRobot > factory[j][0]) bestDistance = currentRobot - factory[j][0];
                else bestDistance = factory[j][0] - currentRobot;
                printf("Best = %d\n",bestDistance);
                if(bestDistance < currentDistance) 
                {
                    currentDistance = bestDistance;
                    factoryNumber = j;
                }
            }
        }
        factory[factoryNumber][1]--; // Remove the used factory;
        totalDistance = totalDistance + bestDistance;
    }

    return totalDistance;
}

*/
typedef struct {
    long long first;
    int second;
} Pair;

// Function to compare integers for sorting robots
int compare(const void* a, const void* b) {
    return (*(int*)a - *(int*)b);
}

// Function to compare factories by their positions
int compareFactories(const void* a, const void* b) {
    return (*(int**)a)[0] - (*(int**)b)[0];
}

// Function to find the minimum total distance
long long minimumTotalDistance(int* robot, int robotSize, int** factory, int factorySize, int* factoryColSize) {
    static const long long INF = LLONG_MAX;

    // Sort robots and factories
    qsort(robot, robotSize, sizeof(int), compare);
    qsort(factory, factorySize, sizeof(int*), compareFactories);

    // Dynamic programming array
    long long *dp = (long long *)malloc((robotSize + 1) * sizeof(long long));
    if (!dp) {
        perror("Failed to allocate memory for dp array");
        exit(EXIT_FAILURE);
    }

    // Initialize dp array
    for (int j = 0; j <= robotSize; j++) {
        dp[j] = INF; // Set all to infinity initially
    }
    dp[0] = 0; // Base case

    // Iterate over each factory
    for (int i = 0; i < factorySize; i++) {
        long long prefix = 0;
        int limit = factory[i][1]; // The number of robots the factory can handle
        Pair* dq = (Pair*)malloc((robotSize + 1) * sizeof(Pair)); // Deque-like structure
        if (!dq) {
            perror("Failed to allocate memory for deque");
            free(dp);
            exit(EXIT_FAILURE);
        }

        int front = 0, back = 0; // Indices for deque

        // Initialize the deque with the first element
        dq[back++] = (Pair){dp[0] - prefix, 0}; // Start with dp[0]

        // Sliding window for assigning robots to the current factory
        for (int j = 1; j <= robotSize; j++) {
            prefix += llabs(robot[j - 1] - factory[i][0]);

            // Maintain the deque size within the limit
            if (j - dq[front].second > limit) {
                front++; // Pop from the front if limit is reached
            }

            // Maintain the deque's properties
            while (back > front && dq[back - 1].first >= (dp[j] != INF ? dp[j] - prefix : INF)) {
                back--; // Pop from the back
            }

            // Push the current value into the deque
            dq[back++] = (Pair){(dp[j] != INF ? dp[j] - prefix : INF), j};

            // Calculate the new dp value
            dp[j] = (dq[front].first != INF ? dq[front].first + prefix : INF);
        }

        free(dq); // Free the deque-like structure
    }

    long long result = dp[robotSize];
    free(dp); // Free the dp array

    return result; // Return the final result
}