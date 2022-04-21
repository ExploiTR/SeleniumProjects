using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Fast.com
{
    internal class Program
    {
        static long timeCount;
        static string format;
        static FileStream stream;

        static bool headless = true;

        static void Main(string[] args)
        {
            long timeIn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (args.Count() != 0)
            {
                timeCount = long.Parse(args[0].Trim());
                format = args[1].Trim();

                switch (format)
                {
                    case "h":
                        {
                            timeIn += (timeCount * 3600);
                            break;
                        }
                    case "m":
                        {
                            timeIn += (timeCount * 60);
                            break;
                        }
                    default:
                        {
                            timeIn += timeCount;
                            break;
                        }
                }
            }
            else
            {
                timeCount = 6;
                format = "h";
                timeIn += 6 * 3600 * 1000;
            }

            Console.WriteLine("ETA : " + timeCount + format + " ( " + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + " / " + timeIn + " )");

            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.EnableVerboseLogging = false;
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--log-level=3");
            options.AddArgument("disable-logging");
            if (headless)
                options.AddArguments("--headless");
            options.PageLoadStrategy = PageLoadStrategy.Eager;

            IWebDriver driver = new ChromeDriver(service, options);
            try
            {
                driver.Navigate().GoToUrl("https://fast.com/");
                if (headless)
                    Console.WriteLine("Opening url");
            }
            catch (Exception)
            {
                if (headless)
                    Console.WriteLine("Url err, restarting...");
                Thread.Sleep(300000);
                driver.Quit();
                Thread.CurrentThread.Interrupt();
                Main(args);
            }

            IWebElement element = driver.FindElement(By.Id("show-more-details-link"));

            while (!element.Displayed)
            {
                if (headless)
                {
                    Console.WriteLine("Wait for download mode end...");
                    Thread.Sleep(500);
                }
                //wait
            }

            if (element != null)
                element.Click();

            if (headless)
                Console.WriteLine("Waiting for upload to start...");
            Thread.Sleep(5000); //wait for start upload

            createFile();

            if (headless)
                Console.WriteLine("Press Ctrl-C to exit!");

            for (; ; )
            {
                if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() > timeIn)
                {
                    if (headless)
                        Console.WriteLine("Timeout!");
                    break;
                }
                while (reloadButton(driver) == null)
                {
                    if (headless)
                    {
                        Console.WriteLine("Waiting for reload button...");
                        Thread.Sleep(500);
                    }
                    //wait
                }
                IWebElement reload = reloadButton(driver);
                if (reload.Displayed)
                {
                    double stat = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / timeIn;

                    long diff = (timeIn - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / 1000;
                    long day = diff / 86400;
                    diff = diff - day * 86400;
                    long hour = diff / 3600;
                    diff = diff - hour * 3600;
                    long minute = diff / 60;
                    long sec = diff - diff * 60;

                    Console.WriteLine("ETA : " + day + "d " + hour + "h " + minute + "m " + sec + "s " +
                        " ( " + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + " / " + timeIn + " - " + stat + "%" + " )");
                    writeFile(driver);

                    for (int i = 0; i <= 30; i++)
                    {
                        Console.WriteLine("Waiting " + i + "/30 " + "s");
                        Thread.Sleep(1000);
                    }

                    if (headless)
                        Console.WriteLine("Next test started...");
                    reload.Click();
                }
                /*  else
                  {
                      if (networkErr(driver))
                      {
                          Thread.Sleep(300000);
                          driver.Quit();
                          Thread.CurrentThread.Interrupt();
                          Main(args);
                      }
                  }*/
            }

            stream.Close();
            driver.Quit();
        }

        private static void createFile()
        {
            if (stream == null)
            {
                stream = new FileStream("C:\\Users\\ExploiTR\\Desktop\\Record.txt", FileMode.Append, FileAccess.Write);
            }
        }

        private static void writeFile(IWebDriver driver)
        {
            string dlVal = downloadSpeed(driver);
            string ulVal = uploadSpeed(driver);


            if (!stream.CanWrite)
            {
                stream = new FileStream("C:\\Users\\ExploiTR\\Desktop\\Record.txt", FileMode.Append, FileAccess.Write);
            }

            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(dlVal + " , " + ulVal + " ," + " " + DateTime.Now.ToString());
            sw.Flush();
            sw.Close();

        }

        private static IWebElement reloadButton(IWebDriver driver)
        {
            try
            {
                return driver.FindElement(By.XPath("//span[contains(@class,'oc-icon-refresh')]"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string downloadSpeed(IWebDriver driver)
        {
            try
            {
                return driver.FindElement(By.Id("speed-value")).Text;
            }
            catch (Exception)
            {
                return "FAIL";
            }
        }

        private static string uploadSpeed(IWebDriver driver)
        {
            try
            {
                return driver.FindElement(By.Id("upload-value")).Text;
            }
            catch (Exception)
            {
                return "FAIL";
            }
        }

        private static bool networkErr(IWebDriver driver)
        {
            IWebElement element = driver.FindElement(By.XPath("//div[@loc-str='no_connection']"));
            return element.Displayed;
        }
    }
}
