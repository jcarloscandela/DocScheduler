namespace DocScheduler.Application
{
    public class SlotNotFoundException : Exception
    {
        public SlotNotFoundException(string message) : base(message)
        {
        }
    }
}
