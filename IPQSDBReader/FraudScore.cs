
namespace IPQSDBReader
{

	public class FraudScore
	{
		public int forStrictness(int strictnesslevel)
		{
			return strictness[strictnesslevel];
		}

		private Dictionary<int, int> strictness = new Dictionary<int, int>();
		public void setFraudScore(int fraudscore, int strictnesslevel)
		{
			strictness[strictnesslevel] = fraudscore;
		}
	}
}