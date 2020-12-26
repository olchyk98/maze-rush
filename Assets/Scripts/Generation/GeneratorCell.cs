namespace Generation
{
    public class GeneratorCell
    {
        public readonly int X;
        public readonly int Y;
        public bool borderedN { get; set; } = true;
        public bool borderedE { get; set; } = true;
        public bool borderedS { get; set; } = true;
        public bool borderedW { get; set; } = true;

        public GeneratorCell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}