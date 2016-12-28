using System;
using System.Net; //Import this to use WebRequest
using System.IO;  //And this to use StreamReader
using Newtonsoft.Json; //And this to use JSON
using System.Windows.Forms; //To use MessageBox

namespace GetThreadReplies
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader fileReader = null;
            string url = "";
            const int limit = 495;

            try
            {
                fileReader = new StreamReader("url.txt");
                url = fileReader.ReadToEnd();
            } catch (IOException e)
            {
                Console.WriteLine("Could not find file url.txt\nDefaulting to manual url entry");
                Console.WriteLine("Enter url of thread to grab replies:");
                url = Console.ReadLine();
                
            }


            string jsonReq;
            if (!url.EndsWith(".json"))
            {
                jsonReq = "http://a.4cdn.org/a/thread/" + extractThreadNumber(url) + ".json";
            } else
            {
                jsonReq = url;
            }
            while (true)
            {
                WebRequest webrequest = WebRequest.Create(jsonReq);
                WebResponse response = webrequest.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                String txtResponse = sr.ReadToEnd();
                dynamic jsonInformation = JsonConvert.DeserializeObject(txtResponse);
                int replies = jsonInformation.posts[0].replies;
                if (replies >= limit)
                {
                    displayMessage("Replies have reached limit", "Thread Replies Watcher");
                    break;
                }
                else
                {
                    Console.WriteLine(replies);
                    Console.WriteLine("Limit not reached yet");
                    //Console.ReadKey();
                    System.Threading.Thread.Sleep(10000);
                }
            }
        }

        private static string extractThreadNumber(string url)
        {
            string threadURL = url;
            char[] charArray = threadURL.ToCharArray();
            if (charArray[threadURL.Length-1] == '/')
            {
                threadURL = threadURL.TrimEnd('/');
            }
            return threadURL.Substring(threadURL.LastIndexOf('/'));
        }

        private static void displayMessage(string message, String windowCaption)
        {
            MessageBox.Show(new Form { TopMost = true }, message, windowCaption);
        }
    }
}
