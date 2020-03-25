using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    public abstract class Action
    {
        public List<EffectState> Conditions;
        public List<EffectState> Effects;

        public int Cost;
        public Agent Actor;

        public abstract void Initialize(Agent actor);
        public abstract void IsViable();

        public abstract void OnEnterAction();
        public abstract void OnUpdateAction();
        public abstract void OnExitAction();

        public abstract void OnActionCompleted();
        public abstract void OnActionFailed();

    }
}
