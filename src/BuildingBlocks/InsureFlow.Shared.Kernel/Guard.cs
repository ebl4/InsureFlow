namespace InsureFlow.Shared.Kernel
{
    public static class Guard
    {
        public static void AgainstNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} must be provided and not be empty.", name);
        }

        public static void AgainstNegativeOrZero(decimal value, string name)
        {
            if (value <= 0) throw new ArgumentException($"{name} must be greater than zero.", name);
        }
    }
}
