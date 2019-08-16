using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;

namespace MCCommon
{
    public abstract class CommandBase : DynamicObject, IConsoleCommand
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        protected const string Yes = "Y";

        public abstract string Display { get; }

        public virtual uint Order { get; } = 0U;

        public virtual uint Group { get; } = 0U;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Input(binder.Name);

            return result != null;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            InnerMap[binder.Name] = value;

            return true;
        }

        public virtual Task DoInput()
        {
            return Task.FromResult(true);
        }

        public virtual Task RunCommand()
        {
            return Task.FromResult(true);
        }

        public virtual bool Continue()
        {
            return true;
        }

        protected bool Continue(string prompt)
        {
            Console.Write($"{prompt} (y/n) : ");

            var answer = Console.ReadLine();

            return Yes.Equals(answer, StringComparison.OrdinalIgnoreCase);
        }

        protected Dictionary<string, object> InnerMap { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        protected object Input(string key) => InnerMap.ContainsKey(key) && InnerMap[key] != null ? InnerMap[key] : default;

        protected dynamic Me => this;

        protected void GetSetExistingValue(
            Func<bool> testForDefaultValue,
            Func<object> getDefaultValue,
            string prompt,
            Action<object> setDefaultValue,
            Action<string> setValueFromInput)
        {
            if (testForDefaultValue())
            {
                Console.Write($"{prompt} ({getDefaultValue()}) : ");

                var res = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(res))
                {
                    setDefaultValue(getDefaultValue());
                }
                else
                {
                    setValueFromInput(res);
                }
            }
            else
            {
                Console.Write($"{prompt} : ");

                var res = Console.ReadLine();

                setValueFromInput(res);
            }
        }

        protected void ResetStopwatch() => _stopwatch.Reset();

        protected void StartStopwatch() => _stopwatch.Start();

        protected void StopStopwatch() => _stopwatch.Stop();

        protected long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;
    }
}
