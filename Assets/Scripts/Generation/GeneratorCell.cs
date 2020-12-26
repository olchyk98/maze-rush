namespace Generation
{
    public class GeneratorCell
    {
        public readonly int x;
        public readonly int y;
        public bool borderedN { get; set; } = true;
        public bool borderedE { get; set; } = true;
        public bool borderedS { get; set; } = true;
        public bool borderedW { get; set; } = true;

        public GeneratorCell(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
}