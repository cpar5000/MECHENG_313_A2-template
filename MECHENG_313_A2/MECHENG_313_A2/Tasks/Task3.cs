using MECHENG_313_A2.Serial;
using MECHENG_313_A2.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MECHENG_313_A2.Tasks
{
    internal class Task3 : Task2
    {

        protected bool configState = false;
        private static Timer timer;


        public override void Start()
        {
            //Setting the initial Green state
            state = "Red";
            FSM.SetCurrentState(state);


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

            FSM.ProcessEvent("a");

            TimerTick();


        }

        public void TimerTick()
        {
            timer = new Timer();

            if (state == "Green")
            {
                timer.Interval = greenLength;
            } else if (state == "Red")
            {
                timer.Interval = redLength;
            } else
            {
                timer.Interval = yellowLength;
            }
           
            timer.Elapsed += Tick;
            timer.AutoReset = true;
            timer.Enabled = true;

        }

        public override async Task<bool> EnterConfigMode()
        {
            //Can only enter config mode is the current state is Red
            while (!String.Equals(FSM.GetCurrentState(), "Red"))
            {

                
                //Using ProcessEvent method to enter config mode. use of state variable is only since Process Event returns current 
            }
            configState = true;
            timer.Interval = yellowLength;
            return true;
        }

        public void Tick(Object source, ElapsedEventArgs e)
        {
            if (configState == true)
            {
                FSM.ProcessEvent("b");
                configState = false;
            } else
            {
                state = FSM.ProcessEvent("a");
            }
          

            if (state == "Green")
            {
                timer.Interval = greenLength;
            }
            else if (state == "Red")
            {
                timer.Interval = redLength;
            }
            else
            {
                timer.Interval = yellowLength;
            }


        }


        // TODO: Implement this
    }
}
