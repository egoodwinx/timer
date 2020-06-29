/*
 * File:        MainWindow.xaml.cs
 * Project:     Skipper's Timer
 * Programmer:  Emily Goodwin
 * First Version: June 27, 2020 
 * Description: Contains the code for the main window 
 */

using System.Windows;
using System.Timers;
using System.Diagnostics;
using System.Windows.Media;
using System;
using System.Linq;
using System.IO;
using System.Configuration;

namespace SkippersTimer
{
    /// MainWindow
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private const int SECONDS_IN_MS = 1000;

        private int breakTimeMS = 180000;// 3 minutes in milliseconds 
        private int backgroundCheckMS = 60000; // every minute i guess
        private int backgroundChangeMinutes = 30;
        private  string logFile = "times.log";

        private Color firstColour = Colors.Red;
        private Color secondColour = Colors.Blue;
        private Color thirdColour = Colors.Green;
        private Color fourthColour = Colors.Purple;
        private Color fifthColour = Colors.Gold;
        private bool stopped = true;
        private Timer timerBackgroundCheck;
        private Timer timerSeconds;
        private Timer timerBreak;
        private Timer timerBreakSeconds;
        private Stopwatch stopwatch;
        private Stopwatch breakStopWatch;
        private int halfHours = 0;
        private System.Collections.Generic.List<string> times = new System.Collections.Generic.List<string>(); 

        /*
         * Method:      MainWindow
         * Description: Constructor/Initializer for the main window
         * Parameters:  void
         * Return:      NA
         */
        public MainWindow()
        {
            stopwatch = new Stopwatch();
            breakStopWatch = new Stopwatch();
            processConfigurationFile();
            setupTimer();
            InitializeComponent();
            updateListFromFile();
            timesListBx.ItemsSource = times;
            if (timesListBx.Items.Count > 0)
            {
                timesListBx.ScrollIntoView(timesListBx.Items.GetItemAt(timesListBx.Items.Count - 1));
            }
            onBackgroundUpdate_EventHandler(null, null);
        }

        /*
         * Method:      processConfigurationFile
         * Description: process the configuration file and set the values for the aplication
         * Parameters:  void
         * Return:      void
         */
        private void processConfigurationFile()
        {
            int breakSecondsConfig;
            int backgroundCheckSecondsConfig;
            int backgroundMinutesConfig;
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["logFile"]))
            {
                logFile = ConfigurationManager.AppSettings["logFile"];
            }
            if (int.TryParse(ConfigurationManager.AppSettings["breakSeconds"], out breakSecondsConfig))
            {
                breakTimeMS = breakSecondsConfig * 1000;
            }
            if (int.TryParse(ConfigurationManager.AppSettings["backgroundCheckSeconds"], out backgroundCheckSecondsConfig))
            {
                backgroundCheckMS = backgroundCheckSecondsConfig * 1000;
            }
            if (int.TryParse(ConfigurationManager.AppSettings["backgroundChangeMinutes"], out backgroundMinutesConfig))
            {
                backgroundChangeMinutes = backgroundMinutesConfig;
            }
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["backgroundFirstColour"]))
            {
                string colour = ConfigurationManager.AppSettings["backgroundFirstColour"];
                firstColour = (Color)ColorConverter.ConvertFromString(colour);
            }
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["backgroundSecondColour"]))
            {
                string colour = ConfigurationManager.AppSettings["backgroundSecondColour"];
                secondColour = (Color)ColorConverter.ConvertFromString(colour);
            }
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["backgroundThirdColour"]))
            {
                string colour = ConfigurationManager.AppSettings["backgroundThirdColour"];
                thirdColour = (Color)ColorConverter.ConvertFromString(colour);
            }
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["backgroundFourthColour"]))
            {
                string colour = ConfigurationManager.AppSettings["backgroundFourthColour"];
                fourthColour = (Color)ColorConverter.ConvertFromString(colour);
            }
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["backgroundFifthColour"]))
            {
                string colour = ConfigurationManager.AppSettings["backgroundFifthColour"];
                fifthColour = (Color)ColorConverter.ConvertFromString(colour);
            }
        }

        /*
         * Method:      setupTimer
         * Description: setup timers
         * Parameters:  void
         * Return:      void
         */
        private void setupTimer()
        {
            timerBackgroundCheck = new Timer(backgroundCheckMS);
            timerBackgroundCheck.Enabled = true;
            timerBackgroundCheck.AutoReset = true;
            timerBackgroundCheck.Elapsed += onBackgroundUpdate_EventHandler;

            timerSeconds = new Timer(SECONDS_IN_MS);
            timerSeconds.Enabled = true;
            timerSeconds.AutoReset = true;
            timerSeconds.Elapsed += onSecond_eventHandler;

            timerBreak = new Timer(breakTimeMS);
            timerBreak.Enabled = false;
            timerBreak.AutoReset = false;
            timerBreak.Elapsed += break_EventHandler;

            timerBreakSeconds = new Timer(SECONDS_IN_MS);
            timerBreakSeconds.Enabled = false;
            timerBreakSeconds.AutoReset = true;
            timerBreakSeconds.Elapsed += onSecondBreak_EventHandler;
        }

        /*
         * Method:      break_EventHandler
         * Description: handle the break event, if the break time runs out reset the timers
         * Parameters:  
         *  object source - who called the event
         *  ElapsedEventArgs e - the events
         * Return:      void
         */
        private void break_EventHandler(object source, ElapsedEventArgs e)
        {
            stopped = true;
            stopTimer();
        }

        /*
         * Method:      onSecondBreak_EventHandler
         * Description: handle updating the break counter to count down
         * Parameters:  
         *  object source - who called the event
         *  ElapsedEventArgs e - the events
         * Return:      void
         */
        private void onSecondBreak_EventHandler(object source, ElapsedEventArgs e)
        {
            if ((breakStopWatch.Elapsed != null) && (stopped))
            {
                TimeSpan breakTime = new TimeSpan(0,0,0,0,breakTimeMS);
                this.Dispatcher.Invoke(() =>
                {
                    startStopBtn.Content = (breakTime - breakStopWatch.Elapsed).ToString("hh\\:mm\\:ss");
                });
            }
        }

        /*
         * Method:      onBackgroundUpdate_EventHandler
         * Description: check the time to see if we need to change the background colour
         * Parameters:  
         *  object source - who called the event
         *  ElapsedEventArgs e - the events
         * Return:      void
         */
        private void onBackgroundUpdate_EventHandler(object source, ElapsedEventArgs e)
        {
            halfHours = (int)(((stopwatch.ElapsedMilliseconds / 1000) / 60) / backgroundChangeMinutes); // divided by ms to seconds, divded by 60 for minutes then every 30 minutes 
            if (halfHours == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Background = new SolidColorBrush(firstColour);
                });
            }
            else if (halfHours == 1)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Background = new SolidColorBrush(secondColour);
                });
            }
            else if (halfHours == 2)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Background = new SolidColorBrush(thirdColour);
                });
            }
            else if (halfHours == 3)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Background = new SolidColorBrush(fourthColour);
                });
            }
            else if (halfHours >= 4)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Background = new SolidColorBrush(fifthColour);
                });
            }
        }

        /*
         * Method:      onSecond_eventHandler
         * Description: update timer counter
         * Parameters:  
         *  object source - who called the event
         *  ElapsedEventArgs e - the event args
         * Return:      void
         */
        private void onSecond_eventHandler(object source, ElapsedEventArgs e)
        {
            if (stopwatch.Elapsed != null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    currentTimeLbl.Content = stopwatch.Elapsed.ToString("hh\\:mm\\:ss");
                });
            }
        }

        /*
         * Method:      startStopBtn_Click
         * Description: Handle the start/stop button being pressed. If stop is pressed start the break counter.
         * Parameters:  
         *  object source - who called the event
         *  RoutedEventArgs e - the event args
         * Return:      void
         */
        private void startStopBtn_Click(object sender, RoutedEventArgs e)
        {
            stopped = !stopped;
            if (stopped)
            {
                timerBreakSeconds.Enabled = true;
                timerBreak.Enabled = true;
                stopwatch.Stop();                
                breakStopWatch.Start();
            }
            else
            {
                timerBackgroundCheck.Start();
                breakStopWatch.Reset();
                timerBreak.Stop();
                timerBreakSeconds.Stop();
                stopwatch.Start();
                startStopBtn.Content = "Stop";
            }
        }

        /*
         * Method:      resetBtn_Click
         * Description: handle the reset button being clicked, reset the timers
         * Parameters:  
         *  object source - who called the event
         *  RoutedEventArgs e - the event args
         * Return:      void
         */
        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {                
            timerBreakSeconds.Enabled = false;
            timerBreak.Enabled = false;
            if (stopped)
            {
                stopTimer();
                timerBackgroundCheck.Stop(); 

            }
            else
            {
                if (stopwatch.ElapsedMilliseconds > 0)
                {
                    writeLog(DateTime.Now.ToString() + "\t " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss"));
                    updateListLastLine();
                }
                restartTimers();
                stopwatch.Restart();
               
                timerBackgroundCheck.Start();
                this.Dispatcher.Invoke(() =>
                {
                    startStopBtn.Content = "Stop";
                });
            }
        }

        /*
         * Method:      restartTimers
         * Description: Restart the timeres
         * Parameters:  void
         * Return:      void
         */
        private void restartTimers()
        {
            this.Dispatcher.Invoke(() =>
            {
                currentTimeLbl.Content = "00:00:00";
                this.Background = new SolidColorBrush(firstColour);
            });
            timerBackgroundCheck.Stop();
            timerBreakSeconds.Stop();
            timerBreak.Stop();
            timerBreak.Enabled = false;
            timerBreakSeconds.Enabled = false;
            breakStopWatch.Reset();
            halfHours = 0;
        }

        /*
         * Method:      stopTimer
         * Description: Stop the timer from coutning down
         * Parameters:  void
         * Return:      void
         */
        private void stopTimer()
        {
            if (stopwatch.ElapsedMilliseconds > 0)
            {
                writeLog(DateTime.Now.ToString() + "\t " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss"));
                updateListLastLine();
            }
            restartTimers();
            stopwatch.Reset();
            this.Dispatcher.Invoke(() =>
            {
                startStopBtn.Content = "Start";
            });
        }

        /*
         * Method:      writeLog
         * Description: write to the log
         * Parameters:  string text - the text to append to the log, will add a \n after the text
         * Return:      void
         */
        private void writeLog(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                using(StreamWriter writer = new StreamWriter(logFile, true))
                {
                    writer.WriteLine(text);
                }
            }
        }

        /*
         * Method:      readLogFile
         * Description: read the entire log file
         * Parameters:  void
         * Return:      void
         */
        private string readLogFile()
        {
            if (File.Exists(logFile))
            {
                return File.ReadAllText(logFile);
            }
            return "";
        }

        /*
         * Method:      readLogLastLineFile
         * Description: read the last line of the log file
         * Parameters:  void
         * Return:      string - the text read
         */
        private string readLogLastLineFile()
        {
            if (File.Exists(logFile))
            {
                return File.ReadLines(logFile).Last();
            }
            return "";
        }

        /*
         * Method:      updateListFromFile
         * Description: update the list of times from the log file
         * Parameters:  void
         * Return:      void
         */
        private void updateListFromFile()
        {
            // split the entries into an array, ignore newlines and empty space
            string[] lines = readLogFile().Split(new[] { "\r\n","r","n" }, StringSplitOptions.RemoveEmptyEntries);
            times.AddRange(lines);
            this.Dispatcher.Invoke(() =>
            {
                timesListBx.Items.Refresh();
            });
        }

        /*
         * Method:      updateListLastLine
         * Description: update the list with the last line added to the log file
         * Parameters:  void
         * Return:      void
         */
        private void updateListLastLine()
        {
            times.Add(readLogLastLineFile());
            this.Dispatcher.Invoke(() =>
            {
                timesListBx.Items.Refresh();
            });
        }

        /*
         * Method:      window_Closing
         * Description: save when window closing
         * Parameters:           
         *  object source - who called the event
         *  RoutedEventArgs e - the event args
         * Return:      void
         */
        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds > 0)
            {
                writeLog(DateTime.Now.ToString() + "\t " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss"));
            }
        }
    }
}
