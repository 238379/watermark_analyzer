using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public class InternetTools
	{
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static Task WaitForInternetConnection(TimeSpan interval)
        {
            return Task.Run(async () =>
            {
                while(!CheckForInternetConnection())
                {
                    await Task.Delay(interval);
                }
            });
        }
    }
}
