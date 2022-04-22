using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace Ookla
{
    internal class Program
    {
        private static FileStream stream;
        static void Main(string[] args)
        {
            try
            {
                int waitSec = 0;

                IWebDriver driver = initDriver();
                driver.Navigate().GoToUrl("https://www.speedtest.net/");

                //IWebElement waitForServer = driver.FindElement(By.XPath("//div[@data-view-name='currentServer'
                //and @data-view-cid='view25']//div[@class='result-label']"));

                while (!serverDisp(driver))
                {
                    Console.WriteLine("Waiting for server list for " + waitSec++ + "s...");
                    Thread.Sleep(1000);
                }

                IWebElement changeServerButton = driver.FindElement(By.XPath("//div[@data-view-cid='view25']//a[@class='btn-server-select']"));
                changeServerButton.Click();

                Thread.Sleep(1000);

                driver.SwitchTo().ActiveElement();

                Thread.Sleep(500);

                IWebElement serverInput = driver.FindElement(By.XPath("//input[@id='host-search']"));
                serverInput.SendKeys("Indinet");

                Thread.Sleep(1000);

                ReadOnlyCollection<IWebElement> servers = driver.FindElements(By.XPath("//ul[@data-view-cid='view36']//li"));
                foreach (IWebElement server in servers)
                {
                    string host = server.FindElement(By.XPath("//span[@class='host-sponsor']")).Text;
                    Console.WriteLine(host);
                }

                servers[0].Click();

                Thread.Sleep(1000);

                driver.SwitchTo().ActiveElement();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static IWebDriver initDriver()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.EnableVerboseLogging = false;
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            Console.WriteLine("Press H for Headless mode.");

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--log-level=3");
            options.AddArgument("disable-logging");
            options.PageLoadStrategy = PageLoadStrategy.Eager;

            if (Console.ReadKey().Key == ConsoleKey.H)
            {
                options.AddArguments("--headless");
                Console.WriteLine("Running headless mode.");
            }
            else
            {
                Console.WriteLine("Running verbose mode.");
            }

            Console.WriteLine("Press S to save records.");

            if (Console.ReadKey().Key == ConsoleKey.S)
            {
                createFile();
                Console.WriteLine("Saving records!");
            }
            else
            {
                Console.WriteLine("Records are discarded.");
            }
            return new ChromeDriver(service, options);
        }

        private void keepTesting(IWebDriver driver)
        {
            int wait = 0;
            driver.FindElement(By.XPath("//a[starts-with(@class,'js-start-test')]")).Click();
            while (testRunning(driver))
            {
                //wait
                Thread.Sleep(1000);
                Console.WriteLine("Waiting for current test to complete for" + wait++ + "s...");
            }
            writeFile(driver);
            Thread.Sleep(500);
            Console.WriteLine("Writing to file...");
            Thread.Sleep(500);
            keepTesting(driver);
        }

        private void writeFile(IWebDriver driver)
        {
            string dl_val = driver.FindElement(By.XPath("//span[contains(@class,'download-speed')]")).Text;
            string ul_val = driver.FindElement(By.XPath("//span[contains(@class,'upload-speed')]")).Text;
            string lat_val = driver.FindElement(By.XPath("//span[contains(@class,'ping-speed')]")).Text;

            if (!stream.CanWrite)
            {
                stream = new FileStream("C:\\Users\\ExploiTR\\Desktop\\Record.txt", FileMode.Append, FileAccess.Write);
            }

            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(lat_val + " , " + dl_val + " , " + ul_val + " ," + " " + DateTime.Now.ToString());
            sw.Flush();
            sw.Close();
        }

        private static void createFile()
        {
            if (stream == null)
            {
                stream = new FileStream("C:\\Users\\ExploiTR\\Desktop\\Record.txt", FileMode.Append, FileAccess.Write);
            }
        }

        private static bool testRunning(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.XPath("//div[@class='gauge-speed-needle' and contains(@style,'opacity: 1')]"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool serverDisp(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.XPath("//div[@data-view-name='currentServer' and @data-view-cid='view25']//a[@class='hostUrl']"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
