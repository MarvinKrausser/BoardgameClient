namespace communication
{
    public class PlayerState
    {
        public string playerName { get; set; }
        public int [] currentPosition { get; set; }
        public int [] spawnPosition { get; set; }
        public Direction direction { get; set; } 
        public Character character { get; set; } 
        public int lives { get; set; }
        public int lembasCount { get; set; }
        public int suspended { get; set; }
        public int reachedCheckpoints { get; set; }
        public Card [] playedCards { get; set; } 
        public int turnOrder { get; set; }
    }
}