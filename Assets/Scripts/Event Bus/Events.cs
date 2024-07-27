namespace Event_Bus
{
    public interface IEvent { }
    
    public struct DiceRolledEvent : IEvent
    {
        public int Result { get; }

        public DiceRolledEvent(int result)
        {
            Result = result;
        }
    }
}