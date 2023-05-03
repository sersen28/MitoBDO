namespace MitoBDO.Model
{
	public class Node
	{
		public string? Name { get; set; }
		public string? Day { get; set; }
		public int? Num { get; set; }
		public string? Stage { get; set; }
		public string? Nation { get; set; }

		public Node(string[] data)
		{
			if (data is null || data.Length < 5) return;		

			this.Name = data[0];
			this.Day = data[1];
			this.Stage = data[3];
			this.Nation = data[4];

			var num = 0;
			var check = int.TryParse(data[2], out num);
			if (check)
			{
				this.Num = num;
			}
		}
	}
}
