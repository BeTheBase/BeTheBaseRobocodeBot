using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    public enum Effect
    {
        DetectEnemy = (1 << 0),
        WeaponReady = (1 << 1),
        HasLowHealth = (1 << 2)
    }

    public abstract class Goal
    {
        public int Priority;

        public List<EffectState> Conditions;
        public List<EffectState> Effects;
        public Stack<Action> ActionStack = new Stack<Action>();

        public Agent agent;

        public abstract void Initialize(Agent agent);
        public abstract bool IsViable(EffectState state);
    }
}
