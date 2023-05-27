using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MECHENG_313_A2.Tasks
{
    public class FiniteStateMachine : IFiniteStateMachine
    {
        //Info struct is used to store the next state and the actions associated with a state transition as a value in the nested dictionary Finite State Table
        public struct Info {
            public string nextState;
            public List<TimestampedAction> actions;
        }

        //Defining the Finite State Table using nested dictionaries
        public Dictionary<string, Dictionary<string, Info>> fst = new Dictionary<string, Dictionary<string, Info>>();
        private string currentState;

        public void AddAction(string state, string eventTrigger, TimestampedAction action)
        {
            //The Info struct stored at the key values in the Finite State Table must be copied, edited then overwritten in order to update the struct
            Info a = fst[state][eventTrigger];
            Info b;
            b = a;
            b.actions.Add(action);
            fst[state][eventTrigger] = b;

        }

        public string GetCurrentState()
        {
            return currentState;
        }

        public string ProcessEvent(string eventTrigger)
        {
            //Finding the relevant Info Struct given key values of currentState and eventTrigger
            if (fst.ContainsKey(currentState) && fst[currentState].ContainsKey(eventTrigger))
            {
                Info info = fst[currentState][eventTrigger];

                //Multithreading each action so they run concurrently
                foreach (TimestampedAction i_action in info.actions)
                {
                    
                    Thread th = new Thread(Thread => { i_action(DateTime.Now, eventTrigger); });
                    th.Start();
                }

                //Changing to the next state and returning it
                SetCurrentState(info.nextState);
                return info.nextState;
            }

            //If there is no Finite State Table entry that is relevant then return the currentState (This should never be called)
            return currentState;

        }

        public void SetCurrentState(string state)
        {
            currentState = state;

        }

        public void SetNextState(string state, string nextState, string eventTrigger)
        {
            //Creating an info struct for each Finite State Table entry
            Info info;
            info.nextState = nextState;
            info.actions = new List<TimestampedAction>();

            //First creating the state keys
            if (!fst.ContainsKey(state))
            {
                fst.Add(state, new Dictionary<string, Info>());
            }
            //Adding eventTrigger keys as the values for relevant state keys. An Info struct is the accomodating value for each eventTrigger key.
            if (!fst[state].ContainsKey(eventTrigger))
            {
               fst[state].Add(eventTrigger, info);
            }
            

        }

    }
}
