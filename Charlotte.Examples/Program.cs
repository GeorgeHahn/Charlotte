namespace Charlotte.Examples
{
    class Program
    {
        static void Main()
        {
            (new SimplePublish()).Run();
            (new SimpleSubscribe()).Run();
            (new WildcardSubscribe()).Run();
        }
    }
}
