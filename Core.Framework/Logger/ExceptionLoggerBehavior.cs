using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework.Logger
{
    public class ExceptionLoggerBehavior : IInterceptionBehavior
    {
        private readonly ILoggerUtility _logger;

        public ExceptionLoggerBehavior(ILoggerUtility logger)
        {
            _logger = logger;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            IMethodReturn result = getNext()(input, getNext);

            if (result.Exception != null)
            {
                var reflectedClassType = input.MethodBase.ReflectedType;

                var message = string.Format("\n---------------------------------------- \nException occurred in {0}.\nParameters: {1}\nException: {2} \nStack: {3}",
                              input.MethodBase,
                              input.Inputs.Cast<object>(),
                              result.Exception,
                              result.Exception.StackTrace);

                _logger.Error(reflectedClassType.ToString(), message);
            }
            return result;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
