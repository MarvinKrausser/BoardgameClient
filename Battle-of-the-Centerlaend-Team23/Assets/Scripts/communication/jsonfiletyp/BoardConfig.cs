using System.Collections.Generic;

namespace communication
{
    public class BoardConfig
    {
        public string name { get; set; }
        public int height { get; set; }
        public int width { get; set; } 
        public List<FieldsAndDirection> startFields { get; set; }
        public int [,] checkPoints { get; set; }
        public FieldsAndDirection eye { get; set; }
        public int [,] holes { get; set; }
        public List<FieldsAndDirection> riverFields { get; set; }
        public int [,,] walls { get; set; }
        public List<LembasFields> lembasFields { get; set; }
        
        public int [,] eagleFields { get; set; }
    }
    public class FieldsAndDirection
    {
        public int [] position { get; set; }
        public Direction direction { get; set; }
    }
}
