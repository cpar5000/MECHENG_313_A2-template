using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MECHENG_313_A2.Tasks
{

    //public delegate int DelFoo(int a, int b);

    public class FiniteStateMachine : IFiniteStateMachine
    {

        public struct Info {
            public string nextState;
            public List<TimestampedAction> actions;
        }

        public Dictionary<string, Dictionary<string, Info>> fst = new Dictionary<string, Dictionary<string, Info>>();
        private string currentState;

        public void AddAction(string state, string eventTrigger, TimestampedAction action)
        {

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
            //DelFoo foo = (a, b) => { return a + b; };
            //foo(1, 2);

            //DateTime time = DateTime.Now;

            if (fst.ContainsKey(currentState) && fst[currentState].ContainsKey(eventTrigger))
            {
                TimestampedAction temp;
                Info info = fst[currentState][eventTrigger];
                SetCurrentState(info.nextState);

                for (var i = 0; i < info.actions.Count; i++)
                {
                    temp = info.actions[i];
                    temp(DateTime.Now, eventTrigger);
                }

                SetCurrentState(info.nextState);
                return info.nextState;
            }

            return currentState;

        }

        public void SetCurrentState(string state)
        {

            currentState = state;

        }

        public void SetNextState(string state, string nextState, string eventTrigger)
        {
            Info info;
            info.nextState = nextState;
            info.actions = new List<TimestampedAction>();
            if (!fst.ContainsKey(state))
            {
                fst.Add(state, new Dictionary<string, Info>());
            }
            if (!fst[state].ContainsKey(eventTrigger))
            {
               fst[state].Add(eventTrigger, info);
            }
            

        }

    }
}
