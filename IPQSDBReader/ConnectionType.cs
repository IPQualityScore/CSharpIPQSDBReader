namespace IPQSDBReader
{
	public class ConnectionType
	{

		public int Raw;

		public override string ToString()
		{
			switch (this.Raw)
			{
				case 1:
					return "Residential";
				case 2:
					return "Mobile";
				case 3:
					return "Corporate";
				case 4:
					return "Data Center";
				case 5:
					return "Education";
				default:
					return "Unknown";
			}
		}

		public static ConnectionType create(byte b)
		{
			ConnectionType ct = new ConnectionType();

			Bitmask data = Bitmask.Create(b);
			if (data.Has(Bitmask.ConnectionTypeThree))
			{
				if (data.Has(Bitmask.ConnectionTypeTwo))
				{
					ct.Raw = 3;
					return ct;
				}

				if (data.Has(Bitmask.ConnectionTypeOne))
				{
					ct.Raw = 5;
					return ct;
				}

				ct.Raw = 1;
				return ct;
			}

			if (data.Has(Bitmask.ConnectionTypeTwo))
			{
				ct.Raw = 2;
				return ct;
			}

			if (data.Has(Bitmask.ConnectionTypeOne))
			{
				ct.Raw = 4;
				return ct;
			}

			ct.Raw = 0;
			return ct;
		}
	}

}