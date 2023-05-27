using MECHENG_313_A2.Serial;
using MECHENG_313_A2.Views;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using System.Linq;

namespace MECHENG_313_A2.Tasks
{
    internal class Task2 : IController
    {

        protected string state = null;
        private string tempState = null;
        protected FiniteStateMachine FSM = new FiniteStateMachine();
        protected MockSerialInterface serialInterface = new MockSerialInterface();
        TrafficLightState displayState = new TrafficLightState();
        protected object lockObject = new object();
        public virtual TaskNumber TaskNumber => TaskNumber.Task2;
        protected Boolean configMode = false;

        protected static int DEFAULT_GREEN_LIGHT_LENGTH = 1000;
        protected static int DEFAULT_YELLOW_LIGHT_LENGTH = 1000;
        protected static int DEFAULT_RED_LIGHT_LENGTH = 1000;
        protected static int DEFAULT_CONFIG_LIGHT_LENGTH = 1000;
        protected int redLength = DEFAULT_RED_LIGHT_LENGTH;
        protected int greenLength = DEFAULT_GREEN_LIGHT_LENGTH;
        protected int yellowLength = DEFAULT_YELLOW_LIGHT_LENGTH;
        protected int configLength = DEFAULT_CONFIG_LIGHT_LENGTH;


        protected ITaskPage _taskPage;

        virtual public void ConfigLightLength(int redLength, int greenLength)
        {


        }

        public virtual async Task<bool> EnterConfigMode()
        {
            //Can only enter config mode is the current state is Red
            if (String.Equals(FSM.GetCurrentState(), "Red"))
            {
                //Using ProcessEvent method to enter config mode. use of state variable is only since Process Event returns current state
                state = FSM.ProcessEvent("b");

                configMode = true;
                return true;
            }
            return false;
        }

        virtual public void ExitConfigMode()
        {
            //Using ProcessEvent method to exit config mode. use of state variable is only since Process Event returns current state
            state = FSM.ProcessEvent("b");
            configMode = false;
        }

        public async Task<string[]> GetPortNames()
        {
            //Using the MockSerialInterface class to obtain port names (.Result converts from Task <string[]> to string[])
            return serialInterface.GetPortNames().Result;
        }

        public async Task<string> OpenLogFile()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log.txt");

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            else
            {
                string[] lines = File.ReadAllLines(filePath);
                Array.Reverse(lines);
                _taskPage.SetLogEntries(lines);
            }



            // Help notes: to read a file named "log.txt" under the LocalApplicationData directory,
            // you may use the following code snippet:
            // string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log.txt");
            // string text = File.ReadAllText(filePath);
            //
            // You can also create/write to file(s) through System.IO.File. 
            // See https://learn.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/data/files?tabs=windows, and
            // https://learn.microsoft.com/en-us/dotnet/api/system.io.file?view=netstandard-2.0 for more details.
            return filePath;
        }

        public async Task<bool> OpenPort(string serialPort, int baudRate)
        {
            bool open = await serialInterface.OpenPort(serialPort, baudRate).ConfigureAwait(false);
            if (open)
            {
                return true;
            }
            return false;
        }

        public void RegisterTaskPage(ITaskPage taskPage)
        {
            _taskPage = taskPage;
        }

       

        public async virtual void Start()
        {
            //Setting the initial Green state
            state = "Green";
            FSM.SetCurrentState(state);
            MethodA(DateTime.Now, "a");
            string logEntry = DateTime.Now + "\t" + "Event Triggered:\tStart\n";
            logger(logEntry);
            _taskPage.SetTrafficLightState(TrafficLightState.Green);


            //Setting the Finite State Table
            FSM.SetNextState("Green", "Yellow", "a");
            FSM.AddAction("Green", "a", MethodA);
            FSM.AddAction("Green", "a", MethodB);
            FSM.AddAction("Green", "a", MethodC);

            FSM.SetNextState("Yellow", "Red", "a");
            FSM.AddAction("Yellow", "a", MethodA);
            FSM.AddAction("Yellow", "a", MethodB);
            FSM.AddAction("Yellow", "a", MethodC);

            FSM.SetNextState("Red", "Green", "a");
            FSM.AddAction("Red", "a", MethodA);
            FSM.AddAction("Red", "a", MethodB);
            FSM.AddAction("Red", "a", MethodC);

            FSM.SetNextState("Red", "Yellow'", "b");
            FSM.AddAction("Red", "b", MethodA);
            FSM.AddAction("Red", "b", MethodB);
            FSM.AddAction("Red", "b", MethodC);

            FSM.SetNextState("Yellow'", "Red", "b");
            FSM.AddAction("Yellow'", "b", MethodA);
            FSM.AddAction("Yellow'", "b", MethodB);
            FSM.AddAction("Yellow'", "b", MethodC);

            FSM.SetNextState("None", "Red", "b");
            FSM.AddAction("None", "b", MethodA);
            FSM.AddAction("None", "b", MethodB);
            FSM.AddAction("None","b", MethodC);

            FSM.SetNextState("Yellow'", "None", "a");
            FSM.AddAction("Yellow'", "a", MethodA);
            FSM.AddAction("Yellow'", "a", MethodB);
            FSM.AddAction("Yellow'", "a", MethodC);

            FSM.SetNextState("None", "Yellow'", "a");
            FSM.AddAction("None", "a", MethodA);
            FSM.AddAction("None", "a", MethodB);
            FSM.AddAction("None", "a", MethodC);
        }

        public virtual void Tick()
        {
            state = FSM.ProcessEvent("a");

        }

        public async void MethodA(DateTime timestamp, string eventTrigger)
        {
            string temp = FSM.GetCurrentState();
            //Since Yellow' is the Yellow in config state the string must be simplified to "Yellow"
            if (temp == "Yellow'")
            {
                temp = "Yellow";
            }

            //Checking that the currentState is valid
            if (Enum.TryParse<TrafficLightState>(temp, out displayState))
            {
                //Sending the serial command to change the Traffic light state and then printing to the gui
                string response = await serialInterface.SetState((TrafficLightState)Enum.Parse(typeof(TrafficLightState), temp, true));
                _taskPage.SerialPrint(timestamp, response + "\n");
            }
        }

        public async void MethodB(DateTime timestamp, string eventTrigger)
        {
            string eventTriggered;
            if (eventTrigger == "a")
            {
                eventTriggered = "Tick";
            } else if (configMode == false)
            {
                eventTriggered = "Entered Config Mode";
            } else
            {
                eventTriggered = "Exited Config Mode";
            }

            string logEntry = timestamp + "\tEvent Triggered: " + eventTriggered + "\n";
            logger(logEntry);

            logEntry = timestamp + "\tEntered State: " + FSM.GetCurrentState() + "\n";
            logger(logEntry);
        }

        public async void MethodC(DateTime timestamp, string eventTrigger)
        {
            string temp = FSM.GetCurrentState();
            if (temp == "Yellow'")
            {
                temp = "Yellow";
            }

            string logEntry = timestamp + "\tAction Started: Update GUI Light: Updating the GUI Traffic Light to " + temp + "\n";
            logger(logEntry);

            if (Enum.TryParse<TrafficLightState>(temp, out displayState))
            {
                _taskPage.SetTrafficLightState((TrafficLightState)Enum.Parse(typeof(TrafficLightState), temp, true));
            }

            logEntry = timestamp + "\tAction Started: Updated GUI Light: The GUI Traffic Light is now " + FSM.GetCurrentState() + "\n";
            logger(logEntry);
        }

        public void logger(string logEntry)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log.txt");
            lock (lockObject) {
                File.AppendAllText(filePath, logEntry);
                _taskPage.AddLogEntry(logEntry);
            }

        }
    }
}
