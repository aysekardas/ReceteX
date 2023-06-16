using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceteX.Utility
{
	public class XmlRetriever
	{

		//içine çekmek istediğimiz url'yi verdik;
		public async Task<string> GetXmlContent(string url)
		{
			//client dışarıdan bir şeye bağlanıp veri çekiyor, sunucu olabilir
			using (var client = new HttpClient())
			{
				//client sadece burada sınırlanmış oluyor bu şekilde yazınca
				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();
				
				return await response.Content.ReadAsStringAsync();

			}
		}
	}
}
