namespace DumpTruck.HelperClasses
{
    public static class Constants
    {
        // Problem constants
        public const int DUMP_ID = 287;
        public const double MAX_DAY_TIME = 43_200D;
        public const double MAX_TRUCK_VOLUME = 100_000D;
        public const int MAX_ORDER_COUNT = 1_177;
        public const int
            MAX_ROUTE_DURATION = 41_400; //690 = 12 hours in minutes minus 30 minutes for emptying in the end
        
        // Penalty constants
        public const double DURATION_PENALTY_MULTIPLIER = 1;
        public const double CAPACITY_PENALTY_MULTIPLIER = 25;
        public const double SATISFACTION_PENALTY_MULTIPLIER = 1.3;

        // Simulated Annealing constants
        public const int ITER_Q = 2_000_000;
        public const int INITIAL_TEMPERATURE = 27_000;
        public const double ALPHA = 0.995;
        public const int SIM_ANNEAL_RUNS = 2;
        public const int APPLIES_TEMP_DOWN = 3_000;
    }
}