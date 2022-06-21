namespace IPQSDBReader
{
	public class AbuseVelocity
	{
		public int Raw;

		public override string ToString()
		{
			switch (this.Raw)
			{
				case 1:
					return "low";
				case 2:
					return "medium";
				case 3:
					return "high";
				default:
					return "none";
			}
		}

		public static AbuseVelocity create(byte b)
		{
			AbuseVelocity av = new AbuseVelocity();

			Bitmask data = Bitmask.Create(b);
			if (data.Has(Bitmask.AbuseVelocityTwo))
			{
				if (data.Has(Bitmask.AbuseVelocityOne))
				{
					av.Raw = 3;
					return av;
				}

				av.Raw = 1;
				return av;
			}

			if (data.Has(Bitmask.AbuseVelocityOne))
			{
				av.Raw = 2;
				return av;
			}

			av.Raw = 0;
			return av;
		}
	}

}