namespace AutoMockHelper.SampleLogic.OrderProcessor
{
	public class TestOrderNumberGeneratorService : IOrderNumberGeneratorService
	{
	    public const int DefaultSeed = 999;

        public int GetNextOrderNumber()
        {
            return DefaultSeed + 1;
        }
    }
}