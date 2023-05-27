using MECHENG_313_A2.Serial;
using MECHENG_313_A2.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;

namespace MECHENG_313_A2.Tasks
{
    internal class Task3 : Task2
    {

        protected int configState = 0;
        private static Timer timer;
        public override TaskNumber TaskNumber => TaskNumber.Task3;


        public override void Start()
        {
            //Setting the initial Green state
            state = "Green";
            FSM.SetCurrentState(state);
            MethodA(DateTime.Now, "a");
            string logEntry = DateTime.Now + "\t" + "Event Triggered:\tStart";
            logger(logEntry);
            _taskPage.SetTrafficLightState(TrafficLightState.Green);


            //Setting all other states
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
            FSM.AddAction("None", "b", MethodC);

            FSM.SetNextState("Yellow'", "None", "a");
            FSM.AddAction("Yellow'", "a", MethodA);
            FSM.AddAction("Yellow'", "a", MethodB);
            FSM.AddAction("Yellow'", "a", MethodC);

            FSM.SetNextState("None", "Yellow'", "a");
            FSM.AddAction("None", "a", MethodA);
            FSM.AddAction("None", "a", MethodB);
            FSM.AddAction("None", "a", MethodC);

            TimerTick();
        }

        public void TimerTick()
        {
            timer = new Timer();

            timer.Interval = greenLength;
           
            timer.Elapsed += Tick;
            timer.AutoReset = true;
            timer.Enabled = true;

        }

        public override void ConfigLightLength(int redLength, int greenLength)
        {

            this.redLength = redLength;
            this.greenLength = greenLength;

        }

        public override async Task<bool> EnterConfigMode()
        {
            //Can only enter config mode is the current state is Red
            while (!String.Equals(FSM.GetCurrentState(), "Red"))
            {

                
                //Using ProcessEvent method to enter config mode. use of state variable is only since Process Event returns current 
            }
            configState = 1;
            while (String.Equals(FSM.GetCurrentState(), "Red"))
            {

            }
            return true;
        }

        public override void ExitConfigMode()
        {

            configState = 2;

        }


        public void Tick(Object source, ElapsedEventArgs e)
        {
            if (configState == 1)
            {
                timer.Interval = configLength;
                FSM.ProcessEvent("b");
                configState = 0;
            }
            else if (configState == 2)
            {
                timer.Interval = redLength;
                FSM.ProcessEvent("b");
                configState = 0;
            }
            else
            {
                state = FSM.ProcessEvent("a");

                if (state == "Green")
                {
                    timer.Interval = greenLength;
                }
                else if (state == "Red")
                {
                    timer.Interval = redLength;
                }
                else if (state == "Yellow")
                {
                    timer.Interval = yellowLength;
                }
                else
                {
                    timer.Interval = configLength;
                }
            }
          



        }
    }
}
