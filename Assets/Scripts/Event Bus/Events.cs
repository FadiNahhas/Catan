using Board;
using Building;
using Space_Station_Tycoon.Scripts.Event_System;

namespace Event_Bus
{
    public interface IEvent { }
    
    public struct DiceRolledEvent : IEvent
    {
        public int Result { get; }

        public DiceRolledEvent(int result)
        {
            Result = result;
            
            EventBus<GameLogEvent>.Raise(new GameLogEvent(ToString()));
        }
        
        public override string ToString()
        {
            return $"Player {BuildManager.Instance.currentPlayer} rolled: {Result}";
        }
    }

    public struct BuildEvent : IEvent
    {
        public BuildingType Type { get; }
        
        public int PlayerID { get; }
        
        public BuildEvent(BuildingType type, int playerID)
        {
            Type = type;
            PlayerID = playerID;
            
            EventBus<GameLogEvent>.Raise(new GameLogEvent(ToString()));
        }
        
        // TODO: Get player name from player manager and convert building type to string
        public override string ToString()
        {
            return $"Player {PlayerID} has placed a {Type.ToString()}";
        }
    }
    
    public struct GameLogEvent : IEvent
    {
        public string Message { get; }

        public GameLogEvent(string message)
        {
            Message = message;
        }
    }
}